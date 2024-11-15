Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)

Write-Output Api_Gateway_EndPoint: $env:Api_Gateway_EndPoint
Write-Output OpenIDConnectOptions_Authority: $env:OpenIDConnectOptions_Authority
Write-Output SignalR_EndPoint: $env:SignalR_EndPoint

$env:MINIO_ROOT_USER = "admin"
$env:MINIO_ROOT_PASSWORD = "adminadmin"

$env:LDAP_ORGANISATION = "Example Inc."
$env:LDAP_DOMAIN = "example.org"
$env:LDAP_BASEDN = "dc=example,dc=org"
$env:LDAP_ADMIN_PASSWORD = "adminadmin"

$env:MONGO_INITDB_ROOT_USERNAME = "admin"
$env:MONGO_INITDB_ROOT_PASSWORD = "adminadmin"

$env:KONG_POSTGRES_USER = "admin"
$env:KONG_POSTGRES_DB = "kong"
$env:KONG_POSTGRES_PASSWORD = "adminadmin"

$env:MetadataApi_Version = "1.1.6"
$env:MultiMediaApi_Version = "0.0.8"
$env:JwtAuthApi_Version = "0.5.5"
$env:OpenidProvider_Version = "0.5.5"
$env:ResourceApi_Version = "0.2.1"
$env:ServiceBus_Version = "0.0.9"

$env:TwcWeb_Version = "1.8.3"
$env:TwcReport_Version = "0.2.5"

docker compose -f docker-compose.yml -f docker-compose.metadataapi.yml -f docker-compose.e2e.yml up -d

function WaitForHealthy {
    param (
        [string]$containerName,
        [int]$maxRetries = 10
    )

    $retries = 0

    while ($retries -lt $maxRetries) {
        $healthStatus = (docker inspect --format="{{.State.Health.Status}}" $containerName)

        if ($healthStatus -eq "healthy") {
            Write-Host "Container $containerName is healthy"
            break
        }

        Write-Host "Waiting for container $containerName to be healthy..."
        $retries++
        Start-Sleep -Seconds 5
    }

    if ($retries -eq $maxRetries) {
        
		docker logs $containerName
        docker compose stop $containerName

        throw "Container $containerName did not become healthy within the specified time"
    }
}

WaitForHealthy "sqlserver"
WaitForHealthy "kong"
WaitForHealthy "elasticsearch"
WaitForHealthy "openidconnect.com.tw"
WaitForHealthy "metadataapi"
WaitForHealthy "multimediaapi"
WaitForHealthy "resourceapi"
WaitForHealthy "twcweb"
WaitForHealthy "servicebus"
WaitForHealthy "twcreport"  
