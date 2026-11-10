using System;
using UnityEngine;
public static class ArChecks {
 static void CheckPlacement(){
  var type=typeof(ArWorkspace).Assembly.GetType("WorkspacePlacement");
  if(type==null)throw new Exception("Panel placement has no tracking-ready gate");
  var ready=type.GetMethod("TrackingReady");
  if((bool)ready.Invoke(null,new object[]{true,0})||(bool)ready.Invoke(null,new object[]{false,3})||!(bool)ready.Invoke(null,new object[]{true,3}))throw new Exception("Panel placed before tracking orientation was ready");
  var view=new GameObject("placement-test-view");
  try{
   view.transform.SetPositionAndRotation(new Vector3(10,2,-7),Quaternion.Euler(22,137,0));
   var pose=(Pose)type.GetMethod("InFront").Invoke(null,new object[]{view.transform,2.5f});
   if(Vector3.Distance(pose.position,view.transform.position+view.transform.forward*2.5f)>.001f)throw new Exception("Panel not centered in translated/rotated view");
   if(Vector3.Dot(pose.rotation*Vector3.forward,view.transform.forward)<.999f)throw new Exception("Panel not facing current view");
  }finally{UnityEngine.Object.DestroyImmediate(view);}
  Debug.Log("ARWORKSPACE TEST PASS: tracking readiness and translated/rotated panel placement");
 }
 static void CheckPointerAndRecovery(){
  var method=typeof(BrowserSurface).GetMethod("TryMapRay");
  if(method==null)throw new Exception("Browser cannot preserve which pointer triggered it");
  var go=GameObject.CreatePrimitive(PrimitiveType.Quad);
  try{
   go.transform.position=new Vector3(0,0,2);var surface=go.AddComponent<BrowserSurface>();Physics.SyncTransforms();
   foreach(float x in new[]{-.25f,.25f}){object[] args={new Ray(new Vector3(x,0,0),Vector3.forward),1280,600,0,0};
    if(!(bool)method.Invoke(surface,args)||Math.Abs((int)args[3]-(x<0?320:960))>1||Math.Abs((int)args[4]-300)>1)throw new Exception("Pointer source did not reach its own web coordinate");}
  }finally{UnityEngine.Object.DestroyImmediate(go);}
  var recover=typeof(WorkspacePlacement).GetMethod("NeedsRecovery");
  if(recover==null)throw new Exception("Panel can remain outside field of view after tracking moves");
  var view=new GameObject("recovery-test");
  try{
   if((bool)recover.Invoke(null,new object[]{view.transform,new Vector3(0,0,2.5f)})||!(bool)recover.Invoke(null,new object[]{view.transform,new Vector3(0,0,-2.5f)})||!(bool)recover.Invoke(null,new object[]{view.transform,new Vector3(0,0,20)}))throw new Exception("Off-screen/drift recovery gate is incorrect");
  }finally{UnityEngine.Object.DestroyImmediate(view);}
  Debug.Log("ARWORKSPACE TEST PASS: distinct gaze/hand coordinates and out-of-view recovery");
 }
 static void CheckDwell(){
  var t=typeof(ArWorkspace).Assembly.GetType("GazeDwellState");if(t==null)throw new Exception("Gaze repeats clicks instead of one-shot dwell");
  var state=Activator.CreateInstance(t);var step=t.GetMethod("Step");int fired=0;
  for(int i=0;i<50;i++)if((bool)step.Invoke(state,new object[]{7,Vector3.forward,.1f}))fired++;
  if(fired!=1)throw new Exception("Stable gaze must fire once, not repeatedly");
  if((bool)step.Invoke(state,new object[]{8,Vector3.forward,.1f}))throw new Exception("New target inherited dwell time");
  for(int i=0;i<20;i++)if((bool)step.Invoke(state,new object[]{8,Vector3.forward,.1f}))fired++;
  if(fired!=1)throw new Exception("A refreshed screen re-clicked under a stationary gaze");
  var moved=Quaternion.Euler(0,8,0)*Vector3.forward;
  for(int i=0;i<20;i++)if((bool)step.Invoke(state,new object[]{8,moved,.1f}))fired++;
  if(fired!=2)throw new Exception("Moving gaze did not re-arm selection");
  step.Invoke(state,new object[]{0,Vector3.forward,.1f});
  if((bool)step.Invoke(state,new object[]{8,Vector3.forward,.1f}))throw new Exception("Lost target retained dwell");
  Debug.Log("ARWORKSPACE TEST PASS: stable gaze is one-shot and target loss resets timing");
 }
 static void CheckReviewRegressions(){
  var go=new GameObject("navigation-test");
  try{var workspace=go.AddComponent<ArWorkspace>();var agent=go.AddComponent<JarvisAgent>();var field=typeof(JarvisAgent).GetField("generation",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);field.SetValue(agent,7);workspace.HideContent();if((int)field.GetValue(agent)==7)throw new Exception("Manual navigation can still execute stale cloud plans");}finally{UnityEngine.Object.DestroyImmediate(go);}
  var layout=typeof(ArWorkspace).Assembly.GetType("SpatialKeyboardLayout");if(layout==null)throw new Exception("Manual Wi-Fi keyboard lacks full password symbols");var chars=new System.Collections.Generic.HashSet<char>();chars.Add(' ');foreach(bool symbols in new[]{false,true})foreach(string row in (string[])layout.GetMethod("Rows").Invoke(null,new object[]{symbols}))foreach(char c in row){chars.Add(c);chars.Add(char.ToUpperInvariant(c));}for(int i=32;i<=126;i++)if(!chars.Contains((char)i))throw new Exception("Manual password keyboard cannot enter ASCII "+i);
  Debug.Log("ARWORKSPACE TEST PASS: manual navigation invalidates plans and password symbols are complete");
 }
 static void CheckTransparentModal(){
  var root=new GameObject("modal-validation",typeof(RectTransform));var page=new GameObject("underlying-page",typeof(RectTransform),typeof(CanvasGroup));page.transform.SetParent(root.transform,false);page.GetComponent<CanvasGroup>().alpha=.7f;
  var header=new GameObject("Persistent navigation and real telemetry",typeof(RectTransform));header.transform.SetParent(root.transform,false);var veil=new HudModalVeil();
  try{veil.HideOthers(root.transform);if(page.GetComponent<CanvasGroup>().alpha!=0||!header.activeSelf)throw new Exception("Transparent keyboard did not isolate underlying content");page.SetActive(false);veil.Restore();if(page.activeSelf||Math.Abs(page.GetComponent<CanvasGroup>().alpha-.7f)>.001f)throw new Exception("Closing keyboard resurrected a hidden page");}
  finally{veil.Restore();UnityEngine.Object.DestroyImmediate(root);}
  Debug.Log("ARWORKSPACE TEST PASS: transparent keyboard hides only drawing and never resurrects pages");
 }
 public static void Run(){
  CheckTransparentModal();
  CheckReviewRegressions();
  CheckDwell();
  CheckPointerAndRecovery();
  CheckPlacement();
  string captured="";Application.LogCallback handler=(message,trace,type)=>{captured+=message;};Application.logMessageReceived+=handler;
  var go=new GameObject("SECRET_CANARY_sk_test_123");
  try{var action=go.AddComponent<SpatialAction>();action.OnTrigger();if(captured.Contains("SECRET_CANARY"))throw new Exception("Sensitive UI names leaked to logcat");}
  finally{Application.logMessageReceived-=handler;UnityEngine.Object.DestroyImmediate(go);}
  Debug.Log("ARWORKSPACE TEST PASS: input labels are not logged");
 }
}
