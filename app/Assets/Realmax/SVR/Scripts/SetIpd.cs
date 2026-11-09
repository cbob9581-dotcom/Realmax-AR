using System.Collections;
using UnityEngine;

public class SetIpd : MonoBehaviour {

	private string path = "file:///sdcard/Android/data/com.realmax.launcher/files/ipdinfo";
	void Start()
	{
		StartCoroutine(LoadFile());
	}
	IEnumerator LoadFile()
	{
		WWW www = new WWW(path);
		yield return www;
		if(www != null && string.IsNullOrEmpty(www.error))
		{
			string info = www.text;
			string[] ipdinfo = info.Split('\n');
			float leftx;
			float lefty;
			float rightx;
			float righty;
			leftx = float.Parse(ipdinfo[7]);
			lefty = float.Parse(ipdinfo[8]);
			rightx = float.Parse(ipdinfo[9]);
			righty = float.Parse(ipdinfo[10]);
			SvrPlugin.Instance.setoffset(leftx, lefty, rightx, righty);
		}
	}
}
