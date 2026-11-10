using UnityEngine;
using UnityEngine.UI;
public class WebInputPanel : MonoBehaviour {
 readonly HudModalVeil veil=new HudModalVeil();Transform root;Font font;Text display;string text="https://www.bilibili.com/";bool caps;SpatialBrowserView browser;
 public bool IsVisible{get{return root&&root.gameObject.activeSelf;}}
 public void Open(Transform parent,Font f,SpatialBrowserView view){veil.HideOthers(parent,root);browser=view;if(root){root.gameObject.SetActive(true);Refresh();return;}font=f;root=new GameObject("WebSpatialKeyboard",typeof(RectTransform)).transform;root.SetParent(parent,false);root.localPosition=new Vector3(0,0,-12);
  var bg=new GameObject("Keyboard background",typeof(RectTransform),typeof(Image),typeof(BoxCollider));bg.layer=0;bg.transform.SetParent(root,false);bg.GetComponent<RectTransform>().sizeDelta=new Vector2(1320,670);bg.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,20);bg.GetComponent<Image>().color=Color.clear;bg.GetComponent<BoxCollider>().size=new Vector3(1320,670,1);bg.transform.localPosition+=Vector3.forward*2;
  display=Label("",new Vector2(0,280),new Vector2(1200,110),30);
  Key("打开网址",new Vector2(-420,160),new Vector2(360,75),()=>{browser.Load(text);Hide();});Key("输入网页",new Vector2(0,160),new Vector2(360,75),()=>{browser.TypeText(text);Hide();});Key("关闭键盘",new Vector2(420,160),new Vector2(360,75),Hide);
  Key("Bilibili",new Vector2(-420,60),new Vector2(360,65),()=>{browser.Load("https://www.bilibili.com/");Hide();});Key("网页后退",new Vector2(0,60),new Vector2(360,65),()=>{browser.Back();Hide();});Key("清空",new Vector2(420,60),new Vector2(360,65),()=>{text="";Refresh();});
  string[] rows={"1234567890-_","qwertyuiop./","asdfghjkl:@","zxcvbnm?=+"};for(int row=0;row<rows.Length;row++){var chars=rows[row];for(int col=0;col<chars.Length;col++){char c=chars[col];Key(c.ToString(),new Vector2((col-(chars.Length-1)/2f)*100,-35-row*57),new Vector2(90,48),()=>{if(text.Length<4096)text+=caps?char.ToUpperInvariant(c):c;Refresh();});}}
  Key("大小写",new Vector2(-420,-275),new Vector2(360,55),()=>{caps=!caps;Refresh();});Key("空格",new Vector2(0,-275),new Vector2(360,55),()=>{text+=" ";Refresh();});Key("退格",new Vector2(420,-275),new Vector2(360,55),()=>{if(text.Length>0)text=text.Substring(0,text.Length-1);Refresh();});Refresh();
 }
 public void Hide(){veil.Restore();if(root)root.gameObject.SetActive(false);}
 void Refresh(){if(display)display.text=(caps?"[SHIFT] ":"")+text;}
 Text Label(string value,Vector2 position,Vector2 size,int pixels){return new SpatialUi(root,font).Label(value,position,size,pixels);}
 void Key(string value,Vector2 position,Vector2 size,System.Action action){new SpatialUi(root,font).Button(value,position,size,action);}
 void OnDestroy(){veil.Restore();if(root)Destroy(root.gameObject);}
}
