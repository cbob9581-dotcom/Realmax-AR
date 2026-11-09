using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;
using System;
using System.IO;
public static class ArBuild {
 [MenuItem("REALMAX/Build Workspace APK")]
 public static void Build(){
  ArChecks.Run();
  var localMaven=Path.GetFullPath(Path.Combine(Application.dataPath,"../.dependencies/maven")).Replace('\\','/');
  var gradleTemplate=File.ReadAllText(Path.Combine(EditorApplication.applicationContentsPath,"PlaybackEngines/AndroidPlayer/Tools/GradleTemplates/settingsTemplate.gradle"));
  gradleTemplate=gradleTemplate.Replace("        google()","        maven { url uri('"+localMaven+"') }\n        maven { url 'https://maven.mozilla.org/maven2/' }\n        google()");
  File.WriteAllText("Assets/Plugins/Android/settingsTemplate.gradle",gradleTemplate);

  EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
  PlayerSettings.companyName="ARWorkspace";PlayerSettings.productName="REALMAX Workspace";
  PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.ar.realmaxworkspace");
  PlayerSettings.bundleVersion="0.1.2";PlayerSettings.Android.bundleVersionCode=3;
  PlayerSettings.Android.minSdkVersion=(AndroidSdkVersions)25;PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)35;
  PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARMv7;
  PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.Mono2x);
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,new[]{GraphicsDeviceType.OpenGLES3});
  PlayerSettings.colorSpace=ColorSpace.Gamma;PlayerSettings.MTRendering=true;PlayerSettings.Android.useCustomKeystore=false;
  var p=typeof(PlayerSettings.Android).GetProperty("applicationEntry");if(p!=null)p.SetValue(null,Enum.Parse(p.PropertyType,"Activity"));
  var so=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);var input=so.FindProperty("activeInputHandler");if(input!=null){input.intValue=0;so.ApplyModifiedPropertiesWithoutUndo();}
  if(!AssetDatabase.LoadAssetAtPath<Material>("Assets/Workspace/Resources/BrowserSurface.mat")){Directory.CreateDirectory("Assets/Workspace/Resources");AssetDatabase.CreateAsset(new Material(Shader.Find("Unlit/Texture")),"Assets/Workspace/Resources/BrowserSurface.mat");}
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Realmax/Prefab/SvrCamera.prefab");
  if(!prefab)throw new Exception("Factory stereo camera prefab missing");
  var rig=(GameObject)PrefabUtility.InstantiatePrefab(prefab);var m=rig.GetComponentInChildren<SvrManager>();
  if(!m||!m.leftCamera||!m.rightCamera)throw new Exception("Both factory eye cameras are required");
  m.settings.trackPosition=true;m.settings.poseStatusFade=false;m.settings.displayType=SvrManager.SvrSettings.eEyeBufferType.StereoSeperate;
  m.leftCamera.cullingMask=~0;m.rightCamera.cullingMask=~0;
  m.leftCamera.clearFlags=CameraClearFlags.SolidColor;m.rightCamera.clearFlags=CameraClearFlags.SolidColor;m.leftCamera.backgroundColor=Color.black;m.rightCamera.backgroundColor=Color.black;
  new GameObject("WorkspaceApp").AddComponent<ArWorkspace>();
  Directory.CreateDirectory("Assets/Workspace/Scenes");var path="Assets/Workspace/Scenes/Workspace.unity";EditorSceneManager.SaveScene(scene,path);AssetDatabase.SaveAssets();
  Directory.CreateDirectory("Builds");var report=BuildPipeline.BuildPlayer(new[]{path},"Builds/RealmaxWorkspace.apk",BuildTarget.Android,BuildOptions.None);
  Debug.Log("ARWORKSPACE BUILD "+report.summary.result+" errors="+report.summary.totalErrors);
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Android build failed");
 }
}
