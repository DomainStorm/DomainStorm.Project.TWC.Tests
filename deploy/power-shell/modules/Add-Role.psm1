function Add-Role() {
    param(
        [String] $RoleId,
        [String] $RoleName
    )

    #. .\modules\Get-AuthHeader.ps1

    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/JwtAuthApi/method/api/role"
    
    $body = @{
        roleId = $RoleId
        name = $RoleName
    }

    Write-Host $($body | ConvertTo-Json) -ForegroundColor Green
    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json))

    try {
        Invoke-RestMethod $endPoint -Method POST -Headers $authheader -Body $bodyJsonBytes
    }
    catch {
        if($_.ErrorDetails.Message) {
            Write-Host $_.ErrorDetails.Message -ForegroundColor Red
        } else {
            Write-Host $_ -ForegroundColor Red
        }
    }
}

Export-ModuleMember -Function Add-Role