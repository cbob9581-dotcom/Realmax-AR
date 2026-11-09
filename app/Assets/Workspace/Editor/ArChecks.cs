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
 public static void Run(){
  CheckPlacement();
  string captured="";Application.LogCallback handler=(message,trace,type)=>{captured+=message;};Application.logMessageReceived+=handler;
  var go=new GameObject("SECRET_CANARY_sk_test_123");
  try{var action=go.AddComponent<SpatialAction>();action.OnTrigger();if(captured.Contains("SECRET_CANARY"))throw new Exception("Sensitive UI names leaked to logcat");}
  finally{Application.logMessageReceived-=handler;UnityEngine.Object.DestroyImmediate(go);}
  Debug.Log("ARWORKSPACE TEST PASS: input labels are not logged");
 }
}
