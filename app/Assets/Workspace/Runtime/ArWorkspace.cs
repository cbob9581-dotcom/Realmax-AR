using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Realmax;
public class ArWorkspace : MonoBehaviour {
 System.Collections.Generic.List<GameObject> chrome=new System.Collections.Generic.List<GameObject>();
 float recoveryTime; string deviceStatus="Checking network and battery..."; Coroutine recenterRoutine; Text status,body; Transform panel; Font font; int count; float nextLog; SpatialBrowserView browser; AiSpatialPanel ai; WebInputPanel webInput;
 IEnumerator Start(){
  Application.targetFrameRate=60;AudioListener.volume=0;
  font=Resources.Load<Font>("WorkspaceFont");if(!font)font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var go=new GameObject("SpatialWorkspace",typeof(RectTransform),typeof(Canvas));
  go.layer=0;panel=go.transform; var c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.worldCamera=SvrManager.Instance.leftCamera;
  var rect=go.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(1400,1100);panel.localScale=Vector3.one*.0016f;
  panel.position=new Vector3(0,0,2.5f);
  Box("Background",new Vector2(0,0),new Vector2(1400,1100),new Color(.025f,.055f,.09f,.97f));
  Label("REALMAX WORKSPACE",new Vector2(-130,490),new Vector2(1000,80),42);
  var recenter=Box("Recenter",new Vector2(530,490),new Vector2(270,80),new Color(.08f,.17f,.27f,.95f));var rc=recenter.gameObject.AddComponent<BoxCollider>();rc.size=new Vector3(270,80,10);var ra=recenter.gameObject.AddComponent<SpatialAction>();ra.image=recenter;ra.action=QueueRecenter;Label("回正",new Vector2(530,490),new Vector2(260,75),28);
  status=Label("Starting stereo runtime...",new Vector2(0,420),new Vector2(1300,80),27);
  body=Label("用手柄射线指向按钮，再按触摸板。\n也可用头部准星停留约 1.5 秒选择。\n手柄菜单键：把面板召回眼前。",new Vector2(0,75),new Vector2(1250,270),34);
  Button("贾维斯",new Vector2(-420,-350),()=>{var agent=GetComponent<JarvisAgent>();if(agent)agent.ShowHome();});
  Button("浏览器",new Vector2(0,-350),()=>ShowBrowser());
  Button("AI 问答",new Vector2(420,-350),()=>ShowAi());
  Button("键盘 / 回正",new Vector2(0,-490),()=>{if(browser&&browser.IsVisible&&!(ai&&ai.IsVisible)){if(!webInput)webInput=gameObject.AddComponent<WebInputPanel>();webInput.Open(panel,font,browser);}else Reposition();});
  Button("向上滚动",new Vector2(-420,-490),()=>{if(ai&&ai.IsVisible)ai.Scroll(1);else if(browser)browser.Scroll(1);});
  Button("向下滚动",new Vector2(420,-490),()=>{if(ai&&ai.IsVisible)ai.Scroll(-1);else if(browser)browser.Scroll(-1);});
  ControllerKeyEvent.MenuKeyDown.AddListener(QueueRecenter);
  ControllerKeyEvent.ReturnKeyDown.AddListener(QueueRecenter);
  foreach(Transform child in panel)chrome.Add(child.gameObject);
  var jarvis=gameObject.AddComponent<JarvisAgent>();jarvis.Configure(this);
  yield return PlaceWhenTracked();
  Canvas.ForceUpdateCanvases();
  Debug.Log("ARWORKSPACE ready; independent stereo app; speaker muted");
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var intent=activity.Call<AndroidJavaObject>("getIntent")){
   if(intent.Call<string>("getStringExtra","startPage")=="browser")ShowBrowser();else jarvis.ShowHome();
   if(intent.Call<bool>("getBooleanExtra","diagnostics",false))StartCoroutine(CaptureEyes());
  }
#endif
 }
 public Transform Panel{get{return panel;}}
 public Font WorkspaceFont{get{return font;}}
 public void SetImmersiveAnswer(bool answer){foreach(var item in chrome)if(item)item.SetActive(!answer);}
 public void HideContent(){SetImmersiveAnswer(false);if(webInput)webInput.Hide();if(browser)browser.Hide();if(ai)ai.Hide();if(body)body.gameObject.SetActive(false);var agent=GetComponent<JarvisAgent>();if(agent){agent.InvalidatePlan();agent.HidePanel();}}
 public void OpenBrowser(){ShowBrowser();}
 public void OpenAiSettings(){ShowAi();}
 public void RecenterNow(){QueueRecenter();}
 public void CaptureDiagnostics(){StartCoroutine(CaptureEyes());}
 IEnumerator CaptureEyes(){
  yield return new WaitForSecondsRealtime(12);
  yield return new WaitForEndOfFrame();
  var m=SvrManager.Instance;if(!m||!m.status.running)yield break;
  var directory=System.IO.Path.Combine(Application.persistentDataPath,"diagnostics");System.IO.Directory.CreateDirectory(directory);
  CaptureEye(m.leftCamera,"left",directory);CaptureEye(m.rightCamera,"right",directory);
 }
 void CaptureEye(Camera eye,string name,string directory){
  var target=eye?eye.targetTexture:null;if(!target){Debug.LogWarning("ARWORKSPACE missing eye texture "+name);return;}
  var previous=RenderTexture.active;Texture2D image=null;
  try{RenderTexture.active=target;image=new Texture2D(target.width,target.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,target.width,target.height),0,0);image.Apply();System.IO.File.WriteAllBytes(System.IO.Path.Combine(directory,name+"-eye.png"),image.EncodeToPNG());Debug.Log("ARWORKSPACE captured "+name+" eye "+target.width+"x"+target.height);}
  catch(System.Exception e){Debug.LogWarning("ARWORKSPACE eye capture failed "+e.GetType().Name);}
  finally{RenderTexture.active=previous;if(image)Destroy(image);}
 }

 void ShowBrowser(){HideContent();if(!browser)browser=gameObject.AddComponent<SpatialBrowserView>();browser.Open(panel);}
 void ShowAi(){HideContent();if(!ai)ai=gameObject.AddComponent<AiSpatialPanel>();ai.Open(panel,font);}
 void Reposition(){QueueRecenter();}
 void QueueRecenter(){if(!panel)return;if(recenterRoutine!=null)StopCoroutine(recenterRoutine);recenterRoutine=StartCoroutine(PlaceWhenTracked());}
 IEnumerator PlaceWhenTracked(){
  // Factory Return listener recenters native tracking in the same event; wait for the resulting eye pose.
  yield return new WaitForEndOfFrame();yield return new WaitForEndOfFrame();
  int stable=0;
  while(stable<30){var m=SvrManager.Instance;stable=m&&m.leftCamera&&WorkspacePlacement.TrackingReady(m.status.running,m.status.pose)?stable+1:0;yield return new WaitForEndOfFrame();}
  var manager=SvrManager.Instance;var view=manager.leftCamera.transform;
  var pose=WorkspacePlacement.InFront(view,2.5f);
  if(manager.rightCamera)pose.position+=(manager.rightCamera.transform.position-view.position)*.5f;
  panel.SetPositionAndRotation(pose.position,pose.rotation);
  Canvas.ForceUpdateCanvases();
  Debug.Log("ARWORKSPACE panel recentered | view="+view.position+" panel="+panel.position+" angle="+Vector3.Angle(view.forward,panel.position-view.position));
  recoveryTime=0;recenterRoutine=null;
 }
 void OnApplicationPause(bool paused){if(!paused&&panel)QueueRecenter();}
 void OnDestroy(){ControllerKeyEvent.MenuKeyDown.RemoveListener(QueueRecenter);ControllerKeyEvent.ReturnKeyDown.RemoveListener(QueueRecenter);}
 void Update(){
  AudioListener.volume=0;
  var m=SvrManager.Instance;if(!m||!status)return;
  bool connected=ControllerDataCallBack.Instance&&ControllerDataCallBack.Instance.IsConnect;
  string s="SVR "+(m.status.running?"RUNNING":"STARTING")+" | pose "+m.status.pose+" | controller "+(connected?"CONNECTED":"WAITING")+" | stereo L/R";
  if(WorkspacePlacement.TrackingReady(m.status.running,m.status.pose)&&m.leftCamera){
   recoveryTime=WorkspacePlacement.NeedsRecovery(m.leftCamera.transform,panel.position)?recoveryTime+Time.unscaledDeltaTime:0;
   if(recoveryTime>1.25f&&recenterRoutine==null){recoveryTime=0;QueueRecenter();}
  }
  status.text=deviceStatus+"\nController "+(connected?"connected":"waiting")+(browser?"\n"+browser.Status:"");
  if(Time.unscaledTime>nextLog){nextLog=Time.unscaledTime+5;
#if UNITY_ANDROID && !UNITY_EDITOR
   try{using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var info=new AndroidJavaClass("com.ar.workspace.DeviceInfo"))deviceStatus=info.CallStatic<string>("getStatus",activity);}catch(System.Exception){deviceStatus="Network/battery status unavailable";}
#endif
   Debug.Log("ARWORKSPACE "+s+" | L="+(m.leftCamera?m.leftCamera.transform.localPosition.ToString():"missing")+" mask="+m.leftCamera.cullingMask+" panel="+panel.position+" R="+(m.rightCamera?m.rightCamera.transform.localPosition.ToString():"missing"));}
 }
 Image Box(string name,Vector2 pos,Vector2 size,Color color){var g=new GameObject(name,typeof(RectTransform),typeof(Image));g.layer=0;g.transform.SetParent(panel,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;var i=g.GetComponent<Image>();i.color=color;return i;}
 Text Label(string text,Vector2 pos,Vector2 size,int pixels){var g=new GameObject("Label",typeof(RectTransform),typeof(Text));g.layer=0;g.transform.SetParent(panel,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;var t=g.GetComponent<Text>();t.font=font;t.fontSize=pixels;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.text=text;return t;}
 void Button(string name,Vector2 pos,System.Action action){var i=Box(name,pos,new Vector2(380,110),new Color(.08f,.17f,.27f,.95f));var col=i.gameObject.AddComponent<BoxCollider>();col.size=new Vector3(380,110,10);var a=i.gameObject.AddComponent<SpatialAction>();a.image=i;a.action=action;Label(name,pos,new Vector2(370,100),32);}
}
