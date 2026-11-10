$configurationDirectory=Join-Path (Split-Path $PSScriptRoot -Parent) '.local'
New-Item -ItemType Directory -Force -Path $configurationDirectory | Out-Null
$env:PSModulePath = (Join-Path $PSHOME "Modules")+";"+(Join-Path $env:ProgramFiles "WindowsPowerShell\Modules")
Import-Module Microsoft.PowerShell.Utility -ErrorAction Stop
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
$form=New-Object System.Windows.Forms.Form
$form.Text='REALMAX 贾维斯 · DeepSeek 配置'
$form.Size=New-Object System.Drawing.Size(620,285)
$form.StartPosition='CenterScreen'
$form.TopMost=$true
$form.ShowInTaskbar=$true
$form.FormBorderStyle='FixedDialog'
$form.MaximizeBox=$false
$form.MinimizeBox=$false
$label=New-Object System.Windows.Forms.Label
$label.Text='请输入 DeepSeek API 密钥（输入会遮罩）。仅加密保存在本机；不写入源码、GitHub 或日志。'
$label.Location=New-Object System.Drawing.Point(20,20)
$label.Size=New-Object System.Drawing.Size(570,55)
$form.Controls.Add($label)
$inputBox=New-Object System.Windows.Forms.TextBox
$inputBox.Location=New-Object System.Drawing.Point(20,85)
$inputBox.Size=New-Object System.Drawing.Size(565,30)
$inputBox.UseSystemPasswordChar=$true
$form.Controls.Add($inputBox)
$status=New-Object System.Windows.Forms.Label
$status.Location=New-Object System.Drawing.Point(20,130)
$status.Size=New-Object System.Drawing.Size(570,40)
$status.Text='验证只读取可用模型列表，不发送聊天内容。'
$form.Controls.Add($status)
$save=New-Object System.Windows.Forms.Button
$save.Text='验证并加密保存'
$save.Location=New-Object System.Drawing.Point(300,185)
$save.Size=New-Object System.Drawing.Size(150,35)
$cancel=New-Object System.Windows.Forms.Button
$cancel.Text='稍后配置'
$cancel.Location=New-Object System.Drawing.Point(465,185)
$cancel.Size=New-Object System.Drawing.Size(120,35)
$cancel.Add_Click({$inputBox.Clear();$form.Close()})
$save.Add_Click({
 $key=$inputBox.Text.Trim()
 if([string]::IsNullOrWhiteSpace($key)){$status.Text='请先输入密钥。';return}
 $save.Enabled=$false;$status.Text='正在通过官方 API 验证认证…';[System.Windows.Forms.Application]::DoEvents()
 try{
  $headers=@{Authorization='Bearer '+$key}
  $models=Invoke-RestMethod -Uri 'https://api.deepseek.com/models' -Headers $headers -Method Get -TimeoutSec 25 -MaximumRedirection 0
  $secure=ConvertTo-SecureString -String $key -AsPlainText -Force
  $credential=New-Object System.Management.Automation.PSCredential('DeepSeek',$secure)
  $credential | Export-Clixml -LiteralPath (Join-Path  'deepseek.credential.xml')
  $ids=@($models.data | ForEach-Object {$_.id})
  $model=if($ids -contains 'deepseek-flash'){'deepseek-flash'}elseif($ids -contains 'deepseek-chat'){'deepseek-chat'}elseif($ids.Count -gt 0){$ids[0]}else{'deepseek-chat'}
  @{provider='openai-compatible';endpoint='https://api.deepseek.com/chat/completions';model=$model;verified=$true;models=$ids} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path  'deepseek-config.json') -Encoding UTF8
  $inputBox.Clear();$key=$null;$headers.Clear();$status.Text='认证通过，密钥已加密保存。眼镜端配置完成后会再次报告。';$save.Text='已保存';$cancel.Text='关闭'
 }catch{
  $code=if($_.Exception.Response){[int]$_.Exception.Response.StatusCode}else{0}
  $status.Text='验证未通过（HTTP '+$code+'）。请检查密钥或网络，完整密钥不会输出。';$save.Enabled=$true
 }finally{$key=$null;if($headers){$headers.Clear()}}
})
$form.Controls.Add($save);$form.Controls.Add($cancel)
$form.AcceptButton=$save
$form.Add_Shown({$form.Activate();$form.BringToFront();$inputBox.Focus()})
[void]$form.ShowDialog()
$inputBox.Clear();$form.Dispose()