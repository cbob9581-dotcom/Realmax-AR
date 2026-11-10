using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public static class HudDiagnostics {
 static void Check(bool condition,string name){if(!condition)throw new Exception("HUD SELFTEST FAIL: "+name);Debug.Log("HUD SELFTEST PASS: "+name);}
 static Text Find(ArWorkspace w,string text){foreach(var label in w.Panel.GetComponentsInChildren<Text>())if(label.text==text)return label;return null;}
 public static IEnumerator Run(ArWorkspace w){
  bool passed=false;string directory=Path.Combine(Application.persistentDataPath,"diagnostics/hud");Directory.CreateDirectory(directory);string result=Path.Combine(directory,"result.txt");File.WriteAllText(result,"RUNNING");
  try{
   w.Navigate(WorkspacePage.Home);yield return new WaitForSecondsRealtime(1);
   foreach(var image in w.Panel.GetComponentsInChildren<Image>())Check(image.color.a==0,"transparent menu surface");
   Check(Find(w,"‹ 返回")&&Find(w,"⌂ 主页")&&Find(w,"回正"),"persistent exits present");
   HudGraphic core=null;foreach(var graphic in w.Panel.GetComponentsInChildren<HudGraphic>())if(graphic.style==HudGraphic.Style.Core)core=graphic;
   Check(core,"animated core exists");float before=core.AnimationPhase;yield return w.CaptureHudFrame("home-a");yield return new WaitForSecondsRealtime(.8f);Check(core.AnimationPhase>before,"animated light phase advances");yield return w.CaptureHudFrame("home-b");
   w.Navigate(WorkspacePage.Settings);w.Navigate(WorkspacePage.Wifi);yield return new WaitForSecondsRealtime(.4f);yield return w.CaptureHudFrame("wifi");w.Back();Check(w.CurrentPage==WorkspacePage.Settings,"network back to settings");w.Back();Check(w.CurrentPage==WorkspacePage.Home,"settings back home");
   w.Navigate(WorkspacePage.Settings);w.Navigate(WorkspacePage.Audio);yield return new WaitForSecondsRealtime(.3f);yield return w.CaptureHudFrame("audio");Check(Find(w,"− 音量")&&Find(w,"+ 音量")&&Find(w,"静音 / 开启声音"),"real volume controls present");Check(w.OutputMuted,"diagnostic output stays muted");w.Back();Check(w.CurrentPage==WorkspacePage.Settings,"audio back settings");w.HomeAndRecenter();
   w.Navigate(WorkspacePage.Apps);yield return new WaitForSecondsRealtime(.4f);yield return w.CaptureHudFrame("apps");w.Back();Check(w.CurrentPage==WorkspacePage.Home,"app list back home");
   w.Navigate(WorkspacePage.Ai);yield return new WaitForSecondsRealtime(.3f);yield return w.CaptureHudFrame("ai-settings");var agent=w.GetComponent<JarvisAgent>();agent.EditText("",false,s=>{});yield return new WaitForSecondsRealtime(.2f);yield return w.CaptureHudFrame("keyboard");w.Back();Check(w.CurrentPage==WorkspacePage.Ai,"keyboard closes before parent page");foreach(var keyboard in w.GetComponents<SpatialTextKeyboard>())Check(!keyboard.IsVisible,"keyboard hidden after back");w.Back();Check(w.CurrentPage==WorkspacePage.Home,"AI settings back home");
   w.OpenBrowser();yield return new WaitForSecondsRealtime(2);w.OpenBrowserInput();var web=w.GetComponent<WebInputPanel>();yield return new WaitForSecondsRealtime(.2f);w.Back();Check(w.CurrentPage==WorkspacePage.Browser&&!web.IsVisible,"web keyboard closes before browser");w.Back();Check(w.CurrentPage==WorkspacePage.Home,"browser workspace exit");
   w.BeginPending();w.HideContent();w.Back();Check(w.CurrentPage==WorkspacePage.Home,"pending return restores original page");
   var page=SpatialUi.Page(w.Panel,"Diagnostic scroll fixture");var scroll=HudScrollText.Create(page,w.WorkspaceFont,Vector2.zero,new Vector2(1100,360),36);var text=new StringBuilder();for(int i=0;i<35;i++)text.Append("连续滚动验收第 ").Append(i+1).Append(" 行，文字发光且保持清晰。\n");scroll.SetText(text.ToString());Check(scroll.CanScrollDown,"long answer can scroll down");scroll.Scroll(1);yield return new WaitForSecondsRealtime(.5f);Check(scroll.CanScrollUp,"long answer can scroll up");yield return w.CaptureHudFrame("scroll");page.gameObject.SetActive(false);UnityEngine.Object.Destroy(page.gameObject);
   w.Navigate(WorkspacePage.Home);yield return new WaitForSecondsRealtime(.3f);yield return w.CaptureHudFrame("home-final");Check(w.OutputMuted,"test completed silently");File.WriteAllText(result,"PASS: dynamic HUD, transparent surfaces, persistent navigation, nested Back, both keyboard exits, browser exit, real audio controls and continuous scroll.");passed=true;Debug.Log("HUD SELFTEST COMPLETE PASS");
  }finally{Realmax.HeadController.DiagnosticHold=false;if(!passed)File.WriteAllText(result,"FAIL: inspect HUD SELFTEST log");}
 }
}
