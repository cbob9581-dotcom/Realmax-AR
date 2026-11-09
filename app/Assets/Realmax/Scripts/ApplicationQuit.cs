using UnityEngine;

namespace Realmax
{

	public class ApplicationQuit : MonoBehaviour
	{
		private float Timer = 0;
		private float Interval = 3.0f;
		private bool IsDone = false;
		void Update()
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				if(!IsDone)
				{
					Timer += Time.deltaTime;
					if(Timer > Interval)
					{
						Timer = 0;
						IsDone = true;
						Quit();
					}
				}
			}

			if(Input.GetKeyUp(KeyCode.Escape))
			{
				IsDone = false;
			}
		}
		private void Quit()
		{
			Application.Quit();
		}
	}
}
