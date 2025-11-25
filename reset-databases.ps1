Write-Host "=== Resetting All Databases ===" -ForegroundColor Cyan

# Stop any running dotnet processes
Write-Host "`nStopping any running services..." -ForegroundColor Yellow
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

$services = @(
    @{Name="AuthService"; Path=".\CMS.AuthService"; DbPattern="auth.db*"},
    @{Name="TenantService"; Path=".\CMS.TenantService"; DbPattern="tenants.db*"},
    @{Name="UserService"; Path=".\CMS.UserService"; DbPattern="users.db*"},
    @{Name="ComplaintService"; Path=".\CMS.ComplaintService"; DbPattern="complaints.db*"}
)

foreach ($service in $services) {
    Write-Host "`n--- Processing $($service.Name) ---" -ForegroundColor Green
    
    Set-Location $service.Path
    
    # Remove database files
    Write-Host "Removing database files..." -ForegroundColor Yellow
    Remove-Item $service.DbPattern -Force -ErrorAction SilentlyContinue
    
    # Remove migrations
    Write-Host "Removing old migrations..." -ForegroundColor Yellow
    Remove-Item .\Migrations\* -Recurse -Force -ErrorAction SilentlyContinue
    
    # Create new migration
    Write-Host "Creating new InitialCreate migration..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to create migration for $($service.Name)" -ForegroundColor Red
        Set-Location ..
        continue
    }
    
    # Update database
    Write-Host "Applying migration to database..." -ForegroundColor Yellow
    dotnet ef database update
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to update database for $($service.Name)" -ForegroundColor Red
    } else {
        Write-Host "$($service.Name) completed successfully!" -ForegroundColor Green
    }
    
    Set-Location ..
}

Write-Host "`n=== Database Reset Complete ===" -ForegroundColor Cyan
Write-Host "You can now start your services." -ForegroundColor Green
