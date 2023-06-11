function Add-KongService() {
    param(
        [String] $Name,
        [String] $Url
    )

    $endPoint = "http://localhost:8001/services"
    
    $body = @{
        name = $Name
        url = $Url
    }
    
    Write-Host $($body | ConvertTo-Json) -ForegroundColor Green
    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json))
    
    try {
        $tokenResponse = Invoke-RestMethod $endPoint -Method POST -Body $bodyJsonBytes -ContentType "application/json"
        return $tokenResponse.id
    }
    catch {
        if($_.ErrorDetails.Message) {
            Write-Host $_.ErrorDetails.Message -ForegroundColor Red
        } else {
            Write-Host $_ -ForegroundColor Red
        }

        throw
    }
}

Export-ModuleMember -Function Add-KongService
