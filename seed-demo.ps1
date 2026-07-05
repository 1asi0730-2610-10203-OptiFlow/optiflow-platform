# OptiFlow - demo seed (ASCII-safe for PowerShell 5.1)
# Creates two ready-to-use logins against the local dev API:
#   * admin  -> business owner: onboarded account + active subscription + a few patients
#   * client -> patient-portal user whose profile resolves via the patient-center by-email endpoint
# Idempotent: re-running signs in existing users instead of failing.

$BASE = "http://localhost:5238"

$ADMIN_EMAIL = "admin@optiflow.com";  $ADMIN_PASS = "Admin123!"
$CLIENT_EMAIL = "client@optiflow.com"; $CLIENT_PASS = "Client123!"

function Post($url, $body, $token) {
    $hdr = @{ "Content-Type" = "application/json" }
    if ($token) { $hdr["Authorization"] = "Bearer $token" }
    try {
        $json = if ($body -ne $null) { $body | ConvertTo-Json -Depth 5 } else { "{}" }
        return Invoke-RestMethod -Method POST -Uri "$BASE$url" -Headers $hdr -Body $json -ErrorAction Stop
    } catch {
        $code = $_.Exception.Response.StatusCode.value__
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $detail = $reader.ReadToEnd()
        } catch { $detail = "$_" }
        Write-Warning "  POST $url -> $code  $detail"
        return $null
    }
}

function Get-Api($url, $token) {
    $hdr = @{ "Content-Type" = "application/json" }
    if ($token) { $hdr["Authorization"] = "Bearer $token" }
    try { return Invoke-RestMethod -Method GET -Uri "$BASE$url" -Headers $hdr -ErrorAction Stop }
    catch { return $null }
}

# Sign up, or sign in if the user already exists. Returns the auth payload (id, token, accountId).
function Ensure-User($email, $password) {
    $auth = Post "/api/v1/authentication/sign-up" @{ email = $email; password = $password }
    if (-not $auth) {
        $auth = Post "/api/v1/authentication/sign-in" @{ email = $email; password = $password }
    }
    if (-not $auth) { throw "Could not authenticate $email" }
    return $auth
}

Write-Host ""
Write-Host "=== OptiFlow demo seed  $(Get-Date -Format 'yyyy-MM-dd HH:mm') ===" -ForegroundColor Cyan
Write-Host "API: $BASE"

# --- 1. Admin: user + business account ------------------------------------
Write-Host ""
Write-Host "[admin] $ADMIN_EMAIL" -ForegroundColor Cyan
$admin = Ensure-User $ADMIN_EMAIL $ADMIN_PASS
$adminToken = $admin.token
Write-Host "  signed in, userId=$($admin.id)"

if ($admin.accountId) {
    $adminAccountId = $admin.accountId
    Write-Host "  account already onboarded -> id $adminAccountId" -ForegroundColor DarkYellow
} else {
    $acc = Post "/api/v1/accounts" @{ name = "OptiFlow Demo Clinic" } $adminToken
    if (-not $acc) { throw "Could not onboard admin account" }
    $adminAccountId = $acc.id
    Write-Host "  onboarded account '$($acc.name)' -> id $adminAccountId" -ForegroundColor Green
}

# --- 2. Subscription plans (shared global catalog) ------------------------
Write-Host ""
Write-Host "[plans]" -ForegroundColor Cyan
$plans = Get-Api "/api/v1/plans" $adminToken
if (-not $plans -or $plans.Count -eq 0) {
    $plansData = @(
        @{ name = "Plan Basico Mensual";       tier = "BASIC";        price = 49.90;  description = "Hasta 2 usuarios" },
        @{ name = "Plan Professional Mensual"; tier = "PROFESSIONAL"; price = 99.90;  description = "Hasta 5 usuarios" },
        @{ name = "Plan Enterprise Mensual";   tier = "ENTERPRISE";   price = 179.90; description = "Usuarios ilimitados" }
    )
    foreach ($p in $plansData) { Post "/api/v1/plans" $p $adminToken | Out-Null }
    $plans = Get-Api "/api/v1/plans" $adminToken
}
Write-Host "  $($plans.Count) plans available"

# --- 3. Admin subscription (best-effort) ----------------------------------
$professional = $plans | Where-Object { $_.tier -eq "PROFESSIONAL" } | Select-Object -First 1
if (-not $professional) { $professional = $plans | Select-Object -First 1 }
if ($professional) {
    $sub = Post "/api/v1/subscriptions" @{
        adminId = 1; planId = $professional.id; tier = $professional.tier
        amount = $professional.price; paymentMethod = "CREDIT_CARD"
    } $adminToken
    if ($sub) { Write-Host "  subscribed to '$($professional.name)'" -ForegroundColor Green }
}

# --- 4. Patients (incl. the client's own record) --------------------------
Write-Host ""
Write-Host "[patients]" -ForegroundColor Cyan
$patients = @(
    @{ firstName = "Cliente"; lastName = "Demo";        dni = "70000001"; phone = "+51 987 000 001"; email = $CLIENT_EMAIL;                birthDate = "1995-05-20" },
    @{ firstName = "Maria";   lastName = "Gonzales";    dni = "70000002"; phone = "+51 987 000 002"; email = "maria.demo@mail.com";        birthDate = "1988-03-14" },
    @{ firstName = "Jorge";   lastName = "Ramirez";     dni = "70000003"; phone = "+51 987 000 003"; email = "jorge.demo@mail.com";        birthDate = "1979-11-05" }
)
$created = 0
foreach ($p in $patients) {
    $r = Post "/api/v1/patients" $p $adminToken
    if ($r) { $created++ }
}
Write-Host "  $created patients ensured (client record: $CLIENT_EMAIL)"

# --- 5. Client: patient-portal user (no business account) -----------------
Write-Host ""
Write-Host "[client] $CLIENT_EMAIL" -ForegroundColor Cyan
$client = Ensure-User $CLIENT_EMAIL $CLIENT_PASS
Write-Host "  signed in, userId=$($client.id)"
$profile = Get-Api "/api/v1/patient-center/patients/by-email?email=$CLIENT_EMAIL" $client.token
if ($profile) { Write-Host "  patient profile resolves: $($profile.firstName) $($profile.lastName)" -ForegroundColor Green }
else { Write-Host "  WARNING: patient profile did not resolve" -ForegroundColor Red }

# --- Summary --------------------------------------------------------------
Write-Host ""
Write-Host "=== Demo seed complete ===" -ForegroundColor Green
Write-Host "  ADMIN  (business owner / dashboard):" -ForegroundColor Cyan
Write-Host "    email:    $ADMIN_EMAIL"
Write-Host "    password: $ADMIN_PASS"
Write-Host "  CLIENT (patient portal):" -ForegroundColor Cyan
Write-Host "    email:    $CLIENT_EMAIL"
Write-Host "    password: $CLIENT_PASS"
Write-Host ""
