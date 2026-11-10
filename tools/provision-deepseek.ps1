param([switch]$TestCloud,[ValidateSet("status","answer","vision")][string]$TestKind="status",[string]$Serial='ff72859b',[string]$Adb='D:\HACKTHENJU\platform-tools-latest-windows\platform-tools\adb.exe')
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$local=Join-Path $root '.local'
$config=Get-Content -LiteralPath (Join-Path $local 'deepseek-config.json') -Raw | ConvertFrom-Json
$credential=Import-Clixml -LiteralPath (Join-Path $local 'deepseek.credential.xml')
if(!$config.verified){throw 'DeepSeek configuration has not been authenticated'}
if($config.models -contains 'deepseek-flash'){$config.model='deepseek-flash'}
$random=New-Object byte[] 32;$rng=[Security.Cryptography.RandomNumberGenerator]::Create();$rng.GetBytes($random);$rng.Dispose();$nonce=([BitConverter]::ToString($random)).Replace('-','').ToLowerInvariant()
$socketName='realmax.deepseek.'+$nonce.Substring(0,16)
$port=0;$client=$null;$payload=$null;$key=$null
function Read-Exact($stream,[int]$length){$bytes=New-Object byte[] $length;$offset=0;while($offset -lt $length){$n=$stream.Read($bytes,$offset,$length-$offset);if($n -le 0){throw 'Local provisioning connection closed'};$offset+=$n};return ,$bytes}
try{
 & $Adb -s $Serial shell am start -W --activity-single-top -n com.ar.realmaxworkspace/com.unity3d.player.RealmaxActivity --ez provisionDeepSeek true --es provisionToken $nonce | Out-Null
 if($LASTEXITCODE -ne 0){throw 'Unable to open headset provisioning endpoint'}
 $ready=$false;$deadline=[DateTime]::UtcNow.AddSeconds(8)
 do{$sockets=& $Adb -s $Serial shell cat /proc/net/unix;$ready=[bool]($sockets | Select-String -SimpleMatch ('@'+$socketName));if(!$ready){Start-Sleep -Milliseconds 150}}while(!$ready -and [DateTime]::UtcNow -lt $deadline)
 if(!$ready){throw 'Headset provisioning endpoint did not become ready'}
 $forward=& $Adb -s $Serial forward tcp:0 ('localabstract:'+$socketName)
 if($LASTEXITCODE -ne 0){throw 'USB forwarding failed'};$port=[int]($forward | Select-Object -Last 1)
 if($port -le 0){throw 'USB forwarding returned no local port'}
 $key=$credential.GetNetworkCredential().Password
 $settings=@{provider='openai-compatible';endpoint='https://api.deepseek.com/chat/completions';model=$config.model;apiKey=$key}
 $json=@{token=$nonce;settings=$settings;testCloud=[bool]$TestCloud;testKind=$TestKind}|ConvertTo-Json -Depth 4 -Compress
 $payload=[Text.Encoding]::UTF8.GetBytes($json)
 $client=New-Object Net.Sockets.TcpClient;$client.ReceiveTimeout=10000;$client.SendTimeout=10000;$client.Connect('127.0.0.1',$port);$stream=$client.GetStream()
 $length=[BitConverter]::GetBytes([Net.IPAddress]::HostToNetworkOrder([int]$payload.Length));$stream.Write($length,0,4);$stream.Write($payload,0,$payload.Length);$stream.Flush()
 $header=Read-Exact $stream 4;$size=[Net.IPAddress]::NetworkToHostOrder([BitConverter]::ToInt32($header,0));if($size -lt 1 -or $size -gt 8192){throw 'Invalid local provisioning response'}
 $response=[Text.Encoding]::UTF8.GetString((Read-Exact $stream $size))|ConvertFrom-Json
 if(!$response.saved){throw 'Headset did not confirm encrypted settings storage'}
 Write-Output ('DeepSeek securely saved in headset Keystore; model='+$config.model)
}finally{
 if($client){$client.Dispose()};if($payload){[Array]::Clear($payload,0,$payload.Length)};$key=$json=$credential=$null;if($settings){$settings['apiKey']=$null};if($port -gt 0){& $Adb -s $Serial forward --remove ('tcp:'+$port) | Out-Null}
}