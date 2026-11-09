using UnityEngine;
using UnityEditor;
using System.IO;

public class Builder : MonoBehaviour
{
    static void BuildScene(string[] scenes, string apkDir, string apkName)
    {
        Directory.CreateDirectory(apkDir);

        if(System.IO.File.Exists(apkDir + apkName))
        {
            System.IO.File.SetAttributes(apkDir + apkName, System.IO.File.GetAttributes(apkDir + apkName) & ~FileAttributes.ReadOnly);
        }

        BuildPipeline.BuildPlayer(scenes, apkDir + apkName, BuildTarget.Android, BuildOptions.None);
    }
    //
	[MenuItem( "SVR/Build BoxWorld" )]
	static void BuildBoxWorld( )
	{
		try
		{
            Debug.Log("Bulding BoxWorld!");

            PlayerSettings.productName = "BoxWorld";
            #if UNITY_5_6
            PlayerSettings.applicationIdentifier = "com.qualcomm.svr.boxworld";
            #else
            PlayerSettings.applicationIdentifier = "com.qualcomm.svr.boxworld";
            #endif
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            string apkDir = "./build/android/";
            string apkName = "BoxWorld.apk";
            string[] scenes = {
                "./Assets/boxWorld.unity",
                "./Assets/overlayWorld.unity",
                "./Assets/planeWorld.unity",
                "./Assets/gridWorld.unity",
            };

            BuildScene(scenes, apkDir, apkName);
		}
		catch (IOException e)
		{
			Debug.LogError( e.Message );
		}
	}
}
