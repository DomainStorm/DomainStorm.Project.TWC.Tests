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

$env:MetadataApi_Version = "0.0.8"
$env:MultiMediaApi_Version = "0.0.5"
$env:JwtAuthApi_Version = "0.2.8"
$env:OpenidProvider_Version = "0.2.8"
$env:TwcWeb_Version = "0.7.5"
$env:ResourceApi_Version = "0.1.1"
$env:ServiceBus_Version = "0.0.4"

docker compose -f docker-compose.yml -f docker-compose.metadataapi.yml -f docker-compose.dev.yml up -d

$containers = @("multimediaapi", "metadataapi", "jwtauthapi", "openidprovider", "twcweb", "resourceapi", "servicebus")

while ($true) {
    $current_time = Get-Date
    $elapsed_time = ($current_time - $start_time).TotalSeconds().ToString()

    if ($elapsed_time -ge $timeout) {
        Write-Host "Container health check timeout"
        exit 1
    }

    $all_containers_healthy = $true

    foreach ($container in $containers) {
        $container_health = docker inspect --format='{{json .State.Health.Status}}' $container

        if ($container_health -ne "healthy") {
            $all_containers_healthy = $false
            Write-Host "Container $container is not healthy"
            break
        }
    }

    if ($all_containers_healthy) {
        Write-Host "All containers are healthy"
        break
    }

    Write-Host "Waiting for containers to become healthy..."
    Start-Sleep -Seconds 5
}

./deploy-seed-pipeline-dev.ps1