Set-Location (Split-Path -Parent $MyInvocation.MyCommand.Path)

$env:Api_Gateway_EndPoint = "https://domain-storm:8443"
$env:OpenIDConnectOptions_Authority = "http://domain-storm:8000/openid"
$env:SignalR_EndPoint = "http://domain-storm:8000/hubUrl"

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