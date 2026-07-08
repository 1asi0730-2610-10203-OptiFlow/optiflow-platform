# OptiFlow - comprehensive data seed (ASCII-safe for PowerShell 5.1)
#
# Populates the platform as if it had been in daily use for ~1 month, exercising every
# write endpoint across all modules (IAM, Subscription, Staff, Inventory, Clinical,
# LabAndOrders, Sales, PatientCenter). Two full optic accounts get a month of realistic
# history; a throwaway account exercises the destructive subscription-lifecycle and IAM
# endpoints so they are covered without disturbing the browsable data.
#
# Usage:
#   pwsh ./seed.ps1                                  # seeds the deployed backend
#   pwsh ./seed.ps1 -BaseUrl http://localhost:5238   # seeds a local dev backend
#   pwsh ./seed.ps1 -Days 30 -Tag demo
#
# Note on subscriptions: gated endpoints (products, sales, etc.) require an ACTIVE
# subscription. The seed obtains one through the normal checkout/subscription flow, which
# activates on a dev backend (no real Stripe key) and on the current deployed backend.
# Run this before deploying the "no activation before payment" fix; already-seeded
# subscriptions keep working afterwards.

param(
    [string]$BaseUrl = "https://optiflow.azurewebsites.net",
    [int]$Days = 30,
    [string]$Password = "Seed1234!",
    [string]$Tag = ("m" + (Get-Date -Format "MMddHHmm"))
)

$BASE = $BaseUrl.TrimEnd('/')
$script:calls = 0
$script:fails = 0

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
        $detail = ""
        try {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $detail = $reader.ReadToEnd()
        } catch { $detail = "$_" }
        Write-Warning ("  {0} {1} -> {2} {3}" -f $method, $url, $code, ($detail -replace '\s+', ' ').Substring(0, [math]::Min(120, $detail.Length)))
        return $null
    }
}
function Post($url, $body, $token)  { return Invoke-Api "POST"  $url $body  $token }
function Put($url, $body, $token)   { return Invoke-Api "PUT"   $url $body  $token }
function Patch($url, $body, $token) { return Invoke-Api "PATCH" $url $body  $token }
function Del($url, $token)          { return Invoke-Api "DELETE" $url $null  $token }
function Get-All($url, $token)      { return Invoke-Api "GET"   $url $null   $token }

function Day($offset)      { return (Get-Date).AddDays(-$offset).ToString("yyyy-MM-ddTHH:mm:ss") }
function DayShort($offset) { return (Get-Date).AddDays(-$offset).ToString("yyyy-MM-dd") }
function Pick($arr)        { return $arr[(Get-Random -Minimum 0 -Maximum $arr.Count)] }

function New-Account($email, $password, $businessName) {
    Write-Host ""
    Write-Host "=== Onboarding $businessName ($email) ===" -ForegroundColor Cyan
    $auth = Post "/api/v1/authentication/sign-up" @{ email = $email; password = $password; userType = "admin" } $null
    if (-not $auth) { $auth = Post "/api/v1/authentication/sign-in" @{ email = $email; password = $password } $null }
    if (-not $auth) { throw "Could not authenticate $email" }
    $token = $auth.token
    $acc = Post "/api/v1/accounts" @{ name = $businessName } $token
    $accId = if ($acc) { $acc.id } else { $auth.accountId }
    Write-Host "  userId=$($auth.id)  accountId=$accId" -ForegroundColor Green
    return @{ Email = $email; Token = $token; UserId = $auth.id; AccountId = "$accId"; Name = $businessName }
}

function Enable-Subscription($ctx, $planId, $tier, $amount) {
    # SelectSubscriptionPlan; on a dev backend / current prod this yields an active subscription.
    Post "/api/v1/subscriptions" @{ adminId = 1; planId = $planId; tier = $tier; amount = $amount; paymentMethod = "CREDIT_CARD" } $ctx.Token | Out-Null
    $me = Get-All "/api/v1/subscriptions/me" $ctx.Token
    if ($me -and $me.hasActiveSubscription) {
        Write-Host "  subscription ACTIVE (id=$($me.subscription.id))" -ForegroundColor Green
        return $me.subscription.id
    }
    Write-Warning "  subscription not active after checkout; gated endpoints may 403"
    return $null
}

function Seed-Account($ctx, $planIds, $planTiers, $planAmounts) {
    $token  = $ctx.Token
    $sfx    = $ctx.AccountId.Substring(0, 4)
    $accNum = [Convert]::ToInt32($sfx, 16)   # 0-65535, makes DNIs unique per account

    # 1) Activate a subscription so gated endpoints are reachable
    $subId = Enable-Subscription $ctx $planIds[1] $planTiers[1] $planAmounts[1]

    # 2) Roles + Staff (staff ids become the Sales userId)
    Write-Host "  [roles + staff]" -ForegroundColor DarkCyan
    $rolesData = @(
        @{ name="reception"; displayName="Recepcion";     description="Atencion al cliente"; color="#3B82F6"; permissions=@("sales.read","patients.read") },
        @{ name="optometry"; displayName="Optometria";     description="Examenes visuales";    color="#10B981"; permissions=@("clinical.read","clinical.write") },
        @{ name="manager";   displayName="Gerencia";       description="Administracion";       color="#F59E0B"; permissions=@("all") }
    )
    foreach ($r in $rolesData) { Post "/roles" $r $token | Out-Null }
    $staffData = @(
        @{ employeeCode="EMP-$sfx-01"; firstName="Maria";    lastName="Reception"; email="maria.$sfx@$($sfx).pe";  phone="+51 940 000 001"; role="Recepcion";    department="Ventas";      status="active"; activeToday=$true },
        @{ employeeCode="EMP-$sfx-02"; firstName="Jose";     lastName="Optometra"; email="jose.$sfx@$($sfx).pe";   phone="+51 940 000 002"; role="Optometria";  department="Clinica";     status="active"; activeToday=$true },
        @{ employeeCode="EMP-$sfx-03"; firstName="Ana";      lastName="Gerente";   email="ana.$sfx@$($sfx).pe";    phone="+51 940 000 003"; role="Gerencia";    department="Direccion";   status="active"; activeToday=$true },
        @{ employeeCode="EMP-$sfx-04"; firstName="Luis";     lastName="Vendedor";  email="luis.$sfx@$($sfx).pe";   phone="+51 940 000 004"; role="Recepcion";   department="Ventas";      status="active"; activeToday=$false }
    )
    $staffIds = @()
    foreach ($s in $staffData) { $r = Post "/staff" $s $token; if ($r) { $staffIds += $r.id } }
    if ($staffIds.Count -eq 0) { $staffIds = @(1) }
    # exercise staff update
    if ($staffIds.Count -gt 3) {
        $upd = $staffData[3].Clone(); $upd.status = "inactive"; $upd.activeToday = $false
        Put "/staff/$($staffIds[3])" $upd $token | Out-Null
    }
    Write-Host "    $($staffIds.Count) staff, 3 roles" -ForegroundColor Green

    # 3) Suppliers
    Write-Host "  [suppliers]" -ForegroundColor DarkCyan
    $suppliersData = @(
        @{ name="VisionPro SA - $($ctx.Name)";        contactPerson="Carlos Medina"; phone="+51 912 001 001"; email="carlos.$sfx@visionpro.pe" },
        @{ name="OpticaGlobal - $($ctx.Name)";         contactPerson="Luisa Rios";    phone="+51 912 002 002"; email="luisa.$sfx@opticaglobal.pe" },
        @{ name="LensWorld - $($ctx.Name)";            contactPerson="Roberto Salas"; phone="+51 912 003 003"; email="roberto.$sfx@lensworld.pe" },
        @{ name="Essilor Andina - $($ctx.Name)";       contactPerson="Ana Torres";    phone="+51 912 004 004"; email="ana.$sfx@essilor.pe" },
        @{ name="Zeiss Distribuidores - $($ctx.Name)"; contactPerson="Pedro Vega";    phone="+51 912 005 005"; email="pedro.$sfx@zeiss.pe" },
        @{ name="Hoya Vision Peru - $($ctx.Name)";     contactPerson="Maria Castro";  phone="+51 912 006 006"; email="maria.$sfx@hoya.pe" }
    )
    $supIds = @(); $supNames = @()
    foreach ($s in $suppliersData) { $r = Post "/suppliers" $s $token; if ($r) { $supIds += $r.id; $supNames += $s.name } }
    if ($supIds.Count -eq 0) { $supIds = @(0); $supNames = @("Generic") }
    Write-Host "    $($supIds.Count) suppliers" -ForegroundColor Green

    # 4) Laboratories
    Write-Host "  [laboratories]" -ForegroundColor DarkCyan
    $labsData = @(
        @{ name="LabVision Central - $($ctx.Name)"; phone="+51 921 100 001"; email="central.$sfx@labvision.pe" },
        @{ name="OptiLab Norte - $($ctx.Name)";      phone="+51 921 100 002"; email="norte.$sfx@optilab.pe" },
        @{ name="Laboratorio San Borja - $($ctx.Name)"; phone="+51 921 100 003"; email="sanborja.$sfx@laboptica.pe" },
        @{ name="Crystal Lab - $($ctx.Name)";        phone="+51 921 100 004"; email="crystal.$sfx@crystallab.pe" }
    )
    $labIds = @(); $labNames = @()
    foreach ($l in $labsData) { $r = Post "/laboratories" $l $token; if ($r) { $labIds += $r.id; $labNames += $l.name } }
    if ($labIds.Count -eq 0) { $labIds = @(1); $labNames = @("Lab") }
    Write-Host "    $($labIds.Count) laboratories" -ForegroundColor Green

    # 5) Products (every EProductCategory)
    Write-Host "  [products]" -ForegroundColor DarkCyan
    $productDefs = @(
        @{ cat="Frames";        sku="MON-001"; name="Montura Ray-Ban RB5154";   brand="Ray-Ban";      model="RB5154";     price=349.90;  stock=25;  min=5  },
        @{ cat="Frames";        sku="MON-002"; name="Montura Oakley OX8046";     brand="Oakley";       model="OX8046";     price=419.90;  stock=18;  min=4  },
        @{ cat="Frames";        sku="MON-003"; name="Montura Prada VPR05X";      brand="Prada";        model="VPR05X";     price=699.00;  stock=8;   min=3  },
        @{ cat="Frames";        sku="MON-004"; name="Montura Gucci GG0396O";     brand="Gucci";        model="GG0396O";    price=890.00;  stock=6;   min=2  },
        @{ cat="ContactLenses"; sku="LC-001";  name="Lente Contacto Acuvue";     brand="Acuvue";       model="Oasys";      price=89.90;   stock=80;  min=20 },
        @{ cat="ContactLenses"; sku="LC-002";  name="Lente Contacto Biofinity";  brand="CooperVision"; model="Biofinity";  price=94.90;   stock=60;  min=15 },
        @{ cat="ContactLenses"; sku="LC-003";  name="Lente Contacto Dailies";    brand="Alcon";        model="DailiesTot"; price=99.90;   stock=12;  min=15 },
        @{ cat="Lenses";        sku="LO-001";  name="Lente Essilor Varilux";     brand="Essilor";      model="Varilux C";  price=520.00;  stock=40;  min=10 },
        @{ cat="Lenses";        sku="LO-002";  name="Lente Zeiss Individual";    brand="Zeiss";        model="Individual"; price=750.00;  stock=30;  min=8  },
        @{ cat="Lenses";        sku="LO-003";  name="Lente Hoya Sync III";       brand="Hoya";         model="Sync III";   price=310.00;  stock=50;  min=10 },
        @{ cat="Accessories";   sku="ACC-001"; name="Estuche Magnetico Premium"; brand="OptiCase";     model="OP-M01";     price=45.00;   stock=100; min=20 },
        @{ cat="Accessories";   sku="ACC-002"; name="Limpia Lentes Zeiss";       brand="Zeiss";        model="CL-100";     price=18.50;   stock=200; min=30 },
        @{ cat="Sunglasses";    sku="SUN-001"; name="Ray-Ban Aviator Classic";   brand="Ray-Ban";      model="RB3025";     price=259.90;  stock=20;  min=5  },
        @{ cat="Sunglasses";    sku="SUN-002"; name="Oakley Holbrook";           brand="Oakley";       model="Holbrook";   price=289.90;  stock=15;  min=5  },
        @{ cat="Equipment";     sku="EQ-001";  name="Frontofocometro Digital";   brand="OptiTech";     model="FD-200";     price=2500.00; stock=2;   min=1  }
    )
    $prodIds = @(); $prodPrices = @()
    for ($i = 0; $i -lt $productDefs.Count; $i++) {
        $p = $productDefs[$i]
        $si = $i % $supIds.Count
        $body = @{
            category = $p.cat; supplierId = $supIds[$si]; supplierName = $supNames[$si]
            sku = "$($p.sku)-$sfx"; name = $p.name; brand = $p.brand; model = $p.model
            price = $p.price; stock = $p.stock; minimumStockThreshold = $p.min
        }
        $r = Post "/products" $body $token
        if ($r) { $prodIds += $r.id; $prodPrices += $p.price }
    }
    Write-Host "    $($prodIds.Count) products" -ForegroundColor Green

    # 5b) Product mutations: update, absolute stock set, restock, adjustment, consume
    if ($prodIds.Count -gt 3) {
        Put   "/products/$($prodIds[0])" @{ name="Montura Ray-Ban RB5154 (2024)"; sku="MON-001-$sfx"; category="Frames"; price=369.90; minimumStockThreshold=5 } $token | Out-Null
        Patch "/products/$($prodIds[1])/stock" @{ newStock = 22 } $token | Out-Null
        Post  "/products/$($prodIds[4])/restock" @{ quantity = 30; author = "$($ctx.Name) - Almacen" } $token | Out-Null
        Post  "/products/$($prodIds[10])/adjustments" @{ newStock = 95; justification = "Ajuste tras inventario fisico mensual"; author = "$($ctx.Name) - Gerencia" } $token | Out-Null
        Post  "/products/$($prodIds[5])/consume" @{ quantity = 2; author = "$($ctx.Name) - Taller" } $token | Out-Null
    }
    Write-Host "    product update / stock / restock / adjustment / consume exercised" -ForegroundColor Green

    # 6) Patients (auto-create clinical records) + patient update
    Write-Host "  [patients + prescriptions]" -ForegroundColor DarkCyan
    $firstNames = @("Alejandro","Valentina","Diego","Lucia","Sebastian","Camila","Mateo","Isabela","Emilio","Sofia","Nicolas","Paula","Rodrigo","Daniela","Fernando","Gabriela","Andres","Ximena")
    $lastNames  = @("Garcia Lopez","Morales Cruz","Rodriguez Perez","Hernandez Vega","Vargas Quispe","Torres Mendoza","Flores Salas","Diaz Ramirez","Castro Huanca","Reyes Chavez","Ruiz Paredes","Guzman Ticona","Mendez Silva","Cordova Rojas","Aguilar Nunez","Salazar Pinto","Rojas Leon","Campos Diaz")
    $patIds = @(); $patNames = @()
    for ($i = 0; $i -lt $firstNames.Count; $i++) {
        $dni = "" + (10000000 + $accNum * 100 + ($i + 1))   # 8-digit numeric, unique per account
        $body = @{
            firstName = $firstNames[$i]; lastName = $lastNames[$i]
            dni = $dni; phone = ("+51 987 0{0:D2} 0{0:D2}" -f ($i + 1))
            email = ("{0}{1}.{2}@mail.com" -f $firstNames[$i].Substring(0,1).ToLower(), ($lastNames[$i] -replace '\s','').ToLower(), $sfx)
            birthDate = (Get-Date).AddYears(-(20 + $i * 3)).ToString("yyyy-MM-dd")
        }
        $r = Post "/api/v1/patients" $body $token
        if ($r) { $patIds += $r.id; $patNames += "$($firstNames[$i]) $($lastNames[$i])" }
    }
    # patient update
    if ($patIds.Count -gt 0) {
        Put "/api/v1/patients/$($patIds[0])" @{ firstName=$firstNames[0]; lastName=$lastNames[0]; dni=("" + (10000000 + $accNum * 100 + 1)); phone="+51 987 999 999"; email="updated.$sfx@mail.com"; birthDate="1988-03-14" } $token | Out-Null
    }
    # prescriptions on the auto-created clinical records
    $records = @(Get-All "/api/v1/clinical-records" $token | ForEach-Object { $_.id })
    $doctors = @("Dr. Eduardo Soto","Dra. Patricia Lima","Dra. Claudia Vera","Dr. Ricardo Fuentes")
    for ($i = 0; $i -lt [math]::Min($records.Count, 14); $i++) {
        $sphD = [math]::Round((Get-Random -Minimum -600 -Maximum 350) / 100.0, 2)
        $sphI = [math]::Round($sphD + (Get-Random -Minimum -50 -Maximum 50) / 100.0, 2)
        $body = @{
            clinicalRecordId = $records[$i]
            odSphere = $sphD; odCylinder = [math]::Round((Get-Random -Minimum -150 -Maximum 0)/100.0,2); odAxis = (Get-Random -Minimum 0 -Maximum 181)
            oiSphere = $sphI; oiCylinder = [math]::Round((Get-Random -Minimum -150 -Maximum 0)/100.0,2); oiAxis = (Get-Random -Minimum 0 -Maximum 181)
            addition = $(if ((Get-Random -Minimum 0 -Maximum 3) -eq 0) { [math]::Round((Get-Random -Minimum 100 -Maximum 300)/100.0,2) } else { $null })
            notes = "Control optometrico de rutina"; doctorName = (Pick $doctors)
        }
        Post "/api/v1/prescriptions" $body $token | Out-Null
    }
    Write-Host "    $($patIds.Count) patients, $([math]::Min($records.Count,14)) prescriptions" -ForegroundColor Green

    # 7) ~1 month of daily sales -> work orders -> status progression -> payments -> notifications
    Write-Host "  [sales / work-orders / payments over $Days days]" -ForegroundColor DarkCyan
    $pmethods   = @("CREDIT_CARD","DEBIT_CARD","TRANSFER","CASH")
    $lensTypes  = @("Monofocal","Bifocal","Progresiva","Anti-reflejo","Fotocromatico")
    $frames     = @("Metal slim negro","Acetato carey marron","Titanio sin aro","Metal dorado","Acetato azul marino")
    $priorities = @("normal","high","urgent")
    $saleN = 0; $paid = 0; $cancelled = 0; $discounted = 0; $quotas = 0; $wos = 0; $notifs = 0
    for ($d = $Days; $d -ge 0; $d--) {
        $todays = Get-Random -Minimum 0 -Maximum 3
        for ($k = 0; $k -lt $todays; $k++) {
            $saleN++
            $pi = Get-Random -Minimum 0 -Maximum $patIds.Count
            $prodIdx = Get-Random -Minimum 0 -Maximum $prodIds.Count
            $qty = Get-Random -Minimum 1 -Maximum 3
            $total = [math]::Round($prodPrices[$prodIdx] * $qty + (Get-Random -Minimum 50 -Maximum 300), 2)
            $advance = [math]::Round($total * ((Get-Random -Minimum 0 -Maximum 4) / 10.0), 2)
            $pm = Pick $pmethods
            $saleBody = @{
                invoiceNumber = ("FAC-$sfx-{0:D3}" -f $saleN); labOrderNumber = $null
                patientId = $patIds[$pi]; patientName = $patNames[$pi]
                userId = (Pick $staffIds); userName = (Pick @("Maria Reception","Jose Optometra","Ana Gerente","Luis Vendedor"))
                totalAmount = $total; advance = $advance; discountCode = $null; discountAmount = 0
                paymentMethod = $pm; createdAt = (Day $d); deliveredAt = $null; notes = $null
                items = @(@{ productId = $prodIds[$prodIdx]; quantity = $qty })
            }
            $sale = Post "/sales" $saleBody $token
            if (-not $sale) { continue }
            $saleId = $sale.id
            $curTotal = $total; $curPending = $total - $advance

            # a quota sale occasionally (advance >= 30%)
            if ((Get-Random -Minimum 1 -Maximum 101) -le 12) {
                $q = Post "/sales/$saleId/generate-quota" @{ advance = [math]::Round($total * 0.35, 2) } $token
                if ($q) { $quotas++ }
            }
            # ~18% get a promotional discount
            if ((Get-Random -Minimum 1 -Maximum 101) -le 18) {
                $disc = Post "/sales/$saleId/apply-discount" @{ discountCode = "PROMO10"; discountAmount = [math]::Round($total * 0.10, 2) } $token
                if ($disc) { $discounted++; $curTotal = $disc.totalAmount; if ($null -ne $disc.pendingBalance) { $curPending = $disc.pendingBalance } }
            }
            # ~10% of older sales get cancelled
            if ($d -le ($Days - 3) -and (Get-Random -Minimum 1 -Maximum 101) -le 10) {
                Post "/sales/$saleId/request-cancellation" $null $token | Out-Null
                $c = Post "/sales/$saleId/cancel" @{ labOrderStatus = "PENDING" } $token
                if ($c) { $cancelled++ }
                continue
            }
            # work order in a lab
            $li = Get-Random -Minimum 0 -Maximum $labIds.Count
            $woBody = @{
                saleId = $saleId; recipeId = 0; labId = $labIds[$li]
                patientName = $patNames[$pi]; laboratoryName = $labNames[$li]
                lensType = (Pick $lensTypes); lensProductId = $null
                frame = (Pick $frames); frameProductId = $null
                prescription = "Segun receta del paciente"; priority = (Pick $priorities)
                deliveryDate = (DayShort (-(Get-Random -Minimum 5 -Maximum 21))); deposit = $advance; total = $curTotal
            }
            $wo = Post "/work-orders" $woBody $token
            if (-not $wo) { continue }
            $wos++; $woId = $wo.id
            # relink the sale explicitly (covers the sale-link endpoint)
            Patch "/work-orders/$woId/sale" @{ saleId = $saleId } $token | Out-Null
            # progress the status based on age
            $flow = @()
            if     ($d -ge 14) { $flow = @("IN_PRODUCTION","QUALITY_CONTROL","READY","DELIVERED") }
            elseif ($d -ge 8)  { $flow = @("IN_PRODUCTION","QUALITY_CONTROL") }
            elseif ($d -ge 3)  { $flow = @("IN_PRODUCTION") }
            $ready = $false
            foreach ($st in $flow) { Patch "/work-orders/$woId/status" @{ status = $st } $token | Out-Null; if ($st -eq "READY" -or $st -eq "DELIVERED") { $ready = $true } }
            # pay off when ready, then notify the patient
            if ($ready -and $curPending -gt 0) {
                $first = [math]::Round($curPending * 0.6, 2); $rest = [math]::Round($curPending - $first, 2)
                Post "/payments/$saleId/pay" @{ amountPaid = $first; method = $pm } $token | Out-Null
                if ($rest -gt 0) { Post "/payments/$saleId/pay" @{ amountPaid = $rest; method = $pm } $token | Out-Null }
                $paid++
                $notif = Post "/api/v1/patients/$($patIds[$pi])/notifications" @{ workOrderId = $woId; message = "Sus lentes estan listos para recoger en tienda." } $token
                if ($notif) {
                    $notifs++
                    if ($notif.id) { Patch "/api/v1/patients/$($patIds[$pi])/notifications/$($notif.id)/read" $null $token | Out-Null }
                }
            }
        }
    }
    Write-Host "    $saleN sales ($cancelled cancelled, $discounted discounted, $quotas quotas, $paid paid off), $wos work orders, $notifs notifications" -ForegroundColor Green

    # 8) Subscription follow-ups (non-destructive on the active subscription)
    if ($subId) {
        Post  "/api/v1/subscription-payments/subscriptions/$subId" @{ amount = $planAmounts[1]; paymentMethod = "CREDIT_CARD" } $token | Out-Null
        Post  "/api/v1/billing/subscriptions/$subId/check-renewal" $null $token | Out-Null
        Post  "/api/v1/billing/subscriptions/$subId/auto-renew" $null $token | Out-Null
        Patch "/api/v1/subscriptions/$subId/plan" @{ newPlanId = $planIds[2]; newTier = $planTiers[2]; amount = $planAmounts[2]; paymentMethod = "CREDIT_CARD" } $token | Out-Null
        Write-Host "    subscription payment / billing renewal / auto-renew / plan-change exercised" -ForegroundColor Green
    }
    # (System notifications use an int recipient id and are emitted internally by event handlers,
    # so they are not seedable via the public API; patient notifications above cover the notify flow.)
}

function Seed-Lifecycle($planIds, $planTiers, $planAmounts) {
    # Throwaway account to cover destructive subscription + IAM endpoints without touching the data accounts.
    Write-Host ""
    Write-Host "=== Lifecycle / IAM edge endpoints (throwaway account) ===" -ForegroundColor Cyan
    $email = "lifecycle.$Tag@optiflow.seed"
    $ctx = New-Account $email $Password "Lifecycle Demo"
    # Subscription lifecycle: activate -> change plan -> cancel; then a second one -> expire
    $s1 = Enable-Subscription $ctx $planIds[0] $planTiers[0] $planAmounts[0]
    if ($s1) {
        Patch "/api/v1/subscriptions/$s1/plan" @{ newPlanId = $planIds[1]; newTier = $planTiers[1]; amount = $planAmounts[1]; paymentMethod = "CREDIT_CARD" } $ctx.Token | Out-Null
        Post  "/api/v1/subscriptions/$s1/cancel" $null $ctx.Token | Out-Null
    }
    $s2 = Enable-Subscription $ctx $planIds[2] $planTiers[2] $planAmounts[2]
    if ($s2) { Post "/api/v1/subscriptions/$s2/expire" $null $ctx.Token | Out-Null }
    # IAM: password change keeps the token valid; the email change invalidates it (JWT subject is the
    # email), so do it LAST with nothing after it.
    Put  "/api/v1/users/$($ctx.UserId)/password" @{ currentPassword = $Password; newPassword = ($Password + "X") } $ctx.Token | Out-Null
    Post "/api/v1/authentication/password-recoveries" @{ email = $email } $null | Out-Null
    Put  "/api/v1/users/$($ctx.UserId)/email" @{ newEmail = "lifecycle.$Tag.upd@optiflow.seed" } $ctx.Token | Out-Null
    Write-Host "    subscription change/cancel/expire + IAM password/recovery/email exercised" -ForegroundColor Green
}

# ============================================================================
Write-Host ""
Write-Host "=== OptiFlow comprehensive seed -> $BASE  ($(Get-Date -Format 'yyyy-MM-dd HH:mm')) ===" -ForegroundColor Cyan
Write-Host "  ~$Days days of history, tag '$Tag'"

$accA = New-Account "miranda.$Tag@optiflow.seed" $Password "Optica Miranda"
$accB = New-Account "andina.$Tag@optiflow.seed"  $Password "Optica Andina"

# Shared plan catalog (created once)
Write-Host ""
Write-Host "=== Subscription plans (shared catalog) ===" -ForegroundColor Cyan
$plansData = @(
    @{ name="Plan Basico Mensual $Tag";        tier="BASiC";        price=49.90;  description="Hasta 2 usuarios, funciones esenciales" },
    @{ name="Plan Professional Mensual $Tag";  tier="PROFESSIONAL"; price=99.90;  description="Hasta 5 usuarios, reportes incluidos" },
    @{ name="Plan Enterprise Mensual $Tag";    tier="ENTERPRISE";   price=179.90; description="Usuarios ilimitados, soporte prioritario" },
    @{ name="Plan Professional Anual $Tag";    tier="PROFESSIONAL"; price=999.00; description="Plan profesional con descuento anual" }
)
$planIds = @(); $planTiers = @(); $planAmounts = @()
foreach ($p in $plansData) {
    $r = Post "/api/v1/plans" $p $accA.Token
    if ($r) { $planIds += $r.id; $planTiers += $p.tier; $planAmounts += $p.price; Write-Host "  plan: $($p.name) -> id $($r.id)" }
}
if ($planIds.Count -lt 3) { throw "Could not create plans; aborting." }

Write-Host ""
Write-Host "=== Seeding Optica Miranda ===" -ForegroundColor Cyan
Seed-Account $accA $planIds $planTiers $planAmounts
Write-Host ""
Write-Host "=== Seeding Optica Andina ===" -ForegroundColor Cyan
Seed-Account $accB $planIds $planTiers $planAmounts

Seed-Lifecycle $planIds $planTiers $planAmounts

Write-Host ""
Write-Host "=== Seed complete ===" -ForegroundColor Green
Write-Host "  API calls: $($script:calls)   failures: $($script:fails)"
Write-Host "  Account A: $($accA.Email)  ($($accA.Name))"
Write-Host "  Account B: $($accB.Email)  ($($accB.Name))"
Write-Host "  Password (all): $Password"
Write-Host "  Sign in as either to browse a lived-in, ~1-month dataset."
Write-Host ""
