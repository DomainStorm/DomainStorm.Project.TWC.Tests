function Add-Function() {
    param(
        [String] $FunctionId,
        [String] $FunctionName,
        [bool] $Always,
        [String] $ParentFunctionId,
        [int16] $Sort,
        [String] $Href,
        [String] $Icon,
        [int] $Catagory
    )

    #. .\modules\Get-AuthHeader.ps1

    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/ResourceApi/method/api/function"
    
    $body = @{
        functionId = $FunctionId
        name = $FunctionName
        always = $Always
        sort = $Sort
        href = $Href
        icon = $Icon
        catagory = $Catagory
    }

    if($ParentFunctionId -ne ""){
        $body.Add("parentFunctionId", $ParentFunctionId);
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

Export-ModuleMember -Function Add-Function