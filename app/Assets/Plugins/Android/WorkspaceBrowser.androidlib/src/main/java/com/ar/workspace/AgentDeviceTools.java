package com.ar.workspace;
import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.*;
import android.content.pm.*;
import android.net.wifi.*;
import android.provider.Settings;
import org.json.*;
import java.util.*;
public final class AgentDeviceTools {
 public static String listApps(Activity activity){
  JSONArray apps=new JSONArray();Intent intent=new Intent(Intent.ACTION_MAIN);intent.addCategory(Intent.CATEGORY_LAUNCHER);PackageManager pm=activity.getPackageManager();Set<String> seen=new HashSet<>();List<ResolveInfo> entries=pm.queryIntentActivities(intent,0);Collections.sort(entries,new ResolveInfo.DisplayNameComparator(pm));
  for(ResolveInfo entry:entries){String pkg=entry.activityInfo.packageName;if(!seen.add(pkg))continue;try{JSONObject app=new JSONObject();app.put("label",String.valueOf(entry.loadLabel(pm)));app.put("packageName",pkg);apps.put(app);}catch(JSONException ignored){}}
  try{return new JSONObject().put("apps",apps).toString();}catch(JSONException ignored){return "{\"apps\":[]}";}
 }
 public static String openApp(Activity activity,String pkg){
  PackageManager pm=activity.getPackageManager();Intent query=new Intent(Intent.ACTION_MAIN);query.addCategory(Intent.CATEGORY_LAUNCHER);boolean listed=false;
  for(ResolveInfo entry:pm.queryIntentActivities(query,0))if(entry.activityInfo.packageName.equals(pkg)){listed=true;break;}
  if(!listed)return "该软件没有可用启动入口";Intent launch=pm.getLaunchIntentForPackage(pkg);if(launch==null)return "软件启动入口不可用";
  activity.runOnUiThread(()->{try{activity.startActivity(launch);}catch(Exception ignored){JarvisVoiceBridge.emit("tool_error","软件未能打开");}});return "正在请求打开软件";
 }
 public static String bluetoothStatus(Activity activity){BluetoothAdapter adapter=BluetoothAdapter.getDefaultAdapter();return adapter==null?"本机没有蓝牙适配器":adapter.isEnabled()?"蓝牙已开启；手柄使用原厂连接接口":"蓝牙已关闭";}
 public static void openBluetoothSettings(Activity activity){activity.runOnUiThread(()->activity.startActivity(new Intent(Settings.ACTION_BLUETOOTH_SETTINGS)));}
 public static String scanWifi(Activity activity){
  JSONObject result=new JSONObject();JSONArray networks=new JSONArray();WifiManager manager=(WifiManager)activity.getApplicationContext().getSystemService(Context.WIFI_SERVICE);
  try{if(manager==null)return result.put("message","Wi-Fi 不可用").put("networks",networks).toString();if(!manager.isWifiEnabled())manager.setWifiEnabled(true);manager.startScan();List<ScanResult> scans=manager.getScanResults();Collections.sort(scans,(a,b)->Integer.compare(b.level,a.level));Set<String> seen=new HashSet<>();
   for(ScanResult scan:scans){if(scan.SSID==null||scan.SSID.isEmpty()||!seen.add(scan.SSID))continue;JSONObject row=new JSONObject();row.put("ssid",scan.SSID);row.put("level",scan.level);row.put("secured",scan.capabilities.contains("PSK")||scan.capabilities.contains("WEP")||scan.capabilities.contains("EAP")||scan.capabilities.contains("SAE"));networks.put(row);if(networks.length()>=30)break;}
   result.put("message",networks.length()==0?"未发现网络；可手动输入 SSID。扫描需要定位权限及系统定位开关。":"选择网络，再手动输入密码");
  }catch(Exception error){try{result.put("message","扫描暂不可用，可手动输入 SSID");}catch(JSONException ignored){}}
  try{return result.put("networks",networks).toString();}catch(JSONException ignored){return "{}";}
 }
 private static String quoted(String value){return "\""+value.replace("\\","\\\\").replace("\"","\\\"")+"\"";}
 public static String connectWifi(Activity activity,String ssid,String password){
  if(ssid==null||ssid.isEmpty()||ssid.getBytes(java.nio.charset.StandardCharsets.UTF_8).length>32)return "SSID 长度不正确";
  if(password==null)password="";if(!password.isEmpty()&&!password.matches("[0-9a-fA-F]{64}")&&(password.length()<8||password.length()>63))return "WPA/WPA2 密码应为 8–63 位，或 64 位十六进制";
  try{WifiManager manager=(WifiManager)activity.getApplicationContext().getSystemService(Context.WIFI_SERVICE);if(manager==null)return "Wi-Fi 不可用";manager.setWifiEnabled(true);int existing=-1;List<WifiConfiguration> saved=manager.getConfiguredNetworks();if(saved!=null)for(WifiConfiguration c:saved)if(quoted(ssid).equals(c.SSID)){existing=c.networkId;break;}
   int id=existing;if(existing<0||!password.isEmpty()){WifiConfiguration config=new WifiConfiguration();config.SSID=quoted(ssid);if(password.isEmpty())config.allowedKeyManagement.set(WifiConfiguration.KeyMgmt.NONE);else{config.allowedKeyManagement.set(WifiConfiguration.KeyMgmt.WPA_PSK);config.preSharedKey=password.matches("[0-9a-fA-F]{64}")?password:quoted(password);}if(existing>=0){config.networkId=existing;id=manager.updateNetwork(config);}else id=manager.addNetwork(config);}
   if(id<0)return "系统未接受此网络配置";manager.saveConfiguration();if(!manager.enableNetwork(id,true))return "无法启用该网络";manager.reconnect();return "已提交连接请求，请查看顶部网络状态";
  }catch(Exception error){return "连接失败："+error.getClass().getSimpleName();}
 }
}