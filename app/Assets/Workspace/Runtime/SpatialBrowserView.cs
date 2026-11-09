using UnityEngine;
public class SpatialBrowserView : MonoBehaviour {
 public int Width{get{return 1280;}}public int Height{get{return 600;}}
 readonly BrowserPageState state=new BrowserPageState(); public bool IsVisible{get{return state.Active;}}
 AndroidJavaObject engine; Texture2D texture; GameObject surface; float nextFrame; bool firstFrame; public string Status{get;private set;}
 public void Open(Transform root){
  state.Show();if(surface){surface.SetActive(true);ApplyActiveState();return;}
  texture=new Texture2D(Width,Height,TextureFormat.RGBA32,false);texture.wrapMode=TextureWrapMode.Clamp;
  var bg=new Color32[Width*Height];for(int i=0;i<bg.Length;i++)bg[i]=new Color32(12,24,40,255);texture.SetPixels32(bg);texture.Apply();
  surface=GameObject.CreatePrimitive(PrimitiveType.Quad);surface.name="GeckoBrowserSurface";surface.transform.SetParent(root,false);surface.transform.localPosition=new Vector3(0,20,-2);surface.transform.localScale=new Vector3(1280,600,1);
  Destroy(surface.GetComponent<Collider>());surface.AddComponent<MeshCollider>();
  var material=new Material(Resources.Load<Material>("BrowserSurface"));material.mainTexture=texture;material.mainTextureScale=new Vector2(1,-1);material.mainTextureOffset=new Vector2(0,1);surface.GetComponent<Renderer>().material=material;
  surface.AddComponent<BrowserSurface>().browser=this;
#if UNITY_ANDROID && !UNITY_EDITOR
  try{using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer")){var activity=player.GetStatic<AndroidJavaObject>("currentActivity");engine=new AndroidJavaObject("com.ar.workspace.SpatialBrowser",activity,Width,Height);Status="Starting GeckoView";ApplyActiveState();}}
  catch(System.Exception e){Status="Browser unavailable: "+e.GetType().Name;Debug.LogError("ARWORKSPACE "+Status);}
#else
  Status="GeckoView requires the Android device";
#endif
 }
 public void Hide(){state.Hide();if(surface)surface.SetActive(false);ApplyActiveState();}
 void ApplyActiveState(){if(engine!=null)engine.Call("setActive",state.Active);}
 void OnApplicationPause(bool paused){state.Pause(paused);ApplyActiveState();}
 public void Click(int x,int y){if(engine!=null)engine.Call("click",x,y);}
 public void Scroll(int direction){if(engine!=null)engine.Call("scroll",direction);}
 public void TypeText(string text){if(engine!=null)engine.Call("typeText",text);}
 public void Back(){if(engine!=null)engine.Call("back");}
 public void Load(string url){if(engine!=null)engine.Call("load",url);}
 void Update(){if(engine==null||!state.Active||Time.unscaledTime<nextFrame)return;nextFrame=Time.unscaledTime+.12f;try{var bytes=engine.Call<byte[]>("takeFrame");if(bytes!=null&&bytes.Length==Width*Height*4){texture.LoadRawTextureData(bytes);texture.Apply(false,false);if(!firstFrame){firstFrame=true;Debug.Log("ARWORKSPACE first GeckoView texture frame "+Width+"x"+Height);}}Status=engine.Call<string>("getStatus");}catch(System.Exception e){Status="Browser frame error: "+e.GetType().Name;}}
 void OnDestroy(){if(engine!=null){engine.Call("close");engine.Dispose();engine=null;}if(surface){var mat=surface.GetComponent<Renderer>().sharedMaterial;if(mat)Destroy(mat);Destroy(surface);}if(texture)Destroy(texture);}
}
