function Get-AuthHeader {
    $clientId = "sti"
    $clientSecret = "TynU8DW6"
    $scope = "profile openid user_write user_read department_write department_read function_write function_read template_write template_read mediaFile_write mediaFile_read dublinCore_write dublinCore_read post_write post_read role_write role_read"

    $reqAuthHeader = @{
        "Authorization" = "Basic $([Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("$($clientId):$($clientSecret)")))" 
    }   

    $reqTokenBody = @{
        grant_type = "client_credentials"
        scope = $scope
    }
    
    $authUri = "http://localhost:5050/connect/token"
    
    $tokenResponse = Invoke-RestMethod -Uri $authUri -Method POST -Body $reqTokenBody -Headers $reqAuthHeader
    
    $authHeader = @{
        "Authorization" = "Bearer $($tokenResponse.access_token)"
        "Content-type" = "application/json"
    }
    
    $authHeader
}

Export-ModuleMember -Function Get-AuthHeader