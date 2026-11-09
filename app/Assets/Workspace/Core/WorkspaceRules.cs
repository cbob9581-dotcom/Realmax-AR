using System;
public static class WorkspaceRules {
 public static string ResolveEndpoint(string provider,string endpoint,string model){
  if(provider!="gemini")return endpoint;
  var uri=new UriBuilder(endpoint);string path=uri.Path.TrimEnd('/');int index=path.IndexOf("/models/",StringComparison.Ordinal);
  if(index>=0)path=path.Substring(0,index);else if(path.EndsWith("/models",StringComparison.Ordinal))path=path.Substring(0,path.Length-7);
  if(model.StartsWith("models/",StringComparison.Ordinal))model=model.Substring(7);
  uri.Path=path+"/models/"+Uri.EscapeDataString(model)+":generateContent";return uri.Uri.AbsoluteUri;
 }
 public static int ClampPage(int page,int length,int size){if(size<=0)throw new ArgumentOutOfRangeException("size");return Math.Max(0,Math.Min(page,Math.Max(0,length-1)/size));}
}
public sealed class BrowserPageState {
 public bool Visible{get;private set;}public bool Paused{get;private set;}
 public bool Active{get{return Visible&&!Paused;}}
 public void Show(){Visible=true;}public void Hide(){Visible=false;}public void Pause(bool value){Paused=value;}
}
public sealed class AiRequestSnapshot {
 public readonly string Provider,Endpoint,Model,ApiKey;
 public AiRequestSnapshot(string provider,string endpoint,string model,string key){Provider=provider;Model=model;ApiKey=key;Endpoint=WorkspaceRules.ResolveEndpoint(provider,endpoint,model);}
}
