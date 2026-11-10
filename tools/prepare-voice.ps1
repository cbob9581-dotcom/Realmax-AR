$env:PSModulePath = (Join-Path $PSHOME "Modules")+";"+(Join-Path $env:ProgramFiles "WindowsPowerShell\Modules")
Import-Module Microsoft.PowerShell.Utility -ErrorAction Stop
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$cache=Join-Path $root 'app\.dependencies\voice';New-Item -ItemType Directory -Force -Path $cache | Out-Null
$items=@(
 @{File='vosk-model-small-cn-0.22.zip';Url='https://alphacephei.com/vosk/models/vosk-model-small-cn-0.22.zip';Hash='3AF8B0E7E0F835AE9D414CE5DF580237A3CFB08D586C9FBBB0F7FF29AD5B14BA'},
 @{File='vosk-android-0.3.75.aar';Url='https://repo.maven.apache.org/maven2/com/alphacephei/vosk-android/0.3.75/vosk-android-0.3.75.aar';Hash='AB2F8B91AC8051561AA325546B35FED9A68B36B8121BAC5C6FB927525C4ADFAD'},
 @{File='jna-5.18.1.aar';Url='https://repo.maven.apache.org/maven2/net/java/dev/jna/jna/5.18.1/jna-5.18.1.aar';Hash='7F053E3EC99E14DD71259C82C1C8A02738D64A13C31226B2ACC170F3060951E0'})
foreach($item in $items){$path=Join-Path $cache $item.File;if(!(Test-Path -LiteralPath $path)){Invoke-WebRequest -Uri $item.Url -OutFile $path};if((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $item.Hash){throw ('Hash mismatch: '+$item.File)}}
foreach($artifact in @(@{Group='com/alphacephei';Name='vosk-android';Version='0.3.75'},@{Group='net/java/dev/jna';Name='jna';Version='5.18.1'})){
 $folder=Join-Path (Join-Path $root 'app\.dependencies\maven') ($artifact.Group+'/'+$artifact.Name+'/'+$artifact.Version);New-Item -ItemType Directory -Force -Path $folder | Out-Null
 $base=$artifact.Name+'-'+$artifact.Version;Copy-Item (Join-Path $cache ($base+'.aar')) (Join-Path $folder ($base+'.aar'))
 $pom=Join-Path $folder ($base+'.pom');if(!(Test-Path -LiteralPath $pom)){Invoke-WebRequest -Uri ('https://repo.maven.apache.org/maven2/'+$artifact.Group+'/'+$artifact.Name+'/'+$artifact.Version+'/'+$base+'.pom') -OutFile $pom}
}
$expanded=Join-Path $cache 'expanded';if(!(Test-Path (Join-Path $expanded 'vosk-model-small-cn-0.22\am\final.mdl'))){Expand-Archive -LiteralPath (Join-Path $cache 'vosk-model-small-cn-0.22.zip') -DestinationPath $expanded -Force}
$assets=Join-Path $root 'app\Assets\Plugins\Android\WorkspaceBrowser.androidlib\src\main\assets\vosk-cn'
if(!(Test-Path (Join-Path $assets 'am\final.mdl'))){New-Item -ItemType Directory -Force -Path $assets | Out-Null;Get-ChildItem -LiteralPath (Join-Path $expanded 'vosk-model-small-cn-0.22') | Copy-Item -Destination $assets -Recurse -Force}
[System.IO.File]::WriteAllText((Join-Path $assets 'uuid'),'3af8b0e7-e0f8-35ae-9d41-4ce5df580237')
Write-Output 'Verified Chinese model and Vosk/JNA assets ready'