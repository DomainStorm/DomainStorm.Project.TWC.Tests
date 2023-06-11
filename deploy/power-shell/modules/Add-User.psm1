function Add-User() {
    param(
        [String] $UserId
    )

    #. .\modules\Get-AuthHeader.ps1

    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/JwtAuthApi/method/api/user"
    
    $body = @{
        userId = $UserId
        code = "sti"
        firstName = "sti"
        lastName = "sti"
        account = "sti"
        password = "passw0rd"
        email = "sti@gmail.com"
        telephoneNumber = "1234567890"
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

Export-ModuleMember -Function Add-User

