using UnityEngine;
using UnityEngine.UI;
public class AiSpatialPanel : MonoBehaviour {
 Transform root;Font font;CloudAiClient client;Text input,output,providerLabel;string question="",field="question",reply="Configure a cloud provider, then enter a question.";bool caps;int page;
 public void Open(Transform parent,Font f){if(root){root.gameObject.SetActive(true);return;}font=f;root=new GameObject("AiSpatialPanel",typeof(RectTransform)).transform;root.SetParent(parent,false);root.localPosition=new Vector3(0,0,-3);client=gameObject.AddComponent<CloudAiClient>();client.Output+=OnOutput;
  Label("AI SETTINGS / QUESTION",new Vector2(0,350),new Vector2(1200,55),26);
  Key("Endpoint",new Vector2(-470,295),new Vector2(280,55),()=>Select("endpoint"));Key("Model",new Vector2(-155,295),new Vector2(280,55),()=>Select("model"));Key("API key",new Vector2(155,295),new Vector2(280,55),()=>Select("key"));Key("Question",new Vector2(470,295),new Vector2(280,55),()=>Select("question"));
  input=Label("",new Vector2(0,225),new Vector2(1240,90),24);output=Label(reply,new Vector2(0,120),new Vector2(1240,120),23);
  providerLabel=Key("Provider",new Vector2(-430,20),new Vector2(390,55),CycleProvider);Key("Save settings",new Vector2(0,20),new Vector2(390,55),()=>client.Save());Key("Send question",new Vector2(430,20),new Vector2(390,55),()=>{page=0;client.Ask(question);});
  string[] rows={"1234567890-_","qwertyuiop./","asdfghjkl:@","zxcvbnm?=+"};for(int row=0;row<rows.Length;row++){var chars=rows[row];for(int col=0;col<chars.Length;col++){char character=chars[col];Key(character.ToString(),new Vector2((col-(chars.Length-1)/2f)*100,-55-row*57),new Vector2(90,48),()=>Append(caps?char.ToUpperInvariant(character):character));}}
  Key("Shift",new Vector2(-460,-282),new Vector2(260,44),()=>{caps=!caps;Refresh();});Key("Space",new Vector2(-150,-282),new Vector2(280,44),()=>Append(' '));Key("Delete",new Vector2(170,-282),new Vector2(280,44),Backspace);Key("Clear",new Vector2(480,-282),new Vector2(240,44),()=>SetValue(""));Refresh();
 }
 public void Hide(){if(root)root.gameObject.SetActive(false);}
 public bool IsVisible{get{return root&&root.gameObject.activeSelf;}}
 public void Scroll(int direction){page=Mathf.Max(0,page-direction);ShowReply();}
 void OnOutput(string text){reply=text;page=0;ShowReply();}
 void ShowReply(){if(!output)return;page=WorkspaceRules.ClampPage(page,reply.Length,360);int start=page*360;output.text=reply.Substring(start,Mathf.Min(360,reply.Length-start));}
 void Select(string value){field=value;Refresh();}
 string Value(){if(field=="endpoint")return client.Settings.endpoint;if(field=="model")return client.Settings.model;if(field=="key")return client.Settings.apiKey;return question;}
 void SetValue(string value){if(field=="endpoint")client.Settings.endpoint=value;else if(field=="model")client.Settings.model=value;else if(field=="key")client.Settings.apiKey=value;else question=value;Refresh();}
 void Append(char c){var s=Value();if(s.Length<4096)SetValue(s+c);}
 void Backspace(){var s=Value();if(s.Length>0)SetValue(s.Substring(0,s.Length-1));}
 void CycleProvider(){var p=client.Settings.provider;client.Settings.provider=p=="openai-compatible"?"anthropic":p=="anthropic"?"gemini":"openai-compatible";Refresh();}
 void Refresh(){if(input){var s=Value();input.text=field.ToUpperInvariant()+(caps?" [SHIFT]":"")+": "+(field=="key"?new string('*',Mathf.Min(50,s.Length)):s);}if(providerLabel)providerLabel.text=client.Settings.provider;}
 Text Label(string text,Vector2 pos,Vector2 size,int fontSize){var g=new GameObject("AI Label",typeof(RectTransform),typeof(Text));g.transform.SetParent(root,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;var t=g.GetComponent<Text>();t.font=font;t.fontSize=fontSize;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.text=text;return t;}
 Text Key(string text,Vector2 pos,Vector2 size,System.Action action){var g=new GameObject("AI input control",typeof(RectTransform),typeof(Image),typeof(BoxCollider),typeof(SpatialAction));g.transform.SetParent(root,false);var r=g.GetComponent<RectTransform>();r.anchoredPosition=pos;r.sizeDelta=size;var i=g.GetComponent<Image>();i.color=new Color(.08f,.17f,.27f,.95f);g.GetComponent<BoxCollider>().size=new Vector3(size.x,size.y,8);var a=g.GetComponent<SpatialAction>();a.image=i;a.action=action;return Label(text,pos,size,22);}
 void OnDestroy(){if(client)client.Output-=OnOutput;if(root)Destroy(root.gameObject);}
}
