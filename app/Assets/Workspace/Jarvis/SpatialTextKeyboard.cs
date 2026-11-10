using UnityEngine;
using UnityEngine.UI;
using System;
public class SpatialTextKeyboard:MonoBehaviour {
 readonly HudModalVeil veil=new HudModalVeil();Transform root,keys;Text display,symbolLabel;Font font;string value;bool secret,caps,symbols;Action<string> submit;
 public bool IsVisible{get{return root&&root.gameObject.activeSelf;}}
 public void Open(Transform parent,Font f,string initial,bool mask,Action<string> done){Hide();veil.HideOthers(parent);font=f;value=initial??"";secret=mask;submit=done;caps=symbols=false;root=SpatialUi.Page(parent,"Manual spatial keyboard",-9);var ui=new SpatialUi(root,font);ui.Box(new Vector2(0,0),new Vector2(1320,680),Color.clear,true);display=ui.Label("",new Vector2(0,270),new Vector2(1210,100),30);ui.Button("确认",new Vector2(-310,170),new Vector2(550,75),()=>{var callback=submit;var result=value;Hide();if(callback!=null)callback(result);});ui.Button("取消",new Vector2(310,170),new Vector2(550,75),Hide);
  ui.Button("大小写",new Vector2(-500,-245),new Vector2(225,60),()=>{caps=!caps;DrawKeys();Refresh();});symbolLabel=ui.Button("符号",new Vector2(-250,-245),new Vector2(225,60),()=>{symbols=!symbols;DrawKeys();Refresh();});ui.Button("空格",new Vector2(0,-245),new Vector2(225,60),()=>{if(value.Length<256)value+=" ";Refresh();});ui.Button("退格",new Vector2(250,-245),new Vector2(225,60),()=>{if(value.Length>0)value=value.Substring(0,value.Length-1);Refresh();});ui.Button("清空",new Vector2(500,-245),new Vector2(225,60),()=>{value="";Refresh();});DrawKeys();Refresh();
 }
 void DrawKeys(){if(keys){keys.gameObject.SetActive(false);Destroy(keys.gameObject);}keys=SpatialUi.Page(root,"Keyboard keys",0);var ui=new SpatialUi(keys,font);var rows=SpatialKeyboardLayout.Rows(symbols);for(int row=0;row<rows.Length;row++){string chars=rows[row];for(int col=0;col<chars.Length;col++){char character=chars[col];char shown=caps?char.ToUpperInvariant(character):character;ui.Button(shown.ToString(),new Vector2((col-(chars.Length-1)/2f)*100,55-row*65),new Vector2(90,55),()=>{if(value.Length<256)value+=caps?char.ToUpperInvariant(character):character;Refresh();});}}}
 void Refresh(){if(display)display.text=(caps?"[大写] ":"")+(secret?new string('●',Mathf.Min(48,value.Length)):value);if(symbolLabel)symbolLabel.text=symbols?"字母":"符号";}
 public void Hide(){veil.Restore();if(root){root.gameObject.SetActive(false);Destroy(root.gameObject);}root=keys=null;display=symbolLabel=null;value="";submit=null;}
 void OnDestroy(){Hide();}
}