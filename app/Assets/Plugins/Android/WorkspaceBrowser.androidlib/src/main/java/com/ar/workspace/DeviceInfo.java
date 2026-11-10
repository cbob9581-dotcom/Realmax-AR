package com.ar.workspace;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
public final class DeviceInfo {
 public static String getStatus(Activity activity){
  String battery="Battery unknown",network="Network disconnected";
  Intent state=activity.registerReceiver(null,new IntentFilter(Intent.ACTION_BATTERY_CHANGED));
  if(state!=null){int level=state.getIntExtra(BatteryManager.EXTRA_LEVEL,-1),scale=state.getIntExtra(BatteryManager.EXTRA_SCALE,100);if(level>=0&&scale>0)battery="Battery "+(level*100/scale)+"%"+(state.getIntExtra(BatteryManager.EXTRA_PLUGGED,0)!=0?" (plugged in)":"");}
  ConnectivityManager manager=(ConnectivityManager)activity.getSystemService(Context.CONNECTIVITY_SERVICE);
  NetworkInfo info=manager==null?null:manager.getActiveNetworkInfo();
  if(info!=null&&info.isConnected())network=info.getType()==ConnectivityManager.TYPE_WIFI?"Wi-Fi connected":"Network connected";
  return network+" | "+battery;
 }
}