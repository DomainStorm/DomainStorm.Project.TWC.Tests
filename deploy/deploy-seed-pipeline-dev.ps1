Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)
Import-Module -Name ./power-shell/modules/Get-AuthHeader -Force
Import-Module -Name ./power-shell/modules/Add-Department -Force
Import-Module -Name ./power-shell/modules/Add-Post -Force
Import-Module -Name ./power-shell/modules/Add-Role -Force
Import-Module -Name ./power-shell/modules/Add-RoleToPost -Force
Import-Module -Name ./power-shell/modules/Add-Function -Force
Import-Module -Name ./power-shell/modules/Add-RoleToFunction -Force
Import-Module -Name ./power-shell/modules/Add-Template -Force

./deploy-seed-kong.ps1

./deploy-template.ps1

./deploy-import-department.ps1
./deploy-import-user.ps1
./deploy-import-post.ps1
./deploy-import-post-role.ps1

Import-Module -Name ./power-shell/modules/Import-Department -Force
Import-Module -Name ./power-shell/modules/Import-User -Force
Import-Module -Name ./power-shell/modules/Import-Post -Force
Import-Module -Name ./power-shell/modules/Import-PostRole -Force

Write-Host "----- Import-Department process start -----" -ForegroundColor Blue
Import-Department -FileFullPath ./import/department_dev.csv
Write-Host "----- Import-Department process end -----" -ForegroundColor Blue

Write-Host "----- Import-User process start -----" -ForegroundColor Blue
Import-User -FileFullPath ./import/user_dev.csv
Write-Host "----- Import-User process end -----" -ForegroundColor Blue

Write-Host "----- Import-Post process start -----" -ForegroundColor Blue
Import-Post -FileFullPath ./import/post_dev.csv
Write-Host "----- Import-Post process end -----" -ForegroundColor Blue

Write-Host "----- Import-PostRole process start -----" -ForegroundColor Blue
Import-PostRole -FileFullPath ./import/postrole_dev.csv
Write-Host "----- Import-PostRole process end -----" -ForegroundColor Blue