Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)
Import-Module -Name ./power-shell/modules/Get-AuthHeader -Force
Import-Module -Name ./power-shell/modules/Add-Department -Force
Import-Module -Name ./power-shell/modules/Add-Post -Force
Import-Module -Name ./power-shell/modules/Add-Role -Force
Import-Module -Name ./power-shell/modules/Add-RoleToPost -Force
Import-Module -Name ./power-shell/modules/Add-Function -Force
Import-Module -Name ./power-shell/modules/Add-RoleToFunction -Force
Import-Module -Name ./power-shell/modules/Add-Template -Force

$userId = "efecd661-04d8-496c-b84d-c216257acd0f"
$departmentId = "83107101-2b0a-49e8-9600-2caf2abbcc50"
$departmentId2 = "d0b8eb62-463e-4817-bc70-7fcc541e91be"
$roleId = "754baad1-5efd-4e69-b048-838ec9109325" #系統管理員
$roleId2 = "49622071-d50c-4e0c-8892-4052d914d0ae" #臨櫃人員
$roleId3 = "a0a2a8d6-3c2d-4ed4-ac69-ab02586eeb52" #主管
$roleId4 = "bdd4f57d-0f16-4b63-81ed-aad29f27b0d1" #歸檔人員
$roleId5 = "b730b9a1-e2db-4dd2-9b1d-0dbdc5b7fa65" #調閱審核
$roleId6 = "f102d678-59f6-4630-aa2c-20a8aa51c9da" #稽催
$roleId7 = "2918e0d7-fd17-41e1-ac1c-b9f042053b42" #問卷管理員
$roleId8 = "7e491632-4b04-443b-bb58-14e2186fcf83" #節目單管理員

$postId = "09e844ab-0525-4ef5-baef-71fe7946fc16"
$postId2 = "f08fd8b6-2472-4e93-9dda-22dacbab136b"

./deploy-seed-kong.ps1

Write-Host "----- Add-Department process start -----" -ForegroundColor Blue
Add-Department -DepartmentId $departmentId -DepartmentCode "D001" -DepartmentName "敦陽" 
Add-Department -DepartmentId $departmentId2 -DepartmentCode "D002" -DepartmentName "資訊部" -ParentDepartmentId $departmentId
Write-Host "----- Add-Department process end -----" -ForegroundColor Blue

Write-Host "----- Add-Post process start -----" -ForegroundColor Blue
Add-Post -PostId $postId -Title "系統分析師" -DepartmentId $departmentId -UserId $userId
Add-Post -PostId $postId2 -Title "小小承辦人" -DepartmentId $departmentId2 -UserId $userId
Write-Host "----- Add-Post process end -----" -ForegroundColor Blue

Write-Host "----- Add-Role-To-Post process start -----" -ForegroundColor Blue
Add-RoleToPost -RoleId $roleId -PostId $postId
Add-RoleToPost -RoleId $roleId2 -PostId $postId
Add-RoleToPost -RoleId $roleId3 -PostId $postId
Add-RoleToPost -RoleId $roleId4 -PostId $postId
Add-RoleToPost -RoleId $roleId5 -PostId $postId
Add-RoleToPost -RoleId $roleId6 -PostId $postId
Add-RoleToPost -RoleId $roleId7 -PostId $postId
Add-RoleToPost -RoleId $roleId8 -PostId $postId
Add-RoleToPost -RoleId $roleId2 -PostId $postId2
Write-Host "----- Add-Role-To-Post process end -----" -ForegroundColor Blue

./deploy-template.ps1

# ./deploy-import-department.ps1
# ./deploy-import-user.ps1
# ./deploy-import-post.ps1
# ./deploy-import-post-role.ps1

$location = Get-Location

Import-Module -Name ./power-shell/modules/Import-Department -Force
Import-Module -Name ./power-shell/modules/Import-User -Force
Import-Module -Name ./power-shell/modules/Import-Post -Force
Import-Module -Name ./power-shell/modules/Import-PostRole -Force

Write-Host "----- Import-Department process start -----" -ForegroundColor Blue
Import-Department -FileFullPath $location/import/department_dev.csv
Write-Host "----- Import-Department process end -----" -ForegroundColor Blue

Write-Host "----- Import-User process start -----" -ForegroundColor Blue
Import-User -FileFullPath $location/import/user_dev.csv
Write-Host "----- Import-User process end -----" -ForegroundColor Blue

Write-Host "----- Import-Post process start -----" -ForegroundColor Blue
Import-Post -FileFullPath $location/import/post_dev.csv
Write-Host "----- Import-Post process end -----" -ForegroundColor Blue

Write-Host "----- Import-PostRole process start -----" -ForegroundColor Blue
Import-PostRole -FileFullPath $location/import/postrole_dev.csv
Write-Host "----- Import-PostRole process end -----" -ForegroundColor Blue