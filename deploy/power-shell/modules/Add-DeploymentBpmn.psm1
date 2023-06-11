function Add-DeploymentBpmn() {
    param(
        [String] $FileFullPath,
        [String] $DeploymentName
    )
    
    $endPoint = "http://localhost:8080/rest/deployment/create"
    $fileBytes = [System.IO.File]::ReadAllBytes($FileFullPath);
    $fileEnc = [System.Text.Encoding]::GetEncoding('UTF-8').GetString($fileBytes);
    $boundary = [System.Guid]::NewGuid().ToString(); 
    $LF = "`r`n";
    
    $body = ( 
        "--$boundary",
        "Content-Disposition: form-data; name=`"deployment-name`"$LF",
        "$DeploymentName",
        "--$boundary",
        "Content-Disposition: form-data; name=`"deploy-changed-only`"$LF",
        "true",    
        "--$boundary",    
        "Content-Disposition: form-data; name=`"*`"; filename=`"$DeploymentName.bpmn`"",
        "Content-Type: application/octet-stream$LF",
        $fileEnc,
        "--$boundary--$LF" 
    ) -join $LF

    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes($body)

    Write-Host $FileFullPath -ForegroundColor Green

    try {
        Invoke-RestMethod $endPoint -Method POST -Headers $authheader -ContentType "multipart/form-data; boundary=`"$boundary`"" -Body $bodyJsonBytes
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

Export-ModuleMember -Function Add-DeploymentBpmn

