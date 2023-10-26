Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)

Import-Module -Name ../power-shell/modules/Get-AuthHeader -Force
Import-Module -Name ../power-shell/modules/Add-Template -Force

Write-Host "----- Add-Template process start -----" -ForegroundColor Blue
$location = Get-Location
Add-Template -FileFullPath $location/templates/啟用申請.html
Add-Template -FileFullPath $location/templates/復用申請.html
Add-Template -FileFullPath $location/templates/過戶申請.html
Add-Template -FileFullPath $location/templates/廢止申請.html
Add-Template -FileFullPath $location/templates/停用申請.html
Add-Template -FileFullPath $location/templates/軍眷優待申請.html
Add-Template -FileFullPath $location/templates/用水種類變更申請.html
Write-Host "----- Add-Template process end -----" -ForegroundColor Blue