# Update VNPay ReturnUrl with ngrok URL
# Usage: .\update-vnpay-url.ps1 "https://abc123.ngrok.io"

param(
    [Parameter(Mandatory=$true)]
    [string]$NgrokUrl
)

# Remove trailing slash if exists
$NgrokUrl = $NgrokUrl.TrimEnd('/')

# Validate URL format
if (-not ($NgrokUrl -match '^https?://')) {
    Write-Host "❌ Error: URL must start with http:// or https://" -ForegroundColor Red
    exit 1
}

$appsettingsPath = "EV Rental\appsettings.json"

if (-not (Test-Path $appsettingsPath)) {
    Write-Host "❌ Error: appsettings.json not found at $appsettingsPath" -ForegroundColor Red
    exit 1
}

Write-Host "📝 Updating VNPay ReturnUrl..." -ForegroundColor Yellow

# Read the file
$content = Get-Content $appsettingsPath -Raw

# Create the new ReturnUrl
$newReturnUrl = "$NgrokUrl/Payment/VNPayReturn"

# Replace the ReturnUrl
$pattern = '"ReturnUrl":\s*"[^"]*"'
$replacement = "`"ReturnUrl`": `"$newReturnUrl`""
$newContent = $content -replace $pattern, $replacement

# Write back to file
$newContent | Set-Content $appsettingsPath -NoNewline

Write-Host "✅ VNPay ReturnUrl updated successfully!" -ForegroundColor Green
Write-Host "   New ReturnUrl: $newReturnUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "🚀 Next steps:" -ForegroundColor Yellow
Write-Host "   1. Make sure ngrok is running: ngrok http 5126"
Write-Host "   2. Run the application: dotnet run --project `"EV Rental/PresentaionLayer.csproj`""
Write-Host "   3. Access via ngrok URL: $NgrokUrl"
