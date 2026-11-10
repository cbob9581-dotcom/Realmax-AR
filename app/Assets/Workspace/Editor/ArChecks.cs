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
 public static void Run(){
  CheckPointerAndRecovery();
  CheckPlacement();
  string captured="";Application.LogCallback handler=(message,trace,type)=>{captured+=message;};Application.logMessageReceived+=handler;
  var go=new GameObject("SECRET_CANARY_sk_test_123");
  try{var action=go.AddComponent<SpatialAction>();action.OnTrigger();if(captured.Contains("SECRET_CANARY"))throw new Exception("Sensitive UI names leaked to logcat");}
  finally{Application.logMessageReceived-=handler;UnityEngine.Object.DestroyImmediate(go);}
  Debug.Log("ARWORKSPACE TEST PASS: input labels are not logged");
 }
}
