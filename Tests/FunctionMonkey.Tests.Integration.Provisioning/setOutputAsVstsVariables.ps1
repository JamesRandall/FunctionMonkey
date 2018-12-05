# Make outputs from resource group deployment available to subsequent tasks

$outputs = ConvertFrom-Json $($env:armoutputs)
foreach ($output in $outputs.PSObject.Properties) {
  Write-Host "##vso[task.setvariable variable=arm_$($output.Name)]$($output.Value.value)"
}