function Import-Post() {
    param(
        [String] $FileFullPath
    )

    $authheader = Get-AuthHeader
    $authheader["Content-type"] = "text/csv"
    
    $endPoint = "http://localhost:3500/v1.0/invoke/ServiceBus/method/api/import/post"
    
    $fileBytes = [System.IO.File]::ReadAllBytes($FileFullPath);
    $fileEnc = [System.Text.Encoding]::GetEncoding('UTF-8').GetString($fileBytes);

    $body = $fileEnc

    # Write-Host $body -ForegroundColor Green  
    $bodyJsonBytes = [System.Text.Encoding]::UTF8.GetBytes($body)

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

Export-ModuleMember -Function Import-Post
