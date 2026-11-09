using UnityEngine;

namespace Realmax
{
	public class AndroidSystemTools
	{
		private static AndroidSystemTools mInstance;
		private AndroidJavaObject ajo;

		private AndroidSystemTools()
		{
			ajo = new AndroidJavaObject("com.realm.istarcontrolunity.ConnectionIStar");
		}

		public static AndroidSystemTools getInstance()
		{
			if(mInstance == null)
			{
				mInstance = new AndroidSystemTools();
			}
			return mInstance;
		}
		public float getPitch()
		{
			return ajo.Call<float>("getPitch");
		}

		public float getRoll()
		{
			return ajo.Call<float>("getRoll");
		}

		public float getYaw()
		{
			return ajo.Call<float>("getYaw");

		}
		public float getTouchX()
		{

			return ajo.Call<float>("getTouchX");
		}
		public float getTouchY()
		{
			return ajo.Call<float>("getTouchY");
		}

		public int getIStarBatteryLevel()
		{
			return ajo.Call<int>("getIStarBatteryLevel");
		}

		public void connectIStarService()
		{
			ajo.Call("connectIStarService");
		}

		public void freeMemory()
		{
			ajo.Call("freeMemory");
		}
	}
}
