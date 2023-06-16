function Add-Template() {
    param(
        [String] $FileFullPath
    )
    $authheader = Get-AuthHeader
    
    $endPoint = "http://localhost:3500/v1.0/invoke/ResourceApi/method/api/template"

    # $multipartContent = [System.Net.Http.MultipartFormDataContent]::new()
    # $multipartFile = $FileFullPath
    # $FileStream = [System.IO.FileStream]::new($multipartFile, [System.IO.FileMode]::Open)
    # $fileHeader = [System.Net.Http.Headers.ContentDispositionHeaderValue]::new("form-data")
    # $fileHeader.Name = "file"
    # $fileHeader.FileName = $FileFullPath
    # $fileContent = [System.Net.Http.StreamContent]::new($FileStream)
    # $fileContent.Headers.ContentDisposition = $fileHeader
    # $multipartContent.Add($fileContent)

    $fileBytes = [System.IO.File]::ReadAllBytes($FileFullPath);
    $fileEnc = [System.Text.Encoding]::GetEncoding('UTF-8').GetString($fileBytes);
    $boundary = [System.Guid]::NewGuid().ToString(); 
    $LF = "`r`n";
    
    $body = ( 
        "--$boundary",
        "Content-Disposition: form-data; name=`"file`"; filename=`"temp.html`"",
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

Export-ModuleMember -Function Add-Template
