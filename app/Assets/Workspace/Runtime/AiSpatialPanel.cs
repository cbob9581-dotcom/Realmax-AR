using UnityEngine;
using UnityEngine.UI;
public class AiSpatialPanel:MonoBehaviour {
 Transform root;Font font;CloudAiClient client;SpatialTextKeyboard keyboard;Text provider,endpoint,model,key,status;
 public bool IsVisible{get{return root&&root.gameObject.activeSelf;}}
 public void Open(Transform parent,Font f){font=f;if(!client)client=gameObject.AddComponent<CloudAiClient>();client.Load();if(!keyboard)keyboard=gameObject.AddComponent<SpatialTextKeyboard>();if(root){root.gameObject.SetActive(true);Refresh();return;}root=SpatialUi.Page(parent,"Transparent cloud AI settings");var ui=new SpatialUi(root,font);ui.Trim();ui.Label("云端 AI 配置",new Vector2(0,305),new Vector2(1200,60),32);provider=ui.Button("",new Vector2(0,210),new Vector2(1180,75),Cycle);endpoint=ui.Button("",new Vector2(0,110),new Vector2(1180,75),()=>Edit("endpoint"));model=ui.Button("",new Vector2(0,10),new Vector2(1180,75),()=>Edit("model"));key=ui.Button("",new Vector2(0,-90),new Vector2(1180,75),()=>Edit("key"));ui.Button("保存配置",new Vector2(-305,-210),new Vector2(550,75),()=>{status.text=client.Save()?"配置已保存到本机":"配置未保存，请检查服务地址、模型和密钥";});ui.Button("输入问题",new Vector2(305,-210),new Vector2(550,75),()=>{if(!client.Save()){status.text="请先完善并保存有效配置";return;}var agent=GetComponent<JarvisAgent>();if(agent)agent.AskQuestion();});status=ui.Label("点击字段手动编辑",new Vector2(0,-300),new Vector2(1180,60),24);Refresh();}
 void Cycle(){var p=client.Settings.provider;client.Settings.provider=p=="openai-compatible"?"anthropic":p=="anthropic"?"gemini":"openai-compatible";Refresh();}
 void Edit(string field){string value=field=="key"?client.Settings.apiKey:field=="model"?client.Settings.model:client.Settings.endpoint;keyboard.Open(root.parent,font,value,field=="key",s=>{if(field=="key")client.Settings.apiKey=s;else if(field=="model")client.Settings.model=s;else client.Settings.endpoint=s;Refresh();});}
 void Refresh(){if(!provider)return;provider.text="服务商   "+client.Settings.provider;endpoint.text="Endpoint   "+client.Settings.endpoint;model.text="Model   "+client.Settings.model;key.text="API key   "+(string.IsNullOrEmpty(client.Settings.apiKey)?"未设置":new string('●',12));}
 public void Hide(){if(keyboard)keyboard.Hide();if(root)root.gameObject.SetActive(false);}
 public void Scroll(int direction){}
 void OnDestroy(){if(root)Destroy(root.gameObject);}
}
