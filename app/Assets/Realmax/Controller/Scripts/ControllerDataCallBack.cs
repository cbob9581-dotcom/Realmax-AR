using UnityEngine;

namespace Realmax
{
	public class ControllerDataCallBack : Singleton<ControllerDataCallBack>
	{

		private int[] intCount = new int[30];
		private bool[] IsChange = new bool[15];
		private string keyCodeTemp;
		private AndroidSystemTools AndroidSystemTools;
		private float pitch;
		public float Pitch
		{
			get
			{
				return pitch;
			}
		}
		private float yaw;
		public float Yaw
		{
			get
			{
				return yaw;
			}
		}
		private float roll;
		public float Roll
		{
			get
			{
				return roll;
			}
		}
		private float touchPadX;
		public float TouchPadX
		{
			get
			{
				return touchPadX;
			}
		}
		private float touchPadY;
		public float TouchPadY
		{
			get
			{
				return touchPadY;
			}
		}
		private bool isConnect = false;
		public bool IsConnect
		{
			get
			{
				return isConnect;
			}
		}
		void Start()
		{

			this.name = "simple_istar";
			AndroidSystemTools = AndroidSystemTools.getInstance();
			AndroidSystemTools.connectIStarService();
		}

		void LateUpdate()
		{
			if(IsConnect == true)
			{
				roll = AndroidSystemTools.getRoll();
				yaw = AndroidSystemTools.getYaw();
				pitch = AndroidSystemTools.getPitch();
				touchPadX = AndroidSystemTools.getTouchX();
				touchPadY = AndroidSystemTools.getTouchY();
			}
		}

		public void onIStarConnected(string boolConnected)
		{

			if(boolConnected.Equals("true"))
			{
				isConnect = true;
			}
			else if(boolConnected.Equals("false"))
			{
				isConnect = false;
			}
		}

		private void OnApplicationQuit()
		{
			AndroidSystemTools.freeMemory();
		}

		public void onKeyEvent(string keyCode)
		{

			keyCodeTemp = keyCode;
			OnMenuKeyPress();
			OnMenuKeyDown();
			OnMenuKeyUp();
			OnReturnKeyPress();
			OnReturnKeyDown();
			OnReturnKeyUp();
			OnTouchPadKeyPress();
			OnTouchPadKeyDown();
			OnTouchPadKeyUp();
			OnIncreaseVoloumeKeyPress();
			OnIncreaseVoloumeKeyDown();
			OnIncreaseVoloumeKeyUp();
			OnReduceVoloumeKeyPress();
			OnReduceVoloumeKeyDown();
			OnReduceVoloumeKeyUp();
		}


		private void OnMenuKeyPress()
		{
			if(keyCodeTemp == "4")
			{
				ControllerKeyEvent.MenuKeyPress.Invoke();
			}
		}
		private void OnReturnKeyPress()
		{
			if(keyCodeTemp == "2")
			{
				ControllerKeyEvent.ReturnKeyPress.Invoke();
			}
		}
		private void OnTouchPadKeyPress()
		{
			if(keyCodeTemp == "1")
			{
				ControllerKeyEvent.TouchPadKeyPress.Invoke();
			}
		}
		private void OnIncreaseVoloumeKeyPress()
		{
			if(keyCodeTemp == "10")
			{
				ControllerKeyEvent.IncreaseVolumeKeyPress.Invoke();
			}
		}
		private void OnReduceVoloumeKeyPress()
		{
			if(keyCodeTemp == "8")
			{
				ControllerKeyEvent.ReduceVolumeKeyPress.Invoke();
			}
		}

		private void OnMenuKeyDown()
		{
			intCount[2] = intCount[0];
			if(keyCodeTemp == "4")
			{
				intCount[0]++;
				if(IsChange[0])
				{
					IsChange[0] = false;
					ControllerKeyEvent.MenuKeyDown.Invoke();
				}
			}
			intCount[1] = intCount[0];
			if(intCount[1] == intCount[2])
			{
				IsChange[0] = true;
			}
		}
		private void OnReturnKeyDown()
		{
			intCount[8] = intCount[6];
			if(keyCodeTemp == "2")
			{
				intCount[6]++;
				if(IsChange[3])
				{
					IsChange[3] = false;
					ControllerKeyEvent.ReturnKeyDown.Invoke();
				}
			}
			intCount[7] = intCount[6];
			if(intCount[7] == intCount[8])
			{
				IsChange[3] = true;
			}
		}
		private void OnTouchPadKeyDown()
		{
			intCount[14] = intCount[12];
			if(keyCodeTemp == "1")
			{
				intCount[12]++;
				if(IsChange[6])
				{
					IsChange[6] = false;
					ControllerKeyEvent.TouchPadKeyDown.Invoke();

				}
			}
			intCount[13] = intCount[12];
			if(intCount[13] == intCount[14])
			{
				IsChange[6] = true;
			}
		}
		private void OnIncreaseVoloumeKeyDown()
		{
			intCount[20] = intCount[18];
			if(keyCodeTemp == "10")
			{
				intCount[18]++;
				if(IsChange[9])
				{
					IsChange[9] = false;
					ControllerKeyEvent.IncreaseVollumeKeyDown.Invoke();
				}
			}
			intCount[19] = intCount[18];
			if(intCount[19] == intCount[20])
			{
				IsChange[9] = true;
			}
		}
		private void OnReduceVoloumeKeyDown()
		{
			intCount[26] = intCount[24];
			if(keyCodeTemp == "8")
			{
				intCount[24]++;
				if(IsChange[12])
				{
					IsChange[12] = false;
					ControllerKeyEvent.ReduceVoloumeKeyDown.Invoke();
				}
			}
			intCount[25] = intCount[24];
			if(intCount[25] == intCount[26])
			{
				IsChange[12] = true;
			}
		}
		private void OnMenuKeyUp()
		{
			intCount[5] = intCount[3];
			if(keyCodeTemp == "4")
			{
				intCount[3]++;
				if(IsChange[1] && !IsChange[2])
				{
					IsChange[1] = false;
					IsChange[2] = true;
				}
			}
			intCount[4] = intCount[3];
			if(intCount[4] == intCount[5])
			{
				if(IsChange[2])
				{
					ControllerKeyEvent.MenuKeyUp.Invoke();
				}
				IsChange[1] = true;
				IsChange[2] = false;
			}
		}
		private void OnReturnKeyUp()
		{
			intCount[11] = intCount[9];
			if(keyCodeTemp == "2")
			{
				intCount[9]++;
				if(IsChange[4] && !IsChange[5])
				{
					IsChange[4] = false;
					IsChange[5] = true;
				}
			}
			intCount[10] = intCount[9];
			if(intCount[10] == intCount[11])
			{
				if(IsChange[5])
				{
					ControllerKeyEvent.ReturnKeyUp.Invoke();
				}
				IsChange[4] = true;
				IsChange[5] = false;
			}
		}
		private void OnTouchPadKeyUp()
		{
			intCount[17] = intCount[15];
			if(keyCodeTemp == "1")
			{
				intCount[15]++;
				if(IsChange[7] && !IsChange[8])
				{
					IsChange[7] = false;
					IsChange[8] = true;
				}
			}
			intCount[16] = intCount[15];
			if(intCount[16] == intCount[17])
			{
				if(IsChange[8])
				{
					ControllerKeyEvent.TouchPadKeyUp.Invoke();
				}
				IsChange[7] = true;
				IsChange[8] = false;
			}
		}
		private void OnIncreaseVoloumeKeyUp()
		{
			intCount[23] = intCount[21];
			if(keyCodeTemp == "10")
			{
				intCount[21]++;
				if(IsChange[10] && !IsChange[11])
				{
					IsChange[10] = false;
					IsChange[11] = true;
				}
			}
			intCount[22] = intCount[21];
			if(intCount[22] == intCount[23])
			{
				if(IsChange[11])
				{
					ControllerKeyEvent.IncreaseVolumeKeyUp.Invoke();
				}
				IsChange[10] = true;
				IsChange[11] = false;
			}
		}
		private void OnReduceVoloumeKeyUp()
		{
			intCount[29] = intCount[27];
			if(keyCodeTemp == "8")
			{
				intCount[27]++;
				if(IsChange[13] && !IsChange[14])
				{
					IsChange[13] = false;
					IsChange[14] = true;
				}
			}
			intCount[28] = intCount[27];
			if(intCount[28] == intCount[29])
			{
				if(IsChange[14])
				{
					ControllerKeyEvent.ReduceVolumeKeyUp.Invoke();
				}
				IsChange[13] = true;
				IsChange[14] = false;
			}
		}
	}
}
