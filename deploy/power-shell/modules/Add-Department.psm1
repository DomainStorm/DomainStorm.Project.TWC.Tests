function Add-Department() {
    param(
        [String] $DepartmentId,
        [String] $DepartmentCode,
        [String] $DepartmentName,
        [String] $ParentDepartmentId = $null
    )

    #. .\modules\Get-AuthHeader.ps1

    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/JwtAuthApi/method/api/department"
    
    $body = @{
        departmentId = $DepartmentId
        code = $DepartmentCode
        name = $DepartmentName
    }

    if($ParentDepartmentId -ne ""){
        $body.Add("parentDepartmentId", $ParentDepartmentId);
    }

    Write-Host $($body | ConvertTo-Json) -ForegroundColor Green  
    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json))

    try {
        Invoke-RestMethod $endPoint -Method POST -Headers $authheader -Body $bodyJsonBytes
    }
    catch {
        # $result = $_.Exception.Response.GetResponseStream()
        # $reader = New-Object System.IO.StreamReader($result)
        # $reader.BaseStream.Position = 0
        # $reader.DiscardBufferedData()

        # Write-Host $reader.ReadToEnd() -ForegroundColor Red
        # $responseBody = $reader.ReadToEnd() | ConvertFrom-Json | Format-List | Out-String;
        # Write-Host ($_.Exception.Message) -ForegroundColor Red
        # Write-Host $responseBody -ForegroundColor Red

        if($_.ErrorDetails.Message) {
            Write-Host $_.ErrorDetails.Message -ForegroundColor Red
        } else {
            Write-Host $_ -ForegroundColor Red
        }
    }  
}

Export-ModuleMember -Function Add-Department
