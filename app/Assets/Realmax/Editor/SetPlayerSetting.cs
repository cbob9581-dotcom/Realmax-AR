using UnityEngine;
using UnityEditor;

namespace Realmax
{
	public class SetPlayerSetting : MonoBehaviour
	{

		[MenuItem("Tools/SetPlayerSettings")]
		static void SetPlayerSettings()
		{
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
			PlayerSettings.companyName = "RMSR";
			PlayerSettings.productName = "RealmaxSdk";
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.RMSR.RealmaxSdk");
			PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
			PlayerSettings.Android.forceSDCardPermission = true;
			PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;

		}

	}
}
