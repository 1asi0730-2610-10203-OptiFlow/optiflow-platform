# OptiFlow - reset seed (ASCII-safe, PowerShell 5.1)
# Seeds a fresh database with:
#   * 2 subscribed admins (each: optic + active subscription + clients with orders)
#   * 2 unsubscribed admins (optic auto-provisioned, no plan -> gated)
# Clients are provisioned automatically when the admin registers them as patients, and log in
# passwordless with their email as username.

$BASE = "http://localhost:5238"

function Post($url, $body, $token) {
    $hdr = @{ "Content-Type" = "application/json" }
    if ($token) { $hdr["Authorization"] = "Bearer $token" }
    try {
        $json = if ($null -ne $body) { $body | ConvertTo-Json -Depth 6 } else { "{}" }
        return Invoke-RestMethod -Method POST -Uri "$BASE$url" -Headers $hdr -Body $json -ErrorAction Stop
    } catch {
        $code = $_.Exception.Response.StatusCode.value__
        Write-Warning "  POST $url -> $code"
        return $null
    }
}
function Patch($url, $body, $token) {
    $hdr = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }
    try { return Invoke-RestMethod -Method PATCH -Uri "$BASE$url" -Headers $hdr -Body ($body | ConvertTo-Json -Depth 6) -ErrorAction Stop }
    catch { Write-Warning "  PATCH $url -> $($_.Exception.Response.StatusCode.value__)"; return $null }
}

function New-Admin($email, $pass) {
    $auth = Post "/api/v1/authentication/sign-up" @{ email = $email; password = $pass; userType = "admin" }
    if (-not $auth) { $auth = Post "/api/v1/authentication/sign-in" @{ email = $email; password = $pass } }
    if (-not $auth) { throw "Could not authenticate admin $email" }
    return $auth
}

$now   = (Get-Date).ToString("o")
$later = (Get-Date).AddYears(1).ToString("o")

Write-Host ""
Write-Host "=== OptiFlow reset seed  $(Get-Date -Format 'yyyy-MM-dd HH:mm') ===" -ForegroundColor Cyan

# --- Plans (shared catalog) -----------------------------------------------
$adminBootstrap = New-Admin "admin1@optiflow.com" "Admin123!"
$plansData = @(
    @{ name = "Plan Basico Mensual";       tier = "BASiC";        price = 99.00;   description = "Hasta 2 usuarios" },
    @{ name = "Plan Professional Mensual"; tier = "PROFESSIONAL"; price = 248.00;  description = "Hasta 5 usuarios" },
    @{ name = "Plan Enterprise Mensual";   tier = "ENTERPRISE";   price = 499.00;  description = "Usuarios ilimitados" },
    @{ name = "Plan Basico Anual";         tier = "BASiC";        price = 984.00;  description = "Hasta 2 usuarios - facturacion anual" },
    @{ name = "Plan Professional Anual";   tier = "PROFESSIONAL"; price = 2472.00; description = "Hasta 5 usuarios - facturacion anual" },
    @{ name = "Plan Enterprise Anual";     tier = "ENTERPRISE";   price = 4968.00; description = "Usuarios ilimitados - facturacion anual" }
)
$existing = @()
try { $existing = Invoke-RestMethod -Method GET -Uri "$BASE/api/v1/plans" -Headers @{ Authorization = "Bearer $($adminBootstrap.token)" } } catch {}
if (-not $existing -or $existing.Count -eq 0) {
    foreach ($p in $plansData) { Post "/api/v1/plans" $p $adminBootstrap.token | Out-Null }
}
$plans = Invoke-RestMethod -Method GET -Uri "$BASE/api/v1/plans" -Headers @{ Authorization = "Bearer $($adminBootstrap.token)" }
Write-Host "plans: $($plans.Count)"

function Seed-Optic($admin, $opticLabel, $planTier, $clients) {
    $t = $admin.token
    Write-Host ""
    Write-Host "[optic] $opticLabel ($($admin.email))" -ForegroundColor Cyan

    # Active subscription
    $plan = $plans | Where-Object { $_.tier -eq $planTier } | Select-Object -First 1
    $sub = Post "/api/v1/subscriptions" @{ adminId = 1; planId = $plan.id; tier = $plan.tier; amount = $plan.price; paymentMethod = "CARD" } $t
    if ($sub) {
        $act = Post "/api/v1/subscriptions/$($sub.id)/activate" @{ tier = $plan.tier; startDate = $now; endDate = $later } $t
        if ($act) { Write-Host "  subscription ACTIVE ($($plan.tier))" -ForegroundColor Green }
    }

    # Default staff roles for the Settings > Roles section
    $roles = @(
        @{ name="ADMINISTRADOR"; displayName="Administrador"; description="Acceso total al sistema"; color="#ef4444"; permissions=@("settings","reports","users","full_access") },
        @{ name="OPTOMETRISTA";  displayName="Optometrista";  description="Historias clinicas, recetas"; color="#8b5cf6"; permissions=@("dashboard","clinical","prescriptions","appointments") },
        @{ name="ASESOR";        displayName="Asesor de Ventas"; description="Ventas, inventario, pacientes"; color="#3b82f6"; permissions=@("sales","inventory","lab_orders") },
        @{ name="RECEPCIONISTA"; displayName="Recepcionista"; description="Citas y registros (solo lectura)"; color="#10b981"; permissions=@("appointments") }
    )
    foreach ($r in $roles) { Post "/roles" $r $t | Out-Null }

    # Supplier + product + lab (needed for orders)
    $sup = Post "/suppliers" @{ name = "Distribuidora $opticLabel"; contactPerson = "Contacto"; phone = "+51 900 000 000"; email = "prov.$($admin.email)" } $t
    $lab = Post "/laboratories" @{ name = "Lab $opticLabel"; phone = "+51 921 000 000"; email = "lab.$($admin.email)" } $t
    $prod = Post "/products" @{ category = "Lenses"; supplierId = $sup.id; supplierName = $sup.name; sku = "LEN-$($admin.email.Substring(0,5))"; name = "Lente Progresivo"; brand = "Essilor"; model = "Varilux"; price = 520.00; stock = 30; minimumStockThreshold = 5 } $t

    # Clients: register as patients (auto-provisions client users) + give each an order
    foreach ($c in $clients) {
        $pat = Post "/api/v1/patients" @{ firstName = $c.first; lastName = $c.last; dni = $c.dni; phone = "+51 987 000 000"; email = $c.email; birthDate = "1990-01-01" } $t
        if (-not $pat) { continue }
        $patName = "$($c.first) $($c.last)"
        $sale = Post "/sales" @{
            invoiceNumber = "FAC-$($c.dni)"; labOrderNumber = $null; patientId = $pat.id; patientName = $patName
            userId = 1; userName = "$opticLabel"; totalAmount = 620.00; advance = 200.00
            discountCode = $null; discountAmount = 0; paymentMethod = "CARD"; createdAt = $now; deliveredAt = $null; notes = $null
            items = @(@{ productId = $prod.id; quantity = 1 })
        } $t
        if ($sale) {
            $wo = Post "/work-orders" @{
                saleId = $sale.id; recipeId = 0; labId = $lab.id; patientName = $patName; laboratoryName = $lab.name
                lensType = "Progresiva"; lensProductId = $null; frame = "Acetato negro"; frameProductId = $null
                prescription = "Segun receta"; priority = "normal"; deliveryDate = (Get-Date).AddDays(7).ToString("yyyy-MM-dd")
                deposit = 200.00; total = 620.00
            } $t
            if ($wo) { Patch "/work-orders/$($wo.id)/status" @{ status = "IN_PRODUCTION" } $t | Out-Null }
        }
        Write-Host "  client $($c.email) -> patient #$($pat.id) with 1 order"
    }
}

Seed-Optic $adminBootstrap "Optica VisionPlus" "PROFESSIONAL" @(
    @{ first = "Ana";  last = "Torres";  dni = "70010001"; email = "client1@optiflow.com" },
    @{ first = "Luis"; last = "Ramirez"; dni = "70010002"; email = "client2@optiflow.com" }
)

$admin2 = New-Admin "admin2@optiflow.com" "Admin123!"
Seed-Optic $admin2 "Optica ClearView" "ENTERPRISE" @(
    @{ first = "Marta"; last = "Diaz";  dni = "70020001"; email = "client3@optiflow.com" },
    @{ first = "Pedro"; last = "Gomez"; dni = "70020002"; email = "client4@optiflow.com" }
)

# --- Unsubscribed admins (optic auto-provisioned, no plan) -----------------
New-Admin "admin3@optiflow.com" "Admin123!" | Out-Null
New-Admin "admin4@optiflow.com" "Admin123!" | Out-Null
Write-Host ""
Write-Host "[unsubscribed] admin3@optiflow.com, admin4@optiflow.com (gated until they buy a plan)" -ForegroundColor DarkYellow

Write-Host ""
Write-Host "=== Seed complete ===" -ForegroundColor Green
Write-Host "SUBSCRIBED admins (password Admin123!):  admin1@optiflow.com, admin2@optiflow.com"
Write-Host "UNSUBSCRIBED admins (password Admin123!): admin3@optiflow.com, admin4@optiflow.com"
Write-Host "CLIENTS (passwordless, username = email):"
Write-Host "  client1@optiflow.com, client2@optiflow.com  (Optica VisionPlus)"
Write-Host "  client3@optiflow.com, client4@optiflow.com  (Optica ClearView)"
Write-Host ""
