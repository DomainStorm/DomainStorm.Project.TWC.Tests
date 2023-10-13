Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)

Import-Module -Name ../../power-shell/modules/Add-KongService -Force
Import-Module -Name ../../power-shell/modules/Add-KongRoute -Force
Import-Module -Name ../../power-shell/modules/Test-KongService -Force

if((Test-KongService -Name "twcweb") -eq $false)
{
    Add-KongService -Name "twcweb" -Url "https://twcweb"
    Add-KongRoute -Name "twcweb" -Paths @("/") -PreserveHost $true
}

if((Test-KongService -Name "jwtauthapi") -eq $false)
{
    Add-KongService -Name "jwtauthapi" -Url "http://jwtauthapi"
    Add-KongRoute -Name "jwtauthapi" -Paths @("/jwtauthapi")
}

if((Test-KongService -Name "metadataapi") -eq $false)
{
    Add-KongService -Name "metadataapi" -Url "http://metadataapi"
    Add-KongRoute -Name "metadataapi" -Paths @("/metadataapi")
}

if((Test-KongService -Name "multimediaapi")-eq $false)
{
    Add-KongService -Name "multimediaapi" -Url "http://multimediaapi"
    Add-KongRoute -Name "multimediaapi" -Paths @("/multimediaapi")
}

if((Test-KongService -Name "resourceapi") -eq $false)
{
    Add-KongService -Name "resourceapi" -Url "http://resourceapi"
    Add-KongRoute -Name "resourceapi" -Paths @("/resourceapi")
}

if((Test-KongService -Name "servicebus") -eq $false)
{
    Add-KongService -Name "servicebus" -Url "http://servicebus"
    Add-KongRoute -Name "servicebus" -Paths @("/servicebus")
}

if((Test-KongService -Name "openidconnect.com.tw") -eq $false)
{
    Add-KongService -Name "openidconnect.com.tw" -Url "http://openidconnect.com.tw:5050/openid"
    Add-KongRoute -Name "openidconnect.com.tw" -Paths @("/openid") -PreserveHost $true
}

if((Test-KongService -Name "twcreport") -eq $false)
{
    Add-KongService -Name "twcreport" -Url "http://twcreport"
    Add-KongRoute -Name "twcreport" -Paths @("/twcreport") -PreserveHost $true
}
