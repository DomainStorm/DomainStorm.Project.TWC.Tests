function Test-KongService() {
    param(
        [String] $Name
    )

    $endPoint = "http://localhost:8001/services/$Name"
    
    try {
        Invoke-RestMethod $endPoint -Method GET -ContentType "application/json"
        return $true
    }
    catch {
        return $false
    }
}

Export-ModuleMember -Function Test-KongService
