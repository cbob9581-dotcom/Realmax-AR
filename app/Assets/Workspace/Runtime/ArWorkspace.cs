using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Realmax;
public class ArWorkspace : MonoBehaviour {
 Text status,body; Transform panel; Font font; int count; float nextLog; SpatialBrowserView browser; AiSpatialPanel ai; WebInputPanel webInput;
 IEnumerator Start(){
  Application.targetFrameRate=60;AudioListener.volume=0;
  font=Resources.Load<Font>("WorkspaceFont");if(!font)font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var go=new GameObject("SpatialWorkspace",typeof(RectTransform),typeof(Canvas));
  panel=go.transform; var c=go.GetComponent<Canvas>();c.renderMode=RenderMode.WorldSpace;
  var rect=go.GetComponent<RectTransform>();rect.sizeDelta=new Vector2(1400,1100);panel.localScale=Vector3.one*.0016f;
  panel.position=new Vector3(0,0,2.5f);
  Box("Background",new Vector2(0,0),new Vector2(1400,1100),new Color(.025f,.055f,.09f,.97f));
  Label("REALMAX WORKSPACE",new Vector2(0,490),new Vector2(1300,80),46);
  status=Label("Starting stereo runtime...",new Vector2(0,420),new Vector2(1300,80),27);
  body=Label("Spatial display + factory controller\nPoint at a button and press the touchpad.\nHead gaze can also activate a button.",new Vector2(0,75),new Vector2(1250,270),34);
  Button("Home",new Vector2(-420,-350),()=>{count++;if(browser)browser.Hide();if(ai)ai.Hide();if(webInput)webInput.Hide();body.gameObject.SetActive(true);body.text="REALMAX spatial workspace\nBrowser and AI run on this headset.\nController actions: "+count;});
  Button("Browser",new Vector2(0,-350),()=>ShowBrowser());
  Button("AI",new Vector2(420,-350),()=>ShowAi());
  Button("Keyboard / Recenter",new Vector2(0,-490),()=>{if(browser&&browser.IsVisible&&!(ai&&ai.IsVisible)){if(!webInput)webInput=gameObject.AddComponent<WebInputPanel>();webInput.Open(panel,font,browser);}else Reposition();});
  Button("Scroll up",new Vector2(-420,-490),()=>{if(ai&&ai.IsVisible)ai.Scroll(1);else if(browser)browser.Scroll(1);});
  Button("Scroll down",new Vector2(420,-490),()=>{if(ai&&ai.IsVisible)ai.Scroll(-1);else if(browser)browser.Scroll(-1);});
  yield return new WaitForSecondsRealtime(2);Reposition();
  Debug.Log("ARWORKSPACE ready; independent stereo app; speaker muted");
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var intent=activity.Call<AndroidJavaObject>("getIntent")){
   if(intent.Call<string>("getStringExtra","startPage")=="browser")ShowBrowser();
   if(intent.Call<bool>("getBooleanExtra","diagnostics",false))StartCoroutine(CaptureEyes());
  }
#endif
 }
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

 void ShowBrowser(){if(webInput)webInput.Hide();if(ai)ai.Hide();body.gameObject.SetActive(false);if(!browser)browser=gameObject.AddComponent<SpatialBrowserView>();browser.Open(panel);}
 void ShowAi(){if(webInput)webInput.Hide();if(browser)browser.Hide();body.gameObject.SetActive(false);if(!ai)ai=gameObject.AddComponent<AiSpatialPanel>();ai.Open(panel,font);}
 void Reposition(){var m=SvrManager.Instance;if(m&&m.head){var f=m.head.forward;panel.position=m.head.position+f*2.5f;panel.rotation=Quaternion.LookRotation(f,Vector3.up);}}
 void Update(){
  AudioListener.volume=0;
  var m=SvrManager.Instance;if(!m||!status)return;
  bool connected=ControllerDataCallBack.Instance&&ControllerDataCallBack.Instance.IsConnect;
  string s="SVR "+(m.status.running?"RUNNING":"STARTING")+" | pose "+m.status.pose+" | controller "+(connected?"CONNECTED":"WAITING")+" | stereo L/R";
  status.text=s+(browser?"\n"+browser.Status:"");
  if(Time.unscaledTime>nextLog){nextLog=Time.unscaledTime+5;Debug.Log("ARWORKSPACE "+s+" | L="+(m.leftCamera?m.leftCamera.transform.localPosition.ToString():"missing")+" R="+(m.rightCamera?m.rightCamera.transform.localPosition.ToString():"missing"));}
 }
 Image Box(string name,Vector2 pos,Vector2 size,Color color){var g=new GameObject(name,typeof(RectTransform),typeof(Image));g.transform.SetParent(panel,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;var i=g.GetComponent<Image>();i.color=color;return i;}
 Text Label(string text,Vector2 pos,Vector2 size,int pixels){var g=new GameObject("Label",typeof(RectTransform),typeof(Text));g.transform.SetParent(panel,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;var t=g.GetComponent<Text>();t.font=font;t.fontSize=pixels;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.text=text;return t;}
 void Button(string name,Vector2 pos,System.Action action){var i=Box(name,pos,new Vector2(380,110),new Color(.08f,.17f,.27f,.95f));var col=i.gameObject.AddComponent<BoxCollider>();col.size=new Vector3(380,110,10);var a=i.gameObject.AddComponent<SpatialAction>();a.image=i;a.action=action;Label(name,pos,new Vector2(370,100),32);}
}
