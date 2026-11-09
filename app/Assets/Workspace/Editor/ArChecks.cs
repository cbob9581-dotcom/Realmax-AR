using System;
using UnityEngine;
public static class ArChecks {
 public static void Run(){
  string captured="";Application.LogCallback handler=(message,trace,type)=>{captured+=message;};Application.logMessageReceived+=handler;
  var go=new GameObject("SECRET_CANARY_sk_test_123");
  try{var action=go.AddComponent<SpatialAction>();action.OnTrigger();if(captured.Contains("SECRET_CANARY"))throw new Exception("Sensitive UI names leaked to logcat");}
  finally{Application.logMessageReceived-=handler;UnityEngine.Object.DestroyImmediate(go);}
  Debug.Log("ARWORKSPACE TEST PASS: input labels are not logged");
 }
}
