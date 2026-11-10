using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
[Serializable] public class JarvisAppCatalog {public JarvisApp[] apps=new JarvisApp[0];}
[Serializable] public class JarvisWifiNetwork {public string ssid;public int level;public bool secured;}
[Serializable] public class JarvisWifiScan {public string message;public JarvisWifiNetwork[] networks=new JarvisWifiNetwork[0];}
public sealed class JarvisPanel {
 readonly JarvisAgent agent;readonly Transform parent;readonly Font font;Transform root;SpatialUi ui;Text status,message;int page;string screen="home",filter="",ssid="",password="";JarvisWifiScan scan=new JarvisWifiScan();
 public bool Visible{get{return root&&root.gameObject.activeSelf;}}
 public JarvisPanel(JarvisAgent owner,Transform p,Font f){agent=owner;parent=p;font=f;}
 public void Hide(){if(root)root.gameObject.SetActive(false);}
 void Build(){if(root)UnityEngine.Object.Destroy(root.gameObject);root=SpatialUi.Page(parent,"Jarvis spatial panel");ui=new SpatialUi(root,font);ui.Box(new Vector2(0,10),new Vector2(1320,680),new Color(.025f,.055f,.09f,1),true);status=ui.Label("贾维斯",new Vector2(0,305),new Vector2(1240,60),30);message=ui.Label(agent.Message,new Vector2(0,225),new Vector2(1240,100),25);}
 void Button(string text,int column,int row,Action click){ui.Button(text,new Vector2((column-1)*410,105-row*210),new Vector2(380,90),click);}
 public void ShowHome(){screen="home";page=0;Build();Button("软件列表",0,0,agent.ShowApps);Button("浏览器",1,0,agent.OpenBrowser);Button("设置",2,0,agent.ShowSettings);Button("网络设置",0,1,agent.ShowWifi);Button("电量 / 网络",1,1,agent.ShowStatus);Button("云端 AI 配置",2,1,agent.OpenAiSettings);ui.Button("拍照问答 · 看一下",new Vector2(0,-190),new Vector2(590,65),()=>agent.Look());ui.Button("开启 / 重新启动语音",new Vector2(-315,-255),new Vector2(590,70),agent.StartVoice);ui.Button("关闭语音",new Vector2(315,-255),new Vector2(590,70),agent.StopVoice);}
 public void ShowApps(string query=""){screen="apps";filter=query??"";page=0;DrawApps();}
 void DrawApps(){Build();var apps=agent.Apps.Where(a=>string.IsNullOrEmpty(filter)||JarvisSession.Normalize(a.label).Contains(JarvisSession.Normalize(filter))).ToArray();int pages=Mathf.Max(1,(apps.Length+5)/6);page=Mathf.Clamp(page,0,pages-1);for(int i=0;i<6;i++){int index=page*6+i;if(index>=apps.Length)break;var app=apps[index];ui.Button(app.label,new Vector2(i%2==0?-315:315,100-i/2*110),new Vector2(590,90),()=>agent.OpenApp(app.packageName));}if(apps.Length==0)ui.Label("没有匹配的软件；可返回完整软件列表",new Vector2(0,40),new Vector2(1200,90));ui.Button("上一页",new Vector2(-430,-255),new Vector2(380,70),()=>{page--;DrawApps();});ui.Button((page+1)+" / "+pages+" · 全部软件",new Vector2(0,-255),new Vector2(380,70),()=>ShowApps());ui.Button("下一页",new Vector2(430,-255),new Vector2(380,70),()=>{page++;DrawApps();});}
 public void ShowSettings(){screen="settings";Build();Button("手动网络设置",0,0,agent.ShowWifi);Button("蓝牙 / 手柄",1,0,agent.ShowBluetooth);Button("电量 / 网络",2,0,agent.ShowStatus);Button("云端 AI 配置",0,1,agent.OpenAiSettings);Button("面板回正",1,1,agent.Recenter);Button("贾维斯主页",2,1,agent.ShowHome);ui.Button("开启语音",new Vector2(-315,-255),new Vector2(590,70),agent.StartVoice);ui.Button("关闭语音",new Vector2(315,-255),new Vector2(590,70),agent.StopVoice);}
 public void ShowBluetooth(){screen="bluetooth";Build();ui.Label(agent.BluetoothStatus(),new Vector2(0,80),new Vector2(1200,100));ui.Label("手柄保留原厂蓝牙连接；系统蓝牙页面使用原有显示方式。",new Vector2(0,-40),new Vector2(1200,100),26);ui.Button("打开系统蓝牙页面",new Vector2(-315,-230),new Vector2(590,80),agent.OpenBluetoothSettings);ui.Button("返回设置",new Vector2(315,-230),new Vector2(590,80),agent.ShowSettings);}
 public void ShowWifi(){screen="wifi";page=0;RefreshWifi();}
 void RefreshWifi(){scan=agent.ScanWifi();DrawWifi();}
 void DrawWifi(){Build();var networks=scan.networks??new JarvisWifiNetwork[0];int pages=Mathf.Max(1,(networks.Length+4)/5);page=Mathf.Clamp(page,0,pages-1);for(int i=0;i<5;i++){int index=page*5+i;if(index>=networks.Length)break;var n=networks[index];ui.Button(n.ssid+(n.secured?"  [需密码]":"  [开放]"),new Vector2(-260,125-i*65),new Vector2(730,55),()=>{ssid=n.ssid;password="";agent.SetMessage("已选网络："+ssid+"。手动输入密码，或直接连接已保存网络。");Update("",0);});}
  ui.Button("刷新扫描",new Vector2(445,100),new Vector2(330,70),RefreshWifi);ui.Button("网络下一页",new Vector2(445,0),new Vector2(330,70),()=>{page++;if(page>=pages)page=0;DrawWifi();});ui.Label(scan.message??"",new Vector2(445,-115),new Vector2(330,150),23);
  ui.Button("手动输入 SSID",new Vector2(-430,-255),new Vector2(380,70),()=>agent.EditText(ssid,false,s=>{ssid=s;agent.SetMessage("已选择网络："+ssid);}));ui.Button("输入密码",new Vector2(0,-255),new Vector2(380,70),()=>agent.EditText("",true,s=>{password=s;agent.SetMessage("网络："+ssid+"，密码已输入。");}));ui.Button("连接",new Vector2(430,-255),new Vector2(380,70),()=>{agent.ConnectWifi(ssid,password);password="";});
 }
 public void ShowPending(string text){if(root)UnityEngine.Object.Destroy(root.gameObject);root=SpatialUi.Page(parent,"Jarvis transparent pending");ui=new SpatialUi(root,font);status=null;message=null;var label=ui.Label(text,new Vector2(0,0),new Vector2(1260,300),36);label.color=SpatialUi.TechBlue;message=label;ui.Button("取消并返回",new Vector2(-500,-255),new Vector2(320,70),agent.ShowHome,true);}
 string answer="";
 public void ShowAnswer(string text){screen="answer";answer=text??"";page=0;DrawAnswer();}
 void DrawAnswer(){
  if(root)UnityEngine.Object.Destroy(root.gameObject);root=SpatialUi.Page(parent,"Jarvis transparent blue answer");ui=new SpatialUi(root,font);status=null;message=null;
  var title=ui.Label("J A R V I S",new Vector2(0,300),new Vector2(1200,60),30);title.color=SpatialUi.TechBlue;
  page=WorkspaceRules.ClampPage(page,answer.Length,220);int start=page*220;
  var text=ui.Label(answer.Substring(start,Mathf.Min(220,answer.Length-start)),new Vector2(0,40),new Vector2(1260,420),36);text.color=SpatialUi.TechBlue;
  ui.Button("〈 上一页",new Vector2(-430,-255),new Vector2(380,70),()=>{page--;DrawAnswer();},true);
  ui.Button("返回贾维斯",new Vector2(0,-255),new Vector2(380,70),agent.ShowHome,true);
  ui.Button("下一页 〉",new Vector2(430,-255),new Vector2(380,70),()=>{page++;DrawAnswer();},true);
 }
 public void Update(string voice,int level){if(status)status.text=string.IsNullOrEmpty(voice)?"贾维斯":voice+"  ·  麦克风 "+level;if(message)message.text=agent.Message;}
}