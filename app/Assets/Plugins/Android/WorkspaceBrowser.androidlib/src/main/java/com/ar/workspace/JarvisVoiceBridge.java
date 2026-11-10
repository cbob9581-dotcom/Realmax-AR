package com.ar.workspace;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import org.json.JSONObject;
import java.util.Locale;
import java.util.concurrent.ConcurrentLinkedQueue;
import java.util.concurrent.atomic.AtomicInteger;
public final class JarvisVoiceBridge {
 private static final ConcurrentLinkedQueue<String> events=new ConcurrentLinkedQueue<>();
 static volatile String status="语音尚未启动",diagnostic="";static volatile int level=0;static volatile boolean visible=false;
 static volatile long awakeUntil=0;static final AtomicInteger wakeEpoch=new AtomicInteger();
 public static void disarm(){awakeUntil=0;wakeEpoch.incrementAndGet();events.clear();}
 public static void setWorkspaceVisible(boolean value){visible=value;if(!value)disarm();}
 public static boolean isEnabled(Context c){return c.getSharedPreferences("jarvis",0).getBoolean("voice",true);}
 public static void start(Activity activity,boolean selfTest){activity.getSharedPreferences("jarvis",0).edit().putBoolean("voice",true).apply();Intent intent=new Intent(activity,JarvisVoiceService.class);intent.putExtra("selfTest",selfTest);if(Build.VERSION.SDK_INT>=26)activity.startForegroundService(intent);else activity.startService(intent);}
 public static void stop(Context c){c.getSharedPreferences("jarvis",0).edit().putBoolean("voice",false).apply();c.stopService(new Intent(c,JarvisVoiceService.class));status="语音已关闭";disarm();}
 public static String takeEvent(){String event=events.poll();return event==null?"":event;}
 public static String getStatus(){return status;}
 public static String getDiagnostic(){return diagnostic;}
 public static int getLevel(){return level;}
 static void emit(String kind,String text){try{JSONObject event=new JSONObject();event.put("kind",kind);event.put("text",text==null?"":text);while(events.size()>=16)events.poll();events.offer(event.toString());}catch(Exception ignored){}}
 static String normalize(String text){return text==null?"":text.toLowerCase(Locale.ROOT).replaceAll("[\\s\\p{Punct}，。！？、]","");}
 static boolean hasWake(String text){String s=normalize(text);for(String wake:new String[]{"贾维斯","加维斯","贾维思","甲维斯","佳维斯","假为师","jarvis"})if(s.contains(wake))return true;return false;}
 static void bringToFront(Context context){if(visible)return;Intent intent=new Intent();intent.setClassName(context,"com.unity3d.player.RealmaxActivity");intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK|Intent.FLAG_ACTIVITY_SINGLE_TOP|Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);context.startActivity(intent);}
}