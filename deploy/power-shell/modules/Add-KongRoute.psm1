function Add-KongRoute() {
    param(
        [String] $Name,
        [String[]] $Paths,
        [String] $ServiceName = $Name,
        [bool] $PreserveHost = $false
    )

    $endPoint = "http://localhost:8001/services/$ServiceName/routes"
    
    $body = @{
        name = $Name
        paths = $Paths
        preserve_host = $PreserveHost
    }
    
    Write-Host $($body | ConvertTo-Json) -ForegroundColor Green
    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json))
    
    try {
        Invoke-RestMethod $endPoint -Method POST -Body $bodyJsonBytes -ContentType "application/json"
    }
    catch {
        if($_.ErrorDetails.Message) {
            Write-Host $_.ErrorDetails.Message -ForegroundColor Red
        } else {
            Write-Host $_ -ForegroundColor Red
        }

       # throw
    }
}

Export-ModuleMember -Function Add-KongRoute
