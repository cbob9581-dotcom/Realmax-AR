using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
[Serializable] public class JarvisAppCatalog {public JarvisApp[] apps=new JarvisApp[0];}
[Serializable] public class JarvisWifiNetwork {public string ssid;public int level;public bool secured;}
[Serializable] public class JarvisWifiScan {public string message;public JarvisWifiNetwork[] networks=new JarvisWifiNetwork[0];}
public sealed class JarvisPanel {
 readonly JarvisAgent agent;readonly Transform parent;readonly Font font;Transform root;SpatialUi ui;Text status,message,audioStatus;HudGraphic core;HudScrollText document;int page;string screen="home",filter="",ssid="",password="";JarvisWifiScan scan=new JarvisWifiScan();
 public bool Visible{get{return root&&root.gameObject.activeSelf;}}
 public JarvisPanel(JarvisAgent owner,Transform p,Font f){agent=owner;parent=p;font=f;}
 public void Hide(){if(root)root.gameObject.SetActive(false);}
 void Build(){if(root)UnityEngine.Object.Destroy(root.gameObject);root=SpatialUi.Page(parent,"Jarvis spatial panel");ui=new SpatialUi(root,font);ui.Trim();document=null;core=null;status=ui.Label("贾维斯",new Vector2(0,305),new Vector2(1240,60),30);message=ui.Label(agent.Message,new Vector2(0,225),new Vector2(1240,100),25);}
 void Button(string text,int column,int row,Action click){ui.Button(text,new Vector2((column-1)*410,105-row*210),new Vector2(380,90),click);}
 public void ShowHome(){screen="home";page=0;Build();status.rectTransform.anchoredPosition=new Vector2(0,-295);message.rectTransform.anchoredPosition=new Vector2(0,55);message.rectTransform.sizeDelta=new Vector2(1240,65);core=HudGraphic.Create(root,new Vector2(0,200),new Vector2(250,250),HudGraphic.Style.Core);ui.Label("J.A.R.V.I.S.",new Vector2(0,200),new Vector2(400,70),34);ui.Button("开始对话",new Vector2(-420,-70),new Vector2(350,80),agent.AskQuestion);ui.Button("看一眼",new Vector2(0,-70),new Vector2(350,80),()=>agent.Look());ui.Button("软件",new Vector2(420,-70),new Vector2(350,80),agent.ShowApps);ui.Button("浏览器",new Vector2(-420,-140),new Vector2(350,80),agent.OpenBrowser);ui.Button("网络",new Vector2(0,-140),new Vector2(350,80),agent.ShowWifi);ui.Button("设置",new Vector2(420,-140),new Vector2(350,80),agent.ShowSettings);ui.Button("开启语音",new Vector2(-310,-235),new Vector2(550,60),agent.StartVoice);ui.Button("关闭语音",new Vector2(310,-235),new Vector2(550,60),agent.StopVoice);}
 public void ShowApps(string query=""){screen="apps";filter=query??"";page=0;DrawApps();}
 void DrawApps(){Build();var apps=agent.Apps.Where(a=>string.IsNullOrEmpty(filter)||JarvisSession.Normalize(a.label).Contains(JarvisSession.Normalize(filter))).ToArray();page=Mathf.Clamp(page,0,Mathf.Max(0,(apps.Length+1)/2-3));for(int i=0;i<6;i++){int index=page*2+i;if(index>=apps.Length)break;var app=apps[index];ui.Button(app.label,new Vector2(i%2==0?-290:290,90-i/2*100),new Vector2(530,80),()=>agent.OpenApp(app.packageName));}if(apps.Length==0)ui.Label("没有匹配的软件",Vector2.zero,new Vector2(1200,90));ui.Arrow(new Vector2(610,155),-1,()=>{page--;DrawApps();});ui.Arrow(new Vector2(610,-170),1,()=>{page++;DrawApps();});ui.Label(apps.Length+" 个软件 · 上下连续滚动",new Vector2(0,-270),new Vector2(900,55),24);}
 public void ShowSettings(){screen="settings";Build();Button("手动网络设置",0,0,agent.ShowWifi);Button("蓝牙 / 手柄",1,0,agent.ShowBluetooth);Button("声音与音量",2,0,agent.ShowAudio);Button("云端 AI 配置",0,1,agent.OpenAiSettings);Button("面板回正",1,1,agent.Recenter);Button("贾维斯主页",2,1,agent.ShowHome);ui.Button("开启语音",new Vector2(-315,-255),new Vector2(590,70),agent.StartVoice);ui.Button("关闭语音",new Vector2(315,-255),new Vector2(590,70),agent.StopVoice);}
 public void ShowAudio(){screen="audio";Build();audioStatus=ui.Label(agent.AudioStatus(),new Vector2(0,70),new Vector2(1200,150),32);ui.Button("− 音量",new Vector2(-420,-120),new Vector2(350,90),()=>agent.AdjustVolume(-1));ui.Button("静音 / 开启声音",new Vector2(0,-120),new Vector2(430,90),agent.ToggleMute);ui.Button("+ 音量",new Vector2(420,-120),new Vector2(350,90),()=>agent.AdjustVolume(1));ui.Label("调节音量保留当前静音状态，点击中间按钮才切换声音",new Vector2(0,-270),new Vector2(1200,70),24);}
 public void RefreshAudio(){if(screen=="audio"&&audioStatus)audioStatus.text=agent.AudioStatus();}
 public void ShowBluetooth(){screen="bluetooth";Build();ui.Label(agent.BluetoothStatus(),new Vector2(0,80),new Vector2(1200,100));ui.Label("手柄保留原厂蓝牙连接；系统蓝牙页面使用原有显示方式。",new Vector2(0,-40),new Vector2(1200,100),26);ui.Button("打开系统蓝牙页面",new Vector2(-315,-230),new Vector2(590,80),agent.OpenBluetoothSettings);ui.Button("返回设置",new Vector2(315,-230),new Vector2(590,80),agent.ShowSettings);}
 public void ShowWifi(){screen="wifi";page=0;RefreshWifi();}
 void RefreshWifi(){scan=agent.ScanWifi();DrawWifi();}
 void DrawWifi(){Build();var networks=scan.networks??new JarvisWifiNetwork[0];page=Mathf.Clamp(page,0,Mathf.Max(0,networks.Length-5));for(int i=0;i<5;i++){int index=page+i;if(index>=networks.Length)break;var n=networks[index];ui.Button(n.ssid+(n.secured?"  [需密码]":"  [开放]"),new Vector2(-260,125-i*65),new Vector2(730,55),()=>{ssid=n.ssid;password="";agent.SetMessage("已选网络："+ssid+"。手动输入密码，或直接连接已保存网络。");Update("",0);});}
  ui.Button("刷新扫描",new Vector2(445,100),new Vector2(330,70),RefreshWifi);ui.Arrow(new Vector2(445,10),-1,()=>{page--;DrawWifi();});ui.Arrow(new Vector2(445,-60),1,()=>{page++;DrawWifi();});ui.Label(scan.message??"",new Vector2(445,-115),new Vector2(330,150),23);
  ui.Button("手动输入 SSID",new Vector2(-430,-255),new Vector2(380,70),()=>agent.EditText(ssid,false,s=>{ssid=s;agent.SetMessage("已选择网络："+ssid);}));ui.Button("输入密码",new Vector2(0,-255),new Vector2(380,70),()=>agent.EditText("",true,s=>{password=s;agent.SetMessage("网络："+ssid+"，密码已输入。");}));ui.Button("连接",new Vector2(430,-255),new Vector2(380,70),()=>{agent.ConnectWifi(ssid,password);password="";});
 }
 string answer="";
 public void ShowPending(string text){screen="pending";answer="";DrawAnswer();document.SetText(text);}
 public void ShowAnswer(string text){answer=text??"";screen="answer";DrawAnswer();}
 public void RestoreAnswer(){screen="answer";DrawAnswer();}
 public void UpdateAnswer(string text){answer=text??"";if(document){document.SetText(answer,true);return;}DrawAnswer();}
 void DrawAnswer(){if(root)UnityEngine.Object.Destroy(root.gameObject);root=SpatialUi.Page(parent,"Continuous luminous answer");ui=new SpatialUi(root,font);status=message=null;core=null;ui.Trim();ui.Label("J A R V I S",new Vector2(0,310),new Vector2(1000,60),30);document=HudScrollText.Create(root,font,new Vector2(0,0),new Vector2(1190,420),36);document.SetText(answer);ui.Arrow(new Vector2(0,255),-1,()=>document.Scroll(-1));ui.Arrow(new Vector2(0,-255),1,()=>document.Scroll(1));}
 public void Update(string voice,int level){if(status)status.text=string.IsNullOrEmpty(voice)?"贾维斯":voice+(agent.OutputMuted?" · 输出静音":" · 声音已开启");if(message)message.text=agent.Message;if(core)core.MicrophoneLevel=level;}
}
