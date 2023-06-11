function Add-RoleToFunction() {
    param(
        [String] $FunctionId,
        [String] $RoleId
    )

    #. .\modules\Get-AuthHeader.ps1

    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/ResourceApi/method/api/function/InsertRole"
    
    $body = @{
        functionId = $FunctionId
        roleId = $RoleId
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

Export-ModuleMember -Function Add-RoleToFunction