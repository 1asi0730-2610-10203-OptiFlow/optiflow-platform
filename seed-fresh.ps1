# OptiFlow - fresh, idempotent seed (PowerShell 5.1 / pwsh)
#
# Rebuilds a clean demo dataset from scratch. Safe to re-run: it wipes prior demo data first,
# so every run yields the same fresh state with NO duplicate-email users.
#
# What it does:
#   1) RESET  - deletes all transactional/user rows (keeps schema, migrations and the plan catalog).
#   2) OPTICS - onboards optic admin accounts via the API.
#   3) ACTIVATE - the API leaves a selected plan as PENDING_PAYMENT (real-Stripe gate), so this flips
#                 the subscription to ACTIVE directly in the DB — the only step that needs DB access.
#   4) SEED   - patients (globally UNIQUE emails), inventory, a lab, and fully-linked orders
#               (sale -> work order -> link) so each order shows on the patient portal.
#   5) VERIFY - signs in as a seeded patient and confirms their orders come back.
#
# Usage (local):
#   pwsh ./seed-fresh.ps1
# Usage (Azure deployment): point at the API AND that database. Deleting production rows is
# irreversible - only run it when you really want a clean slate.
#   pwsh ./seed-fresh.ps1 -BaseUrl https://optiflow.azurewebsites.net `
#        -DbServer <host> -DbUser <user> -DbPassword <pw> -DbName <db>
#   pwsh ./seed-fresh.ps1 -SkipReset     # seed without wiping (may create duplicates - not recommended)

param(
    [string]$BaseUrl    = "http://localhost:5238",
    [string]$DbServer   = "localhost",
    [string]$DbUser     = "root",
    [string]$DbPassword = "password",
    [string]$DbName     = "optiflow_db",
    [string]$MysqlExe   = "C:\Program Files\MySQL\MySQL Server 8.0\bin\mysql.exe",
    # Azure Database for MySQL enforces TLS; PREFERRED works locally and on Azure. Pass REQUIRED if Azure rejects it.
    [ValidateSet("PREFERRED","REQUIRED","DISABLED")]
    [string]$SslMode    = "PREFERRED",
    [int]$Optics        = 2,
    [int]$PatientsEach  = 6,
    [switch]$SkipReset
)

$ErrorActionPreference = "Stop"
$BASE = $BaseUrl.TrimEnd('/')
$script:calls = 0; $script:fails = 0

# ── API helpers ───────────────────────────────────────────────────────────────
function Invoke-Api($method, $url, $body, $token) {
    $script:calls++
    $hdr = @{ "Content-Type" = "application/json" }
    if ($token) { $hdr["Authorization"] = "Bearer $token" }
    try {
        if ($null -ne $body) {
            $json = $body | ConvertTo-Json -Depth 6
            return Invoke-RestMethod -Method $method -Uri "$BASE$url" -Headers $hdr -Body $json -ErrorAction Stop
        }
        return Invoke-RestMethod -Method $method -Uri "$BASE$url" -Headers $hdr -ErrorAction Stop
    } catch {
        $script:fails++
        $code = try { $_.Exception.Response.StatusCode.value__ } catch { "ERR" }
        Write-Warning ("  {0} {1} -> {2}" -f $method, $url, $code)
        return $null
    }
}
function Post($url, $body, $token) { return Invoke-Api "POST"  $url $body  $token }
function Put($url, $body, $token)  { return Invoke-Api "PUT"   $url $body  $token }
function Patch($url, $body, $token){ return Invoke-Api "PATCH" $url $body  $token }
function GetAll($url, $token)      { return Invoke-Api "GET"   $url $null   $token }
function DayShort($offset)         { return (Get-Date).AddDays(-$offset).ToString("yyyy-MM-dd") }
function Day($offset)              { return (Get-Date).AddDays(-$offset).ToString("yyyy-MM-ddTHH:mm:ss") }

# ── DB helper (only for reset + subscription activation) ──────────────────────
function Invoke-Sql($sql) {
    $env:MYSQL_PWD = $DbPassword
    try {
        $out = & $MysqlExe -h $DbServer -u $DbUser --protocol=TCP "--ssl-mode=$SslMode" $DbName -N -B -e $sql 2>&1
        if ($LASTEXITCODE -ne 0) { throw "mysql failed: $out" }
        return $out
    } finally { Remove-Item Env:\MYSQL_PWD -ErrorAction SilentlyContinue }
}

# Wipe every transactional/user table. Keeps subscription_plans (reseeded by the app on boot),
# lens_materials (reference data) and __EFMigrationsHistory.
$WipeTables = @(
    "system_notifications","patient_notifications","analytics_reports","staff_metrics",
    "stock_audit_logs","payments","sale_items","sales","work_orders","laboratories",
    "prescriptions","clinical_records","patients","products","suppliers",
    "staff_roles","staff","subscription_payments","subscription_billings","subscriptions",
    "password_recovery_tokens","accounts","users"
)
function Reset-Database {
    Write-Host "=== RESET: wiping demo data in $DbName ===" -ForegroundColor Yellow
    $stmts = @("SET FOREIGN_KEY_CHECKS=0;")
    foreach ($t in $WipeTables) { $stmts += "DELETE FROM ``$t``;" }
    $stmts += "SET FOREIGN_KEY_CHECKS=1;"
    Invoke-Sql ($stmts -join " ") | Out-Null
    Write-Host "  cleared $($WipeTables.Count) tables" -ForegroundColor Green
}

# ── Onboarding + activation ───────────────────────────────────────────────────
function Onboard-Optic($email, $password, $businessName) {
    Write-Host ""
    Write-Host "=== Optic: $businessName ($email) ===" -ForegroundColor Cyan
    $auth = Post "/api/v1/authentication/sign-up" @{ email = $email; password = $password; userType = "admin" } $null
    if (-not $auth) { $auth = Post "/api/v1/authentication/sign-in" @{ email = $email; password = $password } $null }
    if (-not $auth) { throw "Could not authenticate $email" }
    $token = $auth.token
    Post "/api/v1/accounts" @{ name = $businessName } $token | Out-Null

    # Select the first monthly plan, then flip it ACTIVE in the DB (the API keeps it PENDING_PAYMENT
    # until a real Stripe payment, which we can't do while seeding).
    $plans  = GetAll "/api/v1/plans" $token
    $plan   = $plans | Where-Object { $_.name -match 'Mensual' } | Select-Object -First 1
    if (-not $plan) { $plan = $plans | Select-Object -First 1 }
    Post "/api/v1/subscriptions" @{ adminId = 1; planId = $plan.id; tier = $plan.tier; amount = $plan.price; paymentMethod = "CREDIT_CARD" } $token | Out-Null
    Invoke-Sql "UPDATE subscriptions SET status='ACTIVE', start_date=NOW(), end_date=(NOW() + INTERVAL 1 YEAR) WHERE account_id='$($auth.accountId)' AND status='PENDING_PAYMENT';" | Out-Null
    $me = GetAll "/api/v1/subscriptions/me" $token
    if ($me -and $me.hasActiveSubscription) { Write-Host "  subscription ACTIVE" -ForegroundColor Green }
    else { Write-Warning "  subscription NOT active - gated endpoints will 403" }
    return @{ Token = $token; AccountId = "$($auth.accountId)"; Name = $businessName }
}

# ── Seeding one optic's patients + orders ─────────────────────────────────────
$FirstNames = @("Alejandro","Valentina","Diego","Lucia","Sebastian","Camila","Mateo","Isabela","Emilio","Sofia")
$LastNames  = @("Garcia Lopez","Morales Cruz","Rodriguez Perez","Hernandez Vega","Vargas Quispe","Torres Mendoza","Flores Salas","Diaz Ramirez","Castro Huanca","Reyes Chavez")
$LensTypes  = @("Monofocal","Bifocal","Progresiva","Anti-reflejo")

function Seed-Optic($ctx, $opticIndex) {
    $token = $ctx.Token
    $tag   = "o$opticIndex"

    # A lab + real inventory: a frame AND a lens product, so work orders reference actual stock
    # (lensProductId/frameProductId) and the inventory-depletion integration has something to reduce.
    $lab = Post "/laboratories" @{ name = "LabVision $($ctx.Name)"; phone = "+51 921 100 00$opticIndex"; email = "lab.$tag@labvision.pe" } $token
    $labId = if ($lab) { $lab.id } else { 1 }
    $sup = Post "/suppliers" @{ name = "VisionPro $($ctx.Name)"; contactPerson = "Carlos Medina"; phone = "+51 912 001 00$opticIndex"; email = "sup.$tag@visionpro.pe" } $token
    $supId = if ($sup) { $sup.id } else { 0 }
    $frameProd = Post "/products" @{ category = "Frames"; supplierId = $supId; supplierName = "VisionPro"; sku = "MON-$tag-001"; name = "Montura Ray-Ban RB5154"; brand = "Ray-Ban"; model = "RB5154"; price = 349.90; stock = 120; minimumStockThreshold = 10 } $token
    $frameProdId = if ($frameProd) { $frameProd.id } else { $null }
    $frameName   = if ($frameProd) { $frameProd.name } else { "Montura Ray-Ban RB5154" }
    $lensProd  = Post "/products" @{ category = "Lenses"; supplierId = $supId; supplierName = "VisionPro"; sku = "LEN-$tag-001"; name = "Lente Essilor Varilux"; brand = "Essilor"; model = "Varilux X"; price = 520.00; stock = 120; minimumStockThreshold = 10 } $token
    $lensProdId = if ($lensProd) { $lensProd.id } else { $null }

    $seeded = @()
    for ($i = 0; $i -lt $PatientsEach; $i++) {
        $fn = $FirstNames[$i % $FirstNames.Count]
        $ln = $LastNames[$i % $LastNames.Count]
        # GLOBALLY-unique email (per optic + index) so no two client users ever collide.
        $email = ("{0}.{1}.{2}{3}@mail.com" -f $fn.ToLower(), (($ln -replace '\s','').ToLower()), $tag, $i)
        $dni   = "" + (10000000 + $opticIndex * 1000 + $i)
        $pat = Post "/api/v1/patients" @{ firstName = $fn; lastName = $ln; dni = $dni; phone = ("+51 987 0{0:D2} 0{1:D2}" -f $opticIndex, $i); email = $email; birthDate = (Get-Date).AddYears(-(20 + $i * 3)).ToString("yyyy-MM-dd") } $token
        if (-not $pat) { continue }

        # Two linked orders per patient: sale (carries patientId) -> work order (saleId) -> explicit link.
        for ($o = 0; $o -lt 2; $o++) {
            $total   = [math]::Round(300 + (Get-Random -Minimum 50 -Maximum 400), 2)
            $advance = [math]::Round($total * 0.4, 2)
            $items   = if ($frameProdId) { @(@{ productId = $frameProdId; quantity = 1 }) } else { @() }
            $sale = Post "/sales" @{
                invoiceNumber = ("FAC-$tag-{0:D2}{1}" -f $i, $o); labOrderNumber = $null
                patientId = $pat.id; patientName = "$fn $ln"; userId = 1; userName = "Seed"
                totalAmount = $total; advance = $advance; discountCode = $null; discountAmount = 0
                paymentMethod = "CREDIT_CARD"; createdAt = (Day (10 - $o * 3)); deliveredAt = $null; notes = $null
                items = @($items)   # force a JSON array; PS 5.1 unwraps a single-element array to an object otherwise
            } $token
            if (-not $sale) { continue }
            $wo = Post "/work-orders" @{
                saleId = $sale.id; recipeId = 0; labId = $labId
                patientName = "$fn $ln"; laboratoryName = "LabVision $($ctx.Name)"
                lensType = ($LensTypes | Get-Random); lensProductId = $lensProdId
                frame = $frameName; frameProductId = $frameProdId
                prescription = "Segun receta del paciente"; priority = "normal"
                deliveryDate = (DayShort (-(Get-Random -Minimum 5 -Maximum 21))); deposit = $advance; total = $total
            } $token
            if ($wo) { Patch "/work-orders/$($wo.id)/sale" @{ saleId = $sale.id } $token | Out-Null }
        }
        $seeded += @{ Email = $email; Name = "$fn $ln" }
    }
    Write-Host "  $($seeded.Count) patients, ~$($seeded.Count * 2) linked orders" -ForegroundColor Green
    return $seeded
}

# ── Run ───────────────────────────────────────────────────────────────────────
Write-Host "OptiFlow fresh seed -> $BASE" -ForegroundColor White
if (-not $SkipReset) { Reset-Database } else { Write-Host "(reset skipped)" -ForegroundColor DarkYellow }

$allPatients = @()
for ($n = 1; $n -le $Optics; $n++) {
    $ctx = Onboard-Optic ("optic$n@optiflow.demo") "Seed1234!" ("Optica Demo $n")
    $allPatients += (Seed-Optic $ctx $n)
}

# ── Verify: a seeded patient can see their orders (proves the portal end-to-end) ──
Write-Host ""
Write-Host "=== VERIFY: client portal ===" -ForegroundColor Cyan
$sample = $allPatients | Select-Object -First 1
if ($sample) {
    $login = Post "/api/v1/authentication/sign-in/client" @{ username = $sample.Email } $null
    if ($login -and $login.token) {
        $me = GetAll ("/api/v1/patient-center/patients/by-email?email=" + [uri]::EscapeDataString($sample.Email)) $login.token
        if ($me) {
            $orders = GetAll "/api/v1/patients/$($me.id)/orders" $login.token
            $count = if ($orders) { @($orders).Count } else { 0 }
            if ($count -gt 0) { Write-Host "  OK: $($sample.Name) sees $count orders" -ForegroundColor Green }
            else { Write-Warning "  $($sample.Name) resolved (patient id $($me.id)) but sees 0 orders" }
        } else { Write-Warning "  could not resolve patient by email" }
    } else { Write-Warning "  client sign-in failed for $($sample.Email)" }
}

Write-Host ""
Write-Host ("Done. API calls: {0}, failures: {1}, patients: {2}" -f $script:calls, $script:fails, $allPatients.Count) -ForegroundColor White
