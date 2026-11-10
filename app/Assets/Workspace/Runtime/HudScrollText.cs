using System;
using System.Collections.Generic;
using System.Text;
#if !PURE_TESTS
using UnityEngine;
using UnityEngine.UI;
#endif

public sealed class HudScrollDocument {
 public readonly List<string> Lines=new List<string>();public float Offset;
 public void Set(string text,int columns){Set(text,s=>s.Length<=Math.Max(1,columns));}
 public void SetMeasured(string text,float width,Func<string,float> measure){
  Lines.Clear();var row=new StringBuilder();float used=0;text=(text??"").Replace("\r\n","\n").Replace('\r','\n');var elements=System.Globalization.StringInfo.GetTextElementEnumerator(text);
  while(elements.MoveNext()){string token=(string)elements.Current;if(token=="\n"){Lines.Add(row.ToString());row.Length=0;used=0;continue;}float advance=measure(token);if(row.Length>0&&used+advance>width){Lines.Add(row.ToString());row.Length=0;used=0;}row.Append(token);used+=advance;}Lines.Add(row.ToString());
 }
 public void Set(string text,Func<string,bool> fits){
  Lines.Clear();var row=new StringBuilder();text=(text??"").Replace("\r\n","\n").Replace('\r','\n');
  // Text elements keep surrogate pairs and combining marks together.
  var elements=System.Globalization.StringInfo.GetTextElementEnumerator(text);
  while(elements.MoveNext()){var token=(string)elements.Current;if(token=="\n"){Lines.Add(row.ToString());row.Length=0;continue;}if(row.Length>0&&!fits(row.ToString()+token)){Lines.Add(row.ToString());row.Length=0;}row.Append(token);}
  Lines.Add(row.ToString());
 }
}
#if !PURE_TESTS
public sealed class HudScrollText : MonoBehaviour {
 readonly HudScrollDocument document=new HudScrollDocument();readonly List<Text> labels=new List<Text>();readonly Dictionary<string,float> advances=new Dictionary<string,float>();Text label;RectTransform viewport;float target,lineHeight;int first=-1;string value="";bool following=true;
 float Maximum{get{return Mathf.Max(0,document.Lines.Count*lineHeight-viewport.rect.height);}}
 public bool CanScrollUp{get{return target>.5f;}}
 public bool CanScrollDown{get{return target<Maximum-.5f;}}
 public static HudScrollText Create(Transform parent,Font font,Vector2 position,Vector2 size,int fontSize=36){
  var go=new GameObject("Answer scroll",typeof(RectTransform),typeof(RectMask2D),typeof(HudScrollText));go.transform.SetParent(parent,false);var view=go.GetComponent<RectTransform>();view.anchoredPosition=position;view.sizeDelta=size;
  var control=go.GetComponent<HudScrollText>();control.viewport=view;control.lineHeight=(float)font.lineHeight/font.fontSize*fontSize*1.3f;
  // Keep the visible label small enough for HudGlow's repeated mesh copies.
  control.label=new SpatialUi(go.transform,font).Label("",Vector2.zero,size,fontSize);control.label.alignment=TextAnchor.UpperLeft;control.label.supportRichText=false;control.label.horizontalOverflow=HorizontalWrapMode.Overflow;control.label.lineSpacing=1.3f;
  control.labels.Add(control.label);return control;
 }
 public void SetText(string text,bool followTail=false){
  text=text??"";bool atBottom=target>=Maximum-.5f;bool append=text.StartsWith(value,StringComparison.Ordinal);value=text;
  var settings=label.GetGenerationSettings(new Vector2(viewport.rect.width,100000));settings.generateOutOfBounds=true;settings.horizontalOverflow=HorizontalWrapMode.Overflow;
  var generator=new TextGenerator();document.SetMeasured(text,viewport.rect.width-4,s=>{float advance;if(!advances.TryGetValue(s,out advance)){advance=generator.GetPreferredWidth(s,settings)/label.pixelsPerUnit;if(advances.Count<8192)advances[s]=advance;}return advance;});
  // Following requires both caller opt-in and an existing bottom position. Scrolling up cancels it.
  if(!append){document.Offset=target=0;following=true;}if(followTail&&following&&atBottom)target=Maximum;else target=Mathf.Clamp(target,0,Maximum);
  document.Offset=Mathf.Clamp(document.Offset,0,Maximum);first=-1;Render();
 }
 public void Scroll(int direction){if(direction==0)return;following=false;target=Mathf.Clamp(target+Math.Sign(direction)*viewport.rect.height*.65f,0,Maximum);if(target>=Maximum-.5f)following=true;}
 void Update(){document.Offset=Mathf.MoveTowards(document.Offset,target,Mathf.Max(lineHeight*8,Mathf.Abs(target-document.Offset)*10)*Time.unscaledDeltaTime);Render();}
 void Render(){
  int start=Mathf.Max(0,Mathf.FloorToInt(document.Offset/lineHeight));int rows=Mathf.CeilToInt(viewport.rect.height/lineHeight)+2;
  while(labels.Count<rows){var row=new SpatialUi(transform,label.font).Label("",Vector2.zero,Vector2.zero,label.fontSize);row.alignment=TextAnchor.UpperLeft;row.supportRichText=false;row.horizontalOverflow=HorizontalWrapMode.Overflow;labels.Add(row);}
  for(int i=0;i<labels.Count;i++){var row=labels[i];bool visible=i<rows&&start+i<document.Lines.Count;row.gameObject.SetActive(visible);if(!visible)continue;if(first!=start)row.text=document.Lines[start+i];var rect=row.rectTransform;rect.anchorMin=rect.anchorMax=rect.pivot=new Vector2(0,1);rect.sizeDelta=new Vector2(viewport.rect.width,lineHeight*1.5f);rect.anchoredPosition=new Vector2(0,document.Offset-(start+i)*lineHeight);}
  first=start;
 }
}
#endif
