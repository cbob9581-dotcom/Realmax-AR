using UnityEngine;
using UnityEngine.UI;
public class WebInputPanel : MonoBehaviour {
 Transform root;Font font;Text display;string text="https://www.bilibili.com/";bool caps;SpatialBrowserView browser;
 public void Open(Transform parent,Font f,SpatialBrowserView view){browser=view;if(root){root.gameObject.SetActive(true);Refresh();return;}font=f;root=new GameObject("WebSpatialKeyboard",typeof(RectTransform)).transform;root.SetParent(parent,false);root.localPosition=new Vector3(0,0,-5);
  var bg=new GameObject("Keyboard background",typeof(RectTransform),typeof(Image),typeof(BoxCollider));bg.transform.SetParent(root,false);bg.GetComponent<RectTransform>().sizeDelta=new Vector2(1320,670);bg.GetComponent<RectTransform>().anchoredPosition=new Vector2(0,20);bg.GetComponent<Image>().color=new Color(.025f,.055f,.09f,1);bg.GetComponent<BoxCollider>().size=new Vector3(1320,670,1);bg.transform.localPosition+=Vector3.forward*2;
  display=Label("",new Vector2(0,280),new Vector2(1200,110),30);
  Key("Open URL",new Vector2(-420,160),new Vector2(360,75),()=>{browser.Load(text);Hide();});Key("Type into page",new Vector2(0,160),new Vector2(360,75),()=>{browser.TypeText(text);Hide();});Key("Close keyboard",new Vector2(420,160),new Vector2(360,75),Hide);
  Key("Bilibili",new Vector2(-420,60),new Vector2(360,65),()=>{browser.Load("https://www.bilibili.com/");Hide();});Key("Back page",new Vector2(0,60),new Vector2(360,65),()=>{browser.Back();Hide();});Key("Clear text",new Vector2(420,60),new Vector2(360,65),()=>{text="";Refresh();});
  string[] rows={"1234567890-_","qwertyuiop./","asdfghjkl:@","zxcvbnm?=+"};for(int row=0;row<rows.Length;row++){var chars=rows[row];for(int col=0;col<chars.Length;col++){char c=chars[col];Key(c.ToString(),new Vector2((col-(chars.Length-1)/2f)*100,-35-row*57),new Vector2(90,48),()=>{if(text.Length<4096)text+=caps?char.ToUpperInvariant(c):c;Refresh();});}}
  Key("Shift",new Vector2(-420,-275),new Vector2(360,55),()=>{caps=!caps;Refresh();});Key("Space",new Vector2(0,-275),new Vector2(360,55),()=>{text+=" ";Refresh();});Key("Delete",new Vector2(420,-275),new Vector2(360,55),()=>{if(text.Length>0)text=text.Substring(0,text.Length-1);Refresh();});Refresh();
 }
 public void Hide(){if(root)root.gameObject.SetActive(false);}
 void Refresh(){if(display)display.text=(caps?"[SHIFT] ":"")+text;}
 Text Label(string value,Vector2 position,Vector2 size,int pixels){var g=new GameObject("Web input label",typeof(RectTransform),typeof(Text));g.transform.SetParent(root,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=position;r.sizeDelta=size;var t=g.GetComponent<Text>();t.text=value;t.font=font;t.fontSize=pixels;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return t;}
 void Key(string value,Vector2 position,Vector2 size,System.Action action){var g=new GameObject("Web input control",typeof(RectTransform),typeof(Image),typeof(BoxCollider),typeof(SpatialAction));g.transform.SetParent(root,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=position;r.sizeDelta=size;var image=g.GetComponent<Image>();image.color=new Color(.08f,.17f,.27f,1);g.GetComponent<BoxCollider>().size=new Vector3(size.x,size.y,8);var a=g.GetComponent<SpatialAction>();a.action=action;a.image=image;Label(value,position,size,23);}
 void OnDestroy(){if(root)Destroy(root.gameObject);}
}
