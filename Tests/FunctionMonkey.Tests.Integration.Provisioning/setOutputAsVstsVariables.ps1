param(
 [string]  $resourceGroupName
)

$lastDeployment = Get-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName | Sort Timestamp -Descending | Select -First 1 

if(!$lastDeployment) {
    throw "Deployment could not be found for Resource Group '$resourceGroupName'."
}

if(!$lastDeployment.Outputs) {
    throw "No output parameters could be found for the last deployment of Resource Group '$resourceGroupName'."
}

foreach ($key in $lastDeployment.Outputs.Keys){
    $type = $lastDeployment.Outputs.Item($key).Type
    $value = $lastDeployment.Outputs.Item($key).Value

    if ($type -eq "SecureString") {
        Write-Host "##vso[task.setvariable variable=$key;issecret=true]$value" 
    }
    else {
        Write-Host "##vso[task.setvariable variable=$key;]$value" 
    }
}
