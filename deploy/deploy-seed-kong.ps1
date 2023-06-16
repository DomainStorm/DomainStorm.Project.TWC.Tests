Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)

Import-Module -Name ./power-shell/modules/Add-KongService -Force
Import-Module -Name ./power-shell/modules/Add-KongRoute -Force

Add-KongService -Name "twcweb" -Url "https://twcweb"
Add-KongRoute -Name "twcweb" -Paths @("/") -PreserveHost $true
Add-KongService -Name "jwtauthapi" -Url "http://jwtauthapi"
Add-KongRoute -Name "jwtauthapi" -Paths @("/jwtauthapi")
Add-KongService -Name "metadataapi" -Url "http://metadataapi"
Add-KongRoute -Name "metadataapi" -Paths @("/metadataapi")
Add-KongService -Name "multimediaapi" -Url "http://multimediaapi"
Add-KongRoute -Name "multimediaapi" -Paths @("/multimediaapi")
Add-KongService -Name "resourceapi" -Url "http://resourceapi"
Add-KongRoute -Name "resourceapi" -Paths @("/resourceapi")
Add-KongService -Name "servicebus" -Url "http://servicebus"
Add-KongRoute -Name "servicebus" -Paths @("/servicebus")
Add-KongService -Name "openidconnect.com.tw" -Url "http://openidconnect.com.tw:5050/openid"
Add-KongRoute -Name "openidconnect.com.tw" -Paths @("/openid") -PreserveHost $true