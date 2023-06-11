function Add-RoleToPost() {
    param(
        [String] $RoleId,
        [String] $PostId
    )

    #. .\modules\Get-AuthHeader.ps1

    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/JwtAuthApi/method/api/role/InsertPost"
    
    $body = @{
        roleId = $RoleId
        postId = $PostId
    }

    Write-Host $($body | ConvertTo-Json) -ForegroundColor Green
    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json))
    
    try {
        Invoke-RestMethod $endPoint -Method Patch -Headers $authheader -Body $bodyJsonBytes
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

Export-ModuleMember -Function Add-RoleToPost