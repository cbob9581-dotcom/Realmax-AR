from http.server import BaseHTTPRequestHandler,ThreadingHTTPServer
from pathlib import Path
import json,datetime
ROOT=Path(__file__).resolve().parents[1]
PAGE=r'''<!doctype html><html><meta name="viewport" content="width=device-width,initial-scale=1"><title>REALMAX capability check</title><style>body{font:22px sans-serif;background:#101827;color:white;padding:24px}button{font:24px sans-serif;padding:24px;margin:12px 0;width:100%}pre{white-space:pre-wrap}video{max-width:100%}</style><h1>REALMAX capability check</h1><p>Local test only. No photo or audio files saved. Speaker stays silent.</p><button id="camera">Test camera</button><button id="microphone">Test microphone</button><pre id="out">Ready</pre><script>
const out=document.querySelector('#out');
async function report(result){result.userAgent=navigator.userAgent;out.textContent=JSON.stringify(result,null,2);await fetch('/result',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(result)});}
document.querySelector('#camera').onclick=async()=>{let stream;const r={test:'camera'};try{stream=await navigator.mediaDevices.getUserMedia({video:{facingMode:'environment',width:{ideal:640},height:{ideal:480}},audio:false});const v=document.createElement('video');v.muted=true;v.autoplay=true;v.playsInline=true;document.body.append(v);v.srcObject=stream;await v.play();await new Promise(x=>setTimeout(x,1200));r.width=v.videoWidth;r.height=v.videoHeight;r.label=stream.getVideoTracks()[0].label;r.settings=stream.getVideoTracks()[0].getSettings();r.ok=!!(v.videoWidth&&v.videoHeight&&v.currentTime>0);v.remove();}catch(e){r.ok=false;r.error=e.name+': '+e.message;}finally{if(stream)stream.getTracks().forEach(t=>t.stop());await report(r);}};
document.querySelector('#microphone').onclick=async()=>{let stream,ctx;const r={test:'microphone'};try{stream=await navigator.mediaDevices.getUserMedia({audio:true,video:false});ctx=new AudioContext();await ctx.resume();const input=ctx.createMediaStreamSource(stream),an=ctx.createAnalyser();an.fftSize=2048;input.connect(an);let max=0,sum=0,n=0;for(let i=0;i<20;i++){const data=new Float32Array(an.fftSize);an.getFloatTimeDomainData(data);for(const v of data){max=Math.max(max,Math.abs(v));sum+=v*v;n++;}await new Promise(x=>setTimeout(x,50));}r.label=stream.getAudioTracks()[0].label;r.sampleRate=ctx.sampleRate;r.peak=max;r.rms=Math.sqrt(sum/n);r.ok=stream.getAudioTracks()[0].readyState==='live';r.signalDetected=max>0.00001;}catch(e){r.ok=false;r.error=e.name+': '+e.message;}finally{if(stream)stream.getTracks().forEach(t=>t.stop());if(ctx)await ctx.close();await report(r);}};
</script></html>'''
class Handler(BaseHTTPRequestHandler):
 def do_GET(self):
  self.send_response(200);self.send_header('Content-Type','text/html; charset=utf-8');self.end_headers();self.wfile.write(PAGE.encode())
 def do_POST(self):
  length=int(self.headers.get('Content-Length','0'))
  if length>16384:self.send_error(413);return
  data=json.loads(self.rfile.read(length));data['recorded_at']=datetime.datetime.now(datetime.timezone(datetime.timedelta(hours=8))).isoformat()
  folder=ROOT/'docs'/'runs';folder.mkdir(parents=True,exist_ok=True)
  name=datetime.datetime.now().strftime('%Y%m%d-%H%M%S')+'-'+str(data.get('test','unknown'))+'.json'
  (folder/name).write_text(json.dumps(data,ensure_ascii=False,indent=2),encoding='utf-8')
  print(json.dumps(data,ensure_ascii=False),flush=True)
  self.send_response(200);self.end_headers();self.wfile.write(b'OK')
 def log_message(self,*args):pass
print('Local diagnostics: http://127.0.0.1:8765; no raw camera/audio stored',flush=True)
ThreadingHTTPServer(('127.0.0.1',8765),Handler).serve_forever()
