package com.ar.workspace;
import android.app.Activity;
import android.content.Intent;
import android.net.LocalServerSocket;
import android.net.LocalSocket;
import android.os.Handler;
import android.os.Looper;
import org.json.JSONObject;
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.util.Arrays;
public final class SettingsProvisioner {
 private static LocalServerSocket active;
 public static void handleIntent(Activity activity,Intent intent){
  if(intent==null||!intent.getBooleanExtra("provisionDeepSeek",false))return;
  String token=intent.getStringExtra("provisionToken");if(token==null||!token.matches("[a-f0-9]{64}"))return;
  intent.removeExtra("provisionToken");intent.removeExtra("provisionDeepSeek");
  try{final LocalServerSocket server; synchronized(SettingsProvisioner.class){if(active!=null)active.close();server=new LocalServerSocket("realmax.deepseek."+token.substring(0,16));active=server;}
   new Handler(Looper.getMainLooper()).postDelayed(()->{try{server.close();}catch(IOException ignored){}},60000);
   new Thread(()->receive(activity,server,token),"DeepSeekLocalProvision").start();
  }catch(Exception ignored){}
 }
 private static void receive(Activity activity,LocalServerSocket server,String token){
  byte[] bytes=null;int stage=0;
  try(LocalSocket socket=server.accept();DataInputStream in=new DataInputStream(socket.getInputStream());DataOutputStream out=new DataOutputStream(socket.getOutputStream())){
   stage=1;int peer=socket.getPeerCredentials().getUid();if(peer!=0&&peer!=2000)throw new IOException("ADB peer required");
   stage=2;socket.setSoTimeout(10000);int size=in.readInt();if(size<1||size>16384)throw new IOException("Invalid provision size");bytes=new byte[size];in.readFully(bytes);JSONObject payload=new JSONObject(new String(bytes,StandardCharsets.UTF_8));
   stage=3;String supplied=payload.optString("token");if(!MessageDigest.isEqual(token.getBytes(StandardCharsets.UTF_8),supplied.getBytes(StandardCharsets.UTF_8)))throw new IOException("Unauthorized provision");
   stage=4;JSONObject settings=payload.getJSONObject("settings");String key=settings.optString("apiKey"),endpoint=settings.optString("endpoint"),model=settings.optString("model");
   boolean valid="openai-compatible".equals(settings.optString("provider"))&&"https://api.deepseek.com/chat/completions".equals(endpoint)&&model.matches("[a-zA-Z0-9._/-]{1,128}")&&key.length()>=10&&key.length()<=512&&!key.contains("\r")&&!key.contains("\n");
   stage=5;boolean ok=valid&&SecureSettings.save(activity,settings.toString());settings.remove("apiKey");if(ok&&payload.optBoolean("testCloud",false))JarvisVoiceBridge.emit("cloud_test",("answer".equals(payload.optString("testKind"))||"vision".equals(payload.optString("testKind")))?payload.optString("testKind"):"status");byte[] response=("{\"saved\":"+ok+"}").getBytes(StandardCharsets.UTF_8);out.writeInt(response.length);out.write(response);out.flush();
  }catch(Exception error){android.util.Log.w("JARVIS","USB setup failed stage="+stage+" type="+error.getClass().getSimpleName());}finally{if(bytes!=null)Arrays.fill(bytes,(byte)0);try{server.close();}catch(IOException ignored){}synchronized(SettingsProvisioner.class){if(active==server)active=null;}}
 }
}