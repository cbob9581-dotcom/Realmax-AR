package com.ar.workspace;
import android.Manifest;
import android.app.*;
import android.content.*;
import android.content.pm.PackageManager;
import android.media.*;
import android.os.*;
import android.util.Log;
import org.json.JSONObject;
import org.vosk.*;
import org.vosk.android.StorageService;
import java.io.*;
public class JarvisVoiceService extends Service {
 private volatile boolean running;private volatile AudioRecord audio;private Thread worker;private boolean utteranceWoke;private long lastCommandAt;private int utteranceEpoch;private String lastCommand="";
 @Override public IBinder onBind(Intent intent){return null;}
 @Override public int onStartCommand(Intent intent,int flags,int id){
  if(checkSelfPermission(Manifest.permission.RECORD_AUDIO)!=PackageManager.PERMISSION_GRANTED){JarvisVoiceBridge.status="请允许麦克风权限";stopSelf();return START_NOT_STICKY;}
  NotificationManager manager=(NotificationManager)getSystemService(NOTIFICATION_SERVICE);
  if(Build.VERSION.SDK_INT>=26)manager.createNotificationChannel(new NotificationChannel("jarvis","贾维斯语音",NotificationManager.IMPORTANCE_LOW));
  Intent home=new Intent();home.setClassName(this,"com.unity3d.player.RealmaxActivity");
  PendingIntent open=PendingIntent.getActivity(this,0,home,PendingIntent.FLAG_UPDATE_CURRENT|PendingIntent.FLAG_IMMUTABLE);
  Notification.Builder notice=Build.VERSION.SDK_INT>=26?new Notification.Builder(this,"jarvis"):new Notification.Builder(this);
  startForeground(119,notice.setSmallIcon(android.R.drawable.ic_btn_speak_now).setContentTitle("贾维斯待命").setContentText("本机语音唤起已开启；可在工作台关闭").setContentIntent(open).setOngoing(true).setSound(null).build());
  if(running)return START_STICKY;running=true;boolean test=intent!=null&&intent.getBooleanExtra("selfTest",false);
  worker=new Thread(()->recognize(test),"JarvisOfflineVoice");worker.start();return START_STICKY;
 }
 private void recognize(boolean test){
  Model model=null;Recognizer recognizer=null;AudioRecord mic=null;
  try{
   JarvisVoiceBridge.status="正在载入中文语音模型…";LibVosk.setLogLevel(LogLevel.WARNINGS);
   String path=StorageService.sync(this,"vosk-cn","jarvis-model");if(!running)return;model=new Model(path);if(!running)return;
   if(test)selfTest(model);recognizer=new Recognizer(model,16000.f);recognizer.setEndpointerDelays(.3f,.6f,10.f);
   int min=AudioRecord.getMinBufferSize(16000,AudioFormat.CHANNEL_IN_MONO,AudioFormat.ENCODING_PCM_16BIT);
   mic=new AudioRecord(MediaRecorder.AudioSource.VOICE_RECOGNITION,16000,AudioFormat.CHANNEL_IN_MONO,AudioFormat.ENCODING_PCM_16BIT,Math.max(min,6400));audio=mic;
   if(mic.getState()!=AudioRecord.STATE_INITIALIZED)throw new IOException("Microphone initialization failed");if(!running)return;mic.startRecording();
   short[] samples=new short[1600];JarvisVoiceBridge.status="贾维斯待命 · 本机识别";Log.i("ARWORKSPACE","Jarvis microphone running: mono PCM16 16000Hz");
   while(running){int n=mic.read(samples,0,samples.length);if(n<=0){if(!running)break;throw new IOException("Microphone read failed");}
    double sum=0;for(int i=0;i<n;i++)sum+=(double)samples[i]*samples[i];double rms=Math.sqrt(sum/n);JarvisVoiceBridge.level=(int)Math.max(0,Math.min(100,(20*Math.log10(Math.max(1,rms)/32768.0)+60)*100/60));
    if(recognizer.acceptWaveForm(samples,n))finalText(new JSONObject(recognizer.getResult()).optString("text"));else partial(new JSONObject(recognizer.getPartialResult()).optString("partial"));
    if(SystemClock.uptimeMillis()>JarvisVoiceBridge.awakeUntil)JarvisVoiceBridge.status="贾维斯待命 · 本机识别";
   }
  }catch(Throwable error){JarvisVoiceBridge.status="语音未就绪："+error.getClass().getSimpleName();Log.e("ARWORKSPACE","Jarvis voice initialization/capture failed: "+error.getClass().getSimpleName());}
  finally{audio=null;if(mic!=null){try{mic.stop();}catch(Exception ignored){}mic.release();}if(recognizer!=null)recognizer.close();if(model!=null)model.close();running=false;JarvisVoiceBridge.level=0;}
 }
 private void wake(){utteranceEpoch=JarvisVoiceBridge.wakeEpoch.get();JarvisVoiceBridge.awakeUntil=SystemClock.uptimeMillis()+12000;JarvisVoiceBridge.status="我在，请说指令…";JarvisVoiceBridge.emit("wake","");JarvisVoiceBridge.bringToFront(this);}
 private void partial(String text){if(!utteranceWoke&&JarvisVoiceBridge.hasWake(text)){utteranceWoke=true;wake();}}
 private void finalText(String text){
  long now=SystemClock.uptimeMillis();if(utteranceWoke&&utteranceEpoch!=JarvisVoiceBridge.wakeEpoch.get()){utteranceWoke=false;return;}
  boolean actualWake=JarvisVoiceBridge.hasWake(text);if(actualWake){if(!utteranceWoke)wake();JarvisVoiceBridge.awakeUntil=now+12000;}
  boolean eligible=actualWake||utteranceWoke||(JarvisVoiceBridge.visible&&now<=JarvisVoiceBridge.awakeUntil);
  if(eligible&&text!=null&&!text.trim().isEmpty()){String normalized=JarvisVoiceBridge.normalize(text);if(!normalized.equals(lastCommand)||now-lastCommandAt>2000){lastCommand=normalized;lastCommandAt=now;JarvisVoiceBridge.emit("command",text);}}
  utteranceWoke=false;
 } private void selfTest(Model model){
  try(InputStream stream=getAssets().open("jarvis-selftest.wav");DataInputStream in=new DataInputStream(stream);Recognizer test=new Recognizer(model,16000.f)){
   byte[] header=new byte[12];in.readFully(header);int data=0;while(true){byte[] name=new byte[4];in.readFully(name);int length=Integer.reverseBytes(in.readInt());if(new String(name,"US-ASCII").equals("data")){data=length;break;}if(length<0||length>1048576)throw new IOException("Fixture header invalid");in.skipBytes(length+(length&1));}
   StringBuilder recognized=new StringBuilder();byte[] buffer=new byte[3200];while(data>0){int n=in.read(buffer,0,Math.min(data,buffer.length));if(n<0)break;if(test.acceptWaveForm(buffer,n))recognized.append(new JSONObject(test.getResult()).optString("text")).append(" ");data-=n;}
   String text=recognized.append(new JSONObject(test.getFinalResult()).optString("text")).toString();Log.i("ARWORKSPACE","JARVIS synthetic fixture result: "+text);String normalized=JarvisVoiceBridge.normalize(text);boolean ok=JarvisVoiceBridge.hasWake(text)&&(normalized.contains("软件列表")||normalized.contains("应用列表"));
   JarvisVoiceBridge.diagnostic=ok?"合成语音自检通过":"合成语音自检未识别完整指令";Log.i("ARWORKSPACE","JARVIS SELFTEST wake/apps="+ok);
   JarvisVoiceBridge.emit("selftest",JarvisVoiceBridge.diagnostic);if(ok){wake();JarvisVoiceBridge.emit("command",text);}
  }catch(Exception error){JarvisVoiceBridge.diagnostic="语音自检失败："+error.getClass().getSimpleName();Log.e("ARWORKSPACE",JarvisVoiceBridge.diagnostic);}
 }
 @Override public void onDestroy(){running=false;AudioRecord mic=audio;if(mic!=null)try{mic.stop();}catch(Exception ignored){}stopForeground(true);JarvisVoiceBridge.status="语音已停止";super.onDestroy();}
}