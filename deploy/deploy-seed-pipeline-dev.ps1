Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)
Import-Module -Name ./power-shell/modules/Get-AuthHeader -Force
Import-Module -Name ./power-shell/modules/Add-Department -Force
Import-Module -Name ./power-shell/modules/Add-Post -Force
Import-Module -Name ./power-shell/modules/Add-Role -Force
Import-Module -Name ./power-shell/modules/Add-RoleToPost -Force
Import-Module -Name ./power-shell/modules/Add-Function -Force
Import-Module -Name ./power-shell/modules/Add-RoleToFunction -Force
Import-Module -Name ./power-shell/modules/Add-Template -Force

./deploy-template.ps1

