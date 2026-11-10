using System;
using UnityEngine;
public static class AndroidAgentBridge {
 public static void Call(string className,string method,params object[] args){
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var native=new AndroidJavaClass(className))native.CallStatic(method,args);
#endif
 }
 public static T Call<T>(string className,string method,params object[] args){
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var native=new AndroidJavaClass(className))return native.CallStatic<T>(method,args);
#else
  return default(T);
#endif
 }
 public static T WithActivity<T>(string className,string method,params object[] args){
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var native=new AndroidJavaClass(className)){var all=new object[args.Length+1];all[0]=activity;Array.Copy(args,0,all,1,args.Length);return native.CallStatic<T>(method,all);}
#else
  return default(T);
#endif
 }
 public static void WithActivity(string className,string method,params object[] args){
#if UNITY_ANDROID && !UNITY_EDITOR
  using(var player=new AndroidJavaClass("com.unity3d.player.UnityPlayer"))using(var activity=player.GetStatic<AndroidJavaObject>("currentActivity"))using(var native=new AndroidJavaClass(className)){var all=new object[args.Length+1];all[0]=activity;Array.Copy(args,0,all,1,args.Length);native.CallStatic(method,all);}
#endif
 }
}