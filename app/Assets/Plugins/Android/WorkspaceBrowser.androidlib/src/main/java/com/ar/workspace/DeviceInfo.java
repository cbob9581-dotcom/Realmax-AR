package com.ar.workspace;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.os.BatteryManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.NetworkCapabilities;
import android.net.wifi.WifiManager;
import android.net.wifi.WifiInfo;
import android.media.AudioManager;
public final class DeviceInfo {
 private static volatile long verifiedAt;private static android.net.Network verifiedNetwork;
 public static void markInternetVerified(Activity a){try{ConnectivityManager m=(ConnectivityManager)a.getSystemService(Context.CONNECTIVITY_SERVICE);verifiedNetwork=m==null?null:m.getActiveNetwork();verifiedAt=android.os.SystemClock.elapsedRealtime();}catch(Exception ignored){}}

 public static String getStatus(Activity activity){
  String battery="电量未知",network="网络未连接";
  Intent state=activity.registerReceiver(null,new IntentFilter(Intent.ACTION_BATTERY_CHANGED));
  if(state!=null){int level=state.getIntExtra(BatteryManager.EXTRA_LEVEL,-1),scale=state.getIntExtra(BatteryManager.EXTRA_SCALE,100),status=state.getIntExtra(BatteryManager.EXTRA_STATUS,-1);if(level>=0&&scale>0)battery="电量 "+(level*100/scale)+"%"+(status==BatteryManager.BATTERY_STATUS_CHARGING?" · 充电中":status==BatteryManager.BATTERY_STATUS_FULL?" · 已充满":"");}
  try{ConnectivityManager manager=(ConnectivityManager)activity.getSystemService(Context.CONNECTIVITY_SERVICE);NetworkInfo info=manager==null?null:manager.getActiveNetworkInfo();
   if(info!=null&&info.isConnected()){network=info.getType()==ConnectivityManager.TYPE_WIFI?"Wi-Fi 已连接":"网络已连接";if(info.getType()==ConnectivityManager.TYPE_WIFI){WifiManager wifi=(WifiManager)activity.getApplicationContext().getSystemService(Context.WIFI_SERVICE);WifiInfo connection=wifi==null?null:wifi.getConnectionInfo();String ssid=connection==null?null:connection.getSSID();if(ssid!=null&&!ssid.equals("<unknown ssid>")){ssid=ssid.replace("\"","");network+=" "+ssid;}}
    NetworkCapabilities caps=manager.getNetworkCapabilities(manager.getActiveNetwork());boolean recent=verifiedNetwork!=null&&verifiedNetwork.equals(manager.getActiveNetwork())&&android.os.SystemClock.elapsedRealtime()-verifiedAt<120000;network+=(caps!=null&&caps.hasCapability(NetworkCapabilities.NET_CAPABILITY_VALIDATED))||recent?" · 联网正常":" · 互联网待验证";}
  }catch(Exception ignored){network="网络状态暂不可用";}
  return network+" | "+battery;
 }
 private static SharedPreferences prefs(Activity a){return a.getSharedPreferences("workspace_audio",Context.MODE_PRIVATE);}
 private static AudioManager audio(Activity a){return (AudioManager)a.getSystemService(Context.AUDIO_SERVICE);}
 public static boolean isOutputMuted(Activity a){return prefs(a).getBoolean("muted",true);}
 private static int wanted(Activity a,AudioManager audio){int max=audio.getStreamMaxVolume(AudioManager.STREAM_MUSIC);return Math.max(0,Math.min(max,prefs(a).getInt("volume",Math.max(1,max/3))));}
 public static String getAudioStatus(Activity a){AudioManager manager=audio(a);if(manager==null)return "音量设置暂不可用";boolean muted=isOutputMuted(a);int level=muted?wanted(a,manager):manager.getStreamVolume(AudioManager.STREAM_MUSIC);int max=manager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);return (muted?"预设音量 ":"媒体音量 ")+(max>0?level*100/max:0)+"% · "+(muted?"静音":"声音已开启");}
 public static String changeVolume(Activity a,int direction){AudioManager manager=audio(a);if(manager==null)return "音量设置暂不可用";boolean muted=isOutputMuted(a);int level=muted?wanted(a,manager):manager.getStreamVolume(AudioManager.STREAM_MUSIC);int max=manager.getStreamMaxVolume(AudioManager.STREAM_MUSIC);level=Math.max(0,Math.min(max,level+Integer.signum(direction)));prefs(a).edit().putInt("volume",level).apply();if(!muted)manager.setStreamVolume(AudioManager.STREAM_MUSIC,level,0);return getAudioStatus(a);}
 public static String toggleMute(Activity a){AudioManager manager=audio(a);if(manager==null)return "音量设置暂不可用";boolean muted=isOutputMuted(a);if(!muted){int current=manager.getStreamVolume(AudioManager.STREAM_MUSIC);prefs(a).edit().putInt("volume",current).putBoolean("muted",true).apply();manager.setStreamVolume(AudioManager.STREAM_MUSIC,0,0);}else{prefs(a).edit().putBoolean("muted",false).apply();manager.setStreamVolume(AudioManager.STREAM_MUSIC,wanted(a,manager),0);}SpatialBrowser.refreshAllAudio();return getAudioStatus(a);}
}
