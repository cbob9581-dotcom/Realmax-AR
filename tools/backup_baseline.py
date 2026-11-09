import pathlib,subprocess,datetime,hashlib,json,shutil,sys
ROOT=pathlib.Path(r'F:\AR')
ADB=r'D:\HACKTHENJU\platform-tools-latest-windows\platform-tools\adb.exe'
SERIAL='ff72859b'
def digest(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for b in iter(lambda:f.read(4*1024*1024),b''):h.update(b)
 return h.hexdigest()
def adb(*args):
 r=subprocess.run([ADB,'-s',SERIAL,*args],capture_output=True,timeout=90)
 if r.returncode:raise RuntimeError(r.stderr.decode('utf-8','replace'))
 return r.stdout
resume=len(sys.argv)>1
stamp=sys.argv[1] if resume else datetime.datetime.now(datetime.timezone(datetime.timedelta(hours=8))).strftime('%Y%m%d-%H%M%S')
if not stamp or any(c not in '0123456789-' for c in stamp):raise ValueError('Invalid snapshot name')
backup=ROOT/'.backups'/stamp
backup.mkdir(parents=True,exist_ok=resume)
manifest={'created_at':stamp,'serial':SERIAL,'scope':'Source assets, installed key APKs, current configuration; NOT full firmware or app-private data','source_files':[],'device_apks':[]}
for folder in ('sdk','Export_apk'):
 for src in sorted((ROOT/folder).rglob('*')):
  if not src.is_file():continue
  rel=src.relative_to(ROOT); dst=backup/'source'/rel;dst.parent.mkdir(parents=True,exist_ok=True)
  if not resume or not dst.exists():shutil.copy2(src,dst)
  original=digest(src); copied=digest(dst)
  if original!=copied:raise RuntimeError('Backup hash mismatch: '+str(rel))
  manifest['source_files'].append({'path':rel.as_posix(),'bytes':src.stat().st_size,'sha256':original})
print('Source copies verified:',len(manifest['source_files']),flush=True)
device=backup/'device';device.mkdir(exist_ok=resume)
commands={'properties':'getprop','packages':'pm list packages -f','disabled-packages':'pm list packages -d','audio':'dumpsys audio','camera':'dumpsys media.camera','audio-policy':'dumpsys media.audio_policy','mounts':'cat /proc/mounts','features':'pm list features','display':'wm size; wm density','settings-selected':'settings get global auto_time; settings get global auto_time_zone; settings get system sound_effects_enabled; settings get global wifi_on; settings get global bluetooth_on'}
for name,cmd in commands.items():(device/(name+'.txt')).write_bytes(adb('shell',cmd))
for package in ('com.realmax.launcher','com.setting.rm','com.RMSR.ExampleScene','com.leapmotion.leapdaemon','com.RMSR.Adjust','com.RMSR.AdjustLM'):
 (device/(package+'-package.txt')).write_bytes(adb('shell','dumpsys package '+package))
 paths=adb('shell','pm path '+package).decode().splitlines()
 for i,line in enumerate(paths):
  if not line.startswith('package:'):continue
  remote=line[8:].strip();dest=device/(package+('-split'+str(i) if i else '')+'.apk')
  adb('pull',remote,str(dest))
  remotehash=adb('shell','toybox sha1sum '+remote).decode().split()[0]
  localhash=digest(dest)
  with dest.open('rb') as f:
   h=hashlib.sha1()
   for b in iter(lambda:f.read(4*1024*1024),b''):h.update(b)
  if remotehash!=h.hexdigest():raise RuntimeError('APK transfer hash mismatch: '+package)
  manifest['device_apks'].append({'package':package,'remote_path':remote,'file':dest.name,'bytes':dest.stat().st_size,'sha256':localhash})
manifest['snapshot_files']=[{'path':p.relative_to(backup).as_posix(),'bytes':p.stat().st_size,'sha256':digest(p)} for p in device.rglob('*') if p.is_file() and p.suffix!='.apk']
(backup/'manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
baseline=ROOT/'docs'/'baselines';baseline.mkdir(parents=True,exist_ok=True)
(baseline/(stamp+'.json')).write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
print('Backup complete:',backup,flush=True)
print('Verified device APKs:',len(manifest['device_apks']),flush=True)
