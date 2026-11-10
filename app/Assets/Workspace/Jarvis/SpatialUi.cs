using UnityEngine;
using UnityEngine.UI;
using System;
public sealed class SpatialUi {
 public static Color TechBlue{get{return new Color(.32f,.78f,1f,.98f);}}
 public readonly Transform Root;readonly Font font;
 public SpatialUi(Transform root,Font f){Root=root;font=f;}
 public Image Box(Vector2 position,Vector2 size,Color color,bool block=false){var go=new GameObject("Spatial panel",typeof(RectTransform),typeof(Image));go.layer=0;go.transform.SetParent(Root,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=position;r.sizeDelta=size;var image=go.GetComponent<Image>();image.color=color;if(block){var c=go.AddComponent<BoxCollider>();c.size=new Vector3(size.x,size.y,2);go.transform.localPosition+=Vector3.forward*2;}return image;}
 public Text Label(string value,Vector2 position,Vector2 size,int pixels=28){var go=new GameObject("Spatial label",typeof(RectTransform),typeof(Text));go.layer=0;go.transform.SetParent(Root,false);var r=go.GetComponent<RectTransform>();r.anchoredPosition=position;r.sizeDelta=size;var text=go.GetComponent<Text>();text.font=font;text.fontSize=pixels;text.alignment=TextAnchor.MiddleCenter;text.color=TechBlue;text.supportRichText=false;text.raycastTarget=false;go.AddComponent<HudGlow>();text.text=value;text.horizontalOverflow=HorizontalWrapMode.Wrap;text.verticalOverflow=VerticalWrapMode.Overflow;return text;}
 public Text Button(string value,Vector2 position,Vector2 size,Action action,bool textOnly=false){var image=Box(position,size,Color.clear);var c=image.gameObject.AddComponent<BoxCollider>();c.size=new Vector3(size.x,size.y,8);var click=image.gameObject.AddComponent<SpatialAction>();click.image=image;click.textOnly=true;click.action=action;if(!textOnly)click.hud=HudGraphic.Create(Root,position,size,HudGraphic.Style.Button);var label=Label(value,position,size,28);click.caption=label;return label;}
 public void Arrow(Vector2 position,int direction,Action action){var label=Button("",position,new Vector2(120,60),action,true);HudGraphic.Create(Root,position,new Vector2(80,40),direction<0?HudGraphic.Style.Up:HudGraphic.Style.Down);label.gameObject.name=direction<0?"向上滚动":"向下滚动";}
 public void Trim(){HudGraphic.Create(Root,new Vector2(0,0),new Vector2(1320,720),HudGraphic.Style.Page);}
 public static Transform Page(Transform parent,string name,float z=-4){var go=new GameObject(name,typeof(RectTransform));go.transform.SetParent(parent,false);go.transform.localPosition=new Vector3(0,0,z);return go.transform;}
}
public static class SpatialKeyboardLayout {
 public static string[] Rows(bool symbols){return symbols?new[]{"!@#$%^&*()_+","[]{}<>?/\\|~`",";:'\",.-=","1234567890"}:new[]{"1234567890-_","qwertyuiop./","asdfghjkl:@","zxcvbnm?=+"};}
}
