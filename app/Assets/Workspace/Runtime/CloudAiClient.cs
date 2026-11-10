using System;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
[Serializable] public class AiSettings {public string provider="openai-compatible",endpoint="",model="",apiKey="";}
public class CloudAiClient : MonoBehaviour {
 [Serializable] class Message {public string role="user",content;public Message(string c){content=c;}}
 [Serializable] class ImageUrl {public string url;}
 [Serializable] class ImagePart {public string type,text;public ImageUrl image_url;}
 [Serializable] class ImageMessage {public string role="user";public ImagePart[] content;}
 [Serializable] class ImageRequest {public string model;public int max_tokens=512;public bool stream=false;public ImageMessage[] messages;}
 [Serializable] class ChatRequest {public string model;public Message[] messages;public bool stream=false;}
 [Serializable] class AnthropicRequest {public string model;public int max_tokens=1024;public Message[] messages;}
 [Serializable] class Part {public string text;public Part(string t){text=t;}}
 [Serializable] class Content {public Part[] parts;public Content(string t){parts=new[]{new Part(t)};}}
 [Serializable] class GeminiRequest {public Content[] contents;}
 [Serializable] class Choice {public Message message;}
 [Serializable] class ChatResponse {public Choice[] choices;}
 [Serializable] class AnthropicResponse {public Part[] content;}
 [Serializable] class Candidate {public Content content;}
 [Serializable] class GeminiResponse {public Candidate[] candidates;}
 public AiSettings Settings=new AiSettings();public bool Busy{get;private set;} public event Action<string> Output; public event Action<string,bool> Completed;UnityWebRequest activeRequest;
 void Awake(){Load();}
 void Emit(string s){if(Output!=null)Output(s);}
 void Complete(string text,bool success){Emit(text);if(Completed!=null)Completed(text,success);}
 public void Cancel(){if(activeRequest!=null)activeRequest.Abort();}
 public void Load(){
#if UNITY_ANDROID && !UNITY_EDITOR
  try{using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var a=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var store=new AndroidJavaClass("com.ar.workspace.SecureSettings")){var text=store.CallStatic<string>("load",a);if(!string.IsNullOrEmpty(text))Settings=JsonUtility.FromJson<AiSettings>(text)??new AiSettings();}}
  catch(Exception){Emit("Saved settings could not be read. Re-enter provider settings.");}
#endif
 }
 public bool Save(){
  string error;if(!ProviderRules.Validate(Settings.provider,Settings.endpoint,Settings.model,Settings.apiKey,out error)){Emit(error);return false;}
#if UNITY_ANDROID && !UNITY_EDITOR
  try{using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var a=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var store=new AndroidJavaClass("com.ar.workspace.SecureSettings")){if(!store.CallStatic<bool>("save",a,JsonUtility.ToJson(Settings))){Emit("Settings could not be saved");return false;}}}
  catch(Exception){Emit("Secure settings storage is unavailable");return false;}
#endif
  Emit("Provider settings saved on this headset");return true;
 }
 public void Ask(string prompt){if(Busy)return;string error;if(!ProviderRules.Validate(Settings.provider,Settings.endpoint,Settings.model,Settings.apiKey,out error)){Emit(error);return;}if(string.IsNullOrWhiteSpace(prompt)){Emit("Enter a question first");return;}StartCoroutine(Request(prompt));}
 public void AskImage(string prompt,byte[] jpeg){
  if(Busy)return;string error;
  if(!ProviderRules.Validate(Settings.provider,Settings.endpoint,Settings.model,Settings.apiKey,out error)){Complete(error,false);return;}
  if(Settings.provider!="openai-compatible"){Complete("相机问答目前需要支持图片的 OpenAI 兼容接口。",false);return;}
  if(jpeg==null||jpeg.Length<4||jpeg.Length>2097152||jpeg[0]!=255||jpeg[1]!=216){Complete("照片格式或大小不符合要求。",false);return;}
  var body=new ImageRequest{model=Settings.model,messages=new[]{new ImageMessage{content=new[]{new ImagePart{type="text",text=prompt},new ImagePart{type="image_url",image_url=new ImageUrl{url="data:image/jpeg;base64,"+Convert.ToBase64String(jpeg)}}}}}};
  StartCoroutine(Request(prompt,JsonUtility.ToJson(body)));
 }
 IEnumerator Request(string prompt,string imageJson=null){
  var request=new AiRequestSnapshot(Settings.provider,Settings.endpoint,Settings.model,Settings.apiKey);
  Busy=true;Emit("Requesting cloud AI...");
  string json;
  if(imageJson!=null)json=imageJson;
  else if(request.Provider=="anthropic")json=JsonUtility.ToJson(new AnthropicRequest{model=request.Model,messages=new[]{new Message(prompt)}});
  else if(request.Provider=="gemini")json=JsonUtility.ToJson(new GeminiRequest{contents=new[]{new Content(prompt)}});
  else json=JsonUtility.ToJson(new ChatRequest{model=request.Model,messages=new[]{new Message(prompt)}});
  using(var req=new UnityWebRequest(request.Endpoint,"POST")){
   req.uploadHandler=new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));req.downloadHandler=new DownloadHandlerBuffer();req.timeout=60;req.redirectLimit=0;req.SetRequestHeader("Content-Type","application/json");
   if(request.Provider=="anthropic"){req.SetRequestHeader("x-api-key",request.ApiKey);req.SetRequestHeader("anthropic-version","2023-06-01");}
   else if(request.Provider=="gemini")req.SetRequestHeader("x-goog-api-key",request.ApiKey);
   else req.SetRequestHeader("Authorization","Bearer "+request.ApiKey);
   activeRequest=req;yield return req.SendWebRequest();activeRequest=null;
   Busy=false;Debug.Log("ARWORKSPACE AI HTTP "+req.responseCode);
   if(req.result!=UnityWebRequest.Result.Success){Complete("Cloud request failed (HTTP "+req.responseCode+"). Check endpoint, model, network and account permissions.",false);yield break;}
   try{
    string text="";
    if(request.Provider=="anthropic"){var r=JsonUtility.FromJson<AnthropicResponse>(req.downloadHandler.text);if(r!=null&&r.content!=null)foreach(var p in r.content)text+=p.text;}
    else if(request.Provider=="gemini"){var r=JsonUtility.FromJson<GeminiResponse>(req.downloadHandler.text);if(r!=null&&r.candidates!=null&&r.candidates.Length>0&&r.candidates[0].content!=null)foreach(var p in r.candidates[0].content.parts)text+=p.text;}
    else{var r=JsonUtility.FromJson<ChatResponse>(req.downloadHandler.text);if(r!=null&&r.choices!=null&&r.choices.Length>0&&r.choices[0].message!=null)text=r.choices[0].message.content;}
    Complete(string.IsNullOrEmpty(text)?"The service returned no text for this adapter.":text,!string.IsNullOrEmpty(text));
   }catch(Exception){Complete("The response format does not match the selected provider adapter.",false);}
  }
 }
}
