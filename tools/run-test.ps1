param(
    [string]$TestCategory = "",
    [string]$OutputPath = "",
    [bool]$Local = $true,
    [bool]$NoAzure = $true,
    [string]$ConfigurationFilePath = "",
    [string]$PublicCertificatePath = "",
    [string]$PrivateKeyCertificatePath = "",
    [string]$LogPath = "c:\CMPLogs\TestLog.log"
)

# Process parameters and provide intelligent defaults
if ($TestCategory -eq "")
{
    $TestCategory = "UnitTest"
}
elseif (-not(($TestCategory -eq "UnitTest") -or ($TestCategory -eq "Environment") -or ($TestCategory -eq "Integration")))
{
    $error = New-Object System.ArgumentException "TestCategory has an invalid value.  Valid values are UnitTest|Integration|Environment"
    throw $error
}
if ($OutputPath -eq "")
{
    $error = New-Object System.ArgumentException "OutputPath is not set"
    throw $error
}
if ($ConfigurationFilePath -eq "")
{
    $Local = $true
}
else
{
    if ($Local)
    {
        $error = New-Object System.ArgumentException "Local execution does not require a configuration file"
        throw $error
    }
}
if ($PublicCertificatePath -eq "")
{
    $defaultFolder = "\\cdmstore\data\lincolnu\scripts\cmptestcert"
    write-host "Using default public key certificate folder $defaultFolder"
    $PublicCertificatePath = $defaultFolder
}
if ($PrivateKeyCertificatePath -eq "")
{
    $defaultFolder = "\\cdmstore\data\lincolnu\scripts\cmptestcert"
    write-host "Using default private key certificate folder $defaultFolder"
    $PrivateKeyCertificatePath = $defaultFolder
}

$Now = Get-Date
$NowString = $Now.ToString()
$NowAsName = $NowString.Replace(" ", "_")
$NowAsName = $NowAsName.Replace("/", "-")
$NowAsName = $NowAsName.Replace(":", ".")

# Copy the build to a temporary path
$Build = Join-Path $env:TEMP $NowAsName
New-Item $Build -type directory 
$SourceDirectory = Join-Path $OutputPath "*"
copy-item -rec $SourceDirectory $Build -erroraction Stop

# Load the test dll list
$exe = "$Build\testcollector.exe"
$args = "$TestCategory $Build -all"

$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = $exe
$pinfo.RedirectStandardError = $true
$pinfo.RedirectStandardOutput = $true
$pinfo.UseShellExecute = $false
$pinfo.Arguments = $args
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $pinfo
$p.Start() | Out-Null
$p.WaitForExit()
$stdout = $p.StandardOutput.ReadToEnd()
$stderr = $p.StandardError.ReadToEnd()

if (-not($LogPath -eq ""))
{
    out-file -FilePath $LogPath -Append -inputobject $stdout
    out-file -FilePath $LogPath -Append -inputobject $stderr
}

# Loop through possible versions of visual studio to find the highest version currently installed
for ($i=25; $i -gt 0; $i--)
{
    $key = "HKLM:\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\$i.0"
    $VSInstallDirValue = Get-ItemProperty -Path $key -ErrorAction SilentlyContinue
    if (-not($VSInstallDirValue -eq $null))
    {
        $VSInstallDirValue = $VSInstallDirValue.InstallDir
        if (-not(($VSInstallDirValue -eq $null) -or ($VSInstallDirValue -eq "")))
        {
            break;
        }
    }
}

$exe="$VSInstallDirValue" + "CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
$args = $stdout + " /Settings:$Build\CodeCoverage.runsettings /InIsolation /Logger:trx /UseVsixExtensions:true"

if (-not($LogPath -eq ""))
{
    out-file -FilePath $LogPath -Append -inputobject "Running $exe with arguments $args"
}

write-host "Calling " + $exe + " with arguments " + $args

$pinfo = New-Object System.Diagnostics.ProcessStartInfo
$pinfo.FileName = $exe
$pinfo.RedirectStandardError = $true
$pinfo.RedirectStandardOutput = $true
$pinfo.UseShellExecute = $false
$pinfo.Arguments = $args
$p = New-Object System.Diagnostics.Process
$p.StartInfo = $pinfo
$p.Start() | Out-Null

$timeout = New-Timespan -seconds 600
$stopWatch = [Diagnostics.Stopwatch]::StartNew()
while ($stopWatch.Elapsed -lt $timeout){
    if ($p.HasExited)
    {
        break;
    }
    
    Start-Sleep -seconds 1
}

if (!$p.HasExited)
{
    $p.Kill()
}

$stdout = $p.StandardOutput.ReadToEnd()
$stderr = $p.StandardError.ReadToEnd()

write-host "Exit code " + $stderr
write-host $stdout

if (-not($LogPath -eq ""))
{
    out-file -FilePath $LogPath -Append -inputobject $stdout
    out-file -FilePath $LogPath -Append -inputobject $stderr
}
