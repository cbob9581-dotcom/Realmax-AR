using System;
using System.Collections.Generic;
using System.Text;
public enum JarvisAction { None,Wake,Look,Apps,Settings,Wifi,Bluetooth,Browser,Home,Status,OpenApp,AskAi,Cancel,StopVoice }
[Serializable] public class JarvisApp {public string label,packageName;public JarvisApp(string l,string p){label=l;packageName=p;}}
public sealed class JarvisCommand {public JarvisAction Action;public string Argument;public JarvisCommand(JarvisAction a,string arg=""){Action=a;Argument=arg??"";}}
public sealed class JarvisSession {
 double awakeUntil=-1,lastTime=-100;string lastCommand="";
 public bool IsAwake(double now){return now<=awakeUntil;}
 public void Cancel(){awakeUntil=-1;lastCommand="";}
 public JarvisCommand Accept(string text,double now,JarvisApp[] apps){
  if(string.IsNullOrWhiteSpace(text)||text.Length>2048)return new JarvisCommand(JarvisAction.None);
  string normalized=Normalize(text);int wakeEnd=0;
  foreach(string wake in new[]{"贾维斯","加维斯","贾维思","甲维斯","佳维斯","假为师","jarvis"}){int i=normalized.IndexOf(wake,StringComparison.Ordinal);if(i>=0){wakeEnd=i+wake.Length;break;}}
  if(wakeEnd>0){awakeUntil=now+12;normalized=normalized.Substring(wakeEnd);if(normalized.Length==0)return new JarvisCommand(JarvisAction.Wake);}
  else if(!IsAwake(now))return new JarvisCommand(JarvisAction.None);
  if(normalized=="取消"||normalized=="算了"||normalized=="不用了"){Cancel();return new JarvisCommand(JarvisAction.Cancel);}
  if(normalized==lastCommand&&now-lastTime<2)return new JarvisCommand(JarvisAction.None);
  lastCommand=normalized;lastTime=now;awakeUntil=now+12;
  var command=Parse(normalized,apps);if(command.Action==JarvisAction.StopVoice)Cancel();return command;
 }
 public static string Normalize(string text){var b=new StringBuilder();foreach(char c in text??"")if(!char.IsWhiteSpace(c)&&!char.IsPunctuation(c))b.Append(char.ToLowerInvariant(c));return b.ToString();}
 static JarvisCommand Parse(string s,JarvisApp[] apps){
  foreach(string prefix in new[]{"帮我","给我","麻烦你","麻烦","请你","请"})s=s.Replace(prefix,"");
  if(s.Contains("停止监听")||s.Contains("关闭语音")||s.Contains("关闭麦克风"))return new JarvisCommand(JarvisAction.StopVoice);
  if(s.Contains("眼前有什么")||s=="看一下"||s.Contains("看看前面")||s.Contains("摄像头看到")||s.Contains("你看到了什么"))return new JarvisCommand(JarvisAction.Look,s);
  if(s.Contains("电量")||s.Contains("电池")||s.Contains("多少电")||s.Contains("联网了吗")||s.Contains("网络状态"))return new JarvisCommand(JarvisAction.Status);
  if(s.Contains("软件列表")||s.Contains("应用列表")||s.Contains("查看软件")||s.Contains("查看应用")||(s.Contains("哪些")&&(s.Contains("软件")||s.Contains("应用"))))return new JarvisCommand(JarvisAction.Apps);
  if(s.Contains("wifi")||s.Contains("无线网络")||s.Contains("网络设置")||s.Contains("设置网络")||s.Contains("连接网络"))return new JarvisCommand(JarvisAction.Wifi);
  if(s.Contains("蓝牙"))return new JarvisCommand(JarvisAction.Bluetooth);
  if(s.Contains("打开设置")||s=="设置"||s.Contains("系统设置"))return new JarvisCommand(JarvisAction.Settings);
  if(s.Contains("回主页")||s.Contains("回到主页")||s.Contains("回主界面")||s.Contains("回工作台")||s.Contains("返回主页"))return new JarvisCommand(JarvisAction.Home);
  if(s.Contains("为什么")||s.Contains("怎么")||s.Contains("是什么"))return new JarvisCommand(JarvisAction.AskAi,s);
  if(s.Contains("火狐")){foreach(var app in apps??new JarvisApp[0])if(app.packageName=="org.mozilla.firefox")return new JarvisCommand(JarvisAction.OpenApp,app.packageName);}
  if(s.Contains("浏览器")||s.Contains("b站")||s.Contains("哔哩哔哩"))return new JarvisCommand(JarvisAction.Browser);
  int start=-1;foreach(string verb in new[]{"打开","启动","运行"}){int i=s.IndexOf(verb,StringComparison.Ordinal);if(i>=0){start=i+verb.Length;break;}}
  if(start>=0){string query=s.Substring(start);var matches=new List<JarvisApp>();foreach(var app in apps??new JarvisApp[0]){string label=Normalize(app.label),package=Normalize(app.packageName);if(query.Length>0&&(query==label||query==package))matches.Add(app);}
   if(matches.Count==1)return new JarvisCommand(JarvisAction.OpenApp,matches[0].packageName);
   return matches.Count>1?new JarvisCommand(JarvisAction.Apps,query):new JarvisCommand(JarvisAction.AskAi,s);
  }
  return new JarvisCommand(JarvisAction.AskAi,s);
 }
}
public static class JarvisTools {
 public static bool TryMap(string tool,string argument,JarvisApp[] apps,out JarvisCommand command){
  command=null;if(argument!=null&&argument.Length>2048)return false;
  var map=new Dictionary<string,JarvisAction>{{"show_apps",JarvisAction.Apps},{"show_settings",JarvisAction.Settings},{"show_wifi",JarvisAction.Wifi},{"show_bluetooth",JarvisAction.Bluetooth},{"open_browser",JarvisAction.Browser},{"go_home",JarvisAction.Home},{"get_status",JarvisAction.Status},{"open_app",JarvisAction.OpenApp},{"answer",JarvisAction.AskAi}};
  JarvisAction action;if(tool==null||!map.TryGetValue(tool,out action))return false;
  if(action==JarvisAction.OpenApp){bool found=false;foreach(var app in apps??new JarvisApp[0])if(app.packageName==argument){found=true;break;}if(!found)return false;}
  command=new JarvisCommand(action,argument);return true;
 }
}