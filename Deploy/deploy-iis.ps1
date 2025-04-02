param (
    [string]$projectName = "",
    [string]$iisPath = "",
    [string]$poolName = ""
)

$commitHash = (git rev-parse HEAD).Substring(0,16)
Write-Output ("Commit hash is $commitHash")

Write-Output ('Building app ...')
Set-Location $projectName
dotnet build
if ($lastexitcode -ne 0)
{
    write-host "Error in puild, Code: $lastexitcode, Exiting" -foregroundcolor "red"
    exit $lastexitcode
}

Write-Output ('Prepare publishing content')
dotnet publish -c Release
if ($lastexitcode -ne 0)
{
    write-host "Error in publish, Code: $lastexitcode, Exiting" -foregroundcolor "red"
    exit $lastexitcode
}

Write-Output ('Stopping pool')
if((Get-WebAppPoolState -Name $poolName).Value -ne 'Stopped'){
    Write-Output ('Stopping Application Pool: {0}' -f $poolName)
    Stop-WebAppPool -Name $poolName
	if ($lastexitcode -ne 0)
	{
		write-host "Error in stopping pool, Code: $lastexitcode, Exiting" -foregroundcolor "red"
		exit $lastexitcode
	}
} 

while((Get-WebAppPoolState -Name $poolName).Value -ne 'Stopped'){
	Write-Output ('Waiting for app pool is stopped...')
    Start-Sleep -s 1	
}

Write-Output ('Create version.txt')
New-Item -Path "bin\Release\net7.0\publish\" -Name "version.txt" -ItemType "file" -Value $commitHash

Write-Output ('Copy files to IIS directory')
Remove-Item bin\Release\net7.0\publish\appsettings.Development.json
Copy-Item bin\Release\net7.0\publish\* -Destination $iisPath -Recurse -Force
if ($lastexitcode -ne 0)
{
	write-host "Error in copying to iis, Code: $lastexitcode, Exiting" -foregroundcolor "red"
	exit $lastexitcode
}

Write-Output ('Starting Application Pool: {0}' -f $poolName)
Start-WebAppPool -Name $poolName
if ($lastexitcode -ne 0)
{
	write-host "Error in starting pool, Code: $lastexitcode, Exiting" -foregroundcolor "red"
	exit $lastexitcode
}

exit 0