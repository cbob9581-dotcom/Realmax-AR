using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Realmax;
public class ArWorkspace : MonoBehaviour {
 readonly WorkspaceNavigation navigation=new WorkspaceNavigation();
 bool outputMuted=true;public bool OutputMuted{get{return outputMuted;}}
 bool restoring; float recoveryTime; string deviceStatus="正在读取电量与网络…"; Coroutine recenterRoutine; Text status; Transform panel; Font font; float nextLog; SpatialBrowserView browser; AiSpatialPanel ai; WebInputPanel webInput;
 public WorkspacePage CurrentPage{get{return navigation.Current;}}
 IEnumerator Start(){
  bool hudSelfTest=false;
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var intent=activity.Call<AndroidJavaObject>("getIntent"))hudSelfTest=intent.Call<bool>("getBooleanExtra","hudSelfTest",false);
#endif
  if(hudSelfTest)HeadController.DiagnosticHold=true;
  Application.targetFrameRate=60;AudioListener.volume=0;
  font=Resources.Load<Font>("WorkspaceFont");if(!font)font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var go=new GameObject("SpatialWorkspace",typeof(RectTransform),typeof(Canvas));panel=go.transform;var c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;c.worldCamera=SvrManager.Instance.leftCamera;
  go.GetComponent<RectTransform>().sizeDelta=new Vector2(1400,1100);panel.localScale=Vector3.one*.0016f;panel.position=new Vector3(0,0,2.5f);
  var header=SpatialUi.Page(panel,"Persistent navigation and real telemetry",-20);var ui=new SpatialUi(header,font);
  ui.Button("‹ 返回",new Vector2(-550,470),new Vector2(220,70),Back);ui.Button("⌂ 主页",new Vector2(-300,470),new Vector2(220,70),HomeAndRecenter);
  ui.Label("REALMAX / JARVIS",new Vector2(100,470),new Vector2(520,65),30);ui.Button("回正",new Vector2(550,470),new Vector2(220,70),QueueRecenter);
  status=ui.Label(deviceStatus,new Vector2(0,395),new Vector2(1320,75),23);
  ControllerKeyEvent.MenuKeyDown.AddListener(HomeAndRecenter);ControllerKeyEvent.ReturnKeyDown.AddListener(Back);
  var jarvis=gameObject.AddComponent<JarvisAgent>();jarvis.Configure(this);jarvis.ShowHome();
  yield return PlaceWhenTracked();if(hudSelfTest)StartCoroutine(HudDiagnostics.Run(this));Canvas.ForceUpdateCanvases();Debug.Log("ARWORKSPACE ready; independent stereo app; speaker muted");
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var intent=activity.Call<AndroidJavaObject>("getIntent")){
   if(intent.Call<string>("getStringExtra","startPage")=="browser")OpenBrowser();
   if(intent.Call<bool>("getBooleanExtra","diagnostics",false))StartCoroutine(CaptureEyes());
  }
#endif
 }
 public void Navigate(WorkspacePage page){if(!restoring)navigation.Open(page);Render(page);}
 void Render(WorkspacePage page){HideContent();var agent=GetComponent<JarvisAgent>();if(page==WorkspacePage.Browser){if(!browser)browser=gameObject.AddComponent<SpatialBrowserView>();browser.Open(panel);var controls=SpatialUi.Page(panel,"Browser workspace controls",-6);browserControls=controls;var ui=new SpatialUi(controls,font);ui.Button("网页后退",new Vector2(-430,-355),new Vector2(300,60),browser.Back);ui.Button("网址 / 输入",new Vector2(0,-355),new Vector2(300,60),OpenBrowserInput);ui.Arrow(new Vector2(520,220),-1,()=>browser.Scroll(1));ui.Arrow(new Vector2(520,-220),1,()=>browser.Scroll(-1));}else if(page==WorkspacePage.Ai){if(!ai)ai=gameObject.AddComponent<AiSpatialPanel>();ai.Open(panel,font);}else if(agent)agent.RenderPage(page);}
 Transform browserControls;
 public void BeginPending(){navigation.Open(WorkspacePage.Pending);}
 public void CompleteAnswer(){if(navigation.Current==WorkspacePage.Pending)navigation.Replace(WorkspacePage.Answer);}
 public void Back(){foreach(var keyboard in GetComponents<SpatialTextKeyboard>())if(keyboard.IsVisible){keyboard.Hide();return;}if(webInput&&webInput.IsVisible){webInput.Hide();return;}var page=navigation.Back();restoring=true;Render(page);restoring=false;QueueRecenter();}
 public void HomeAndRecenter(){Navigate(WorkspacePage.Home);QueueRecenter();}
 public Transform Panel{get{return panel;}}
 public Font WorkspaceFont{get{return font;}}
 public void SetImmersiveAnswer(bool answer){}
 public void HideContent(){if(browserControls){browserControls.gameObject.SetActive(false);Destroy(browserControls.gameObject);browserControls=null;}if(webInput)webInput.Hide();if(browser)browser.Hide();if(ai)ai.Hide();var agent=GetComponent<JarvisAgent>();if(agent){agent.InvalidatePlan();agent.HidePanel();}}
 public void OpenBrowserInput(){if(!browser||!browser.IsVisible)return;if(!webInput)webInput=gameObject.AddComponent<WebInputPanel>();webInput.Open(panel,font,browser);}
 public void OpenBrowser(){Navigate(WorkspacePage.Browser);}
 public void OpenAiSettings(){Navigate(WorkspacePage.Ai);}
 public void RefreshAudio(){
#if UNITY_ANDROID && !UNITY_EDITOR
  try{outputMuted=AndroidAgentBridge.WithActivity<bool>("com.ar.workspace.DeviceInfo","isOutputMuted");}catch(System.Exception){outputMuted=true;}
#endif
  AudioListener.volume=outputMuted?0:1;
 }
 public void RecenterNow(){QueueRecenter();}
 public IEnumerator CaptureHudFrame(string phase){yield return new WaitForEndOfFrame();var m=SvrManager.Instance;if(!m||!m.status.running)yield break;var directory=System.IO.Path.Combine(Application.persistentDataPath,"diagnostics/hud");System.IO.Directory.CreateDirectory(directory);CaptureEye(m.leftCamera,phase+"-left",directory);CaptureEye(m.rightCamera,phase+"-right",directory);}
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
 void OnDestroy(){ControllerKeyEvent.MenuKeyDown.RemoveListener(HomeAndRecenter);ControllerKeyEvent.ReturnKeyDown.RemoveListener(Back);}
 void Update(){
  AudioListener.volume=outputMuted?0:1;if(Input.GetKeyDown(KeyCode.Escape))Back();
  var m=SvrManager.Instance;if(!m||!status)return;
  bool connected=ControllerDataCallBack.Instance&&ControllerDataCallBack.Instance.IsConnect;
  string s="SVR "+(m.status.running?"RUNNING":"STARTING")+" | pose "+m.status.pose+" | controller "+(connected?"CONNECTED":"WAITING")+" | stereo L/R";
  if(WorkspacePlacement.TrackingReady(m.status.running,m.status.pose)&&m.leftCamera){
   recoveryTime=WorkspacePlacement.NeedsRecovery(m.leftCamera.transform,panel.position)?recoveryTime+Time.unscaledDeltaTime:0;
   if(recoveryTime>1.25f&&recenterRoutine==null){recoveryTime=0;QueueRecenter();}
  }
  status.text=deviceStatus+"  ·  手柄 "+(connected?"已连接":"等待连接")+(outputMuted?"  ·  输出静音":"  ·  声音已开启");
  if(Time.unscaledTime>nextLog){nextLog=Time.unscaledTime+5;
#if UNITY_ANDROID && !UNITY_EDITOR
   try{using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var info=new AndroidJavaClass("com.ar.workspace.DeviceInfo")){deviceStatus=info.CallStatic<string>("getStatus",activity);outputMuted=info.CallStatic<bool>("isOutputMuted",activity);AudioListener.volume=outputMuted?0:1;}}catch(System.Exception){deviceStatus="Network/battery status unavailable";}
#endif
   Debug.Log("ARWORKSPACE "+s+" | L="+(m.leftCamera?m.leftCamera.transform.localPosition.ToString():"missing")+" mask="+m.leftCamera.cullingMask+" panel="+panel.position+" R="+(m.rightCamera?m.rightCamera.transform.localPosition.ToString():"missing"));}
 }
}
