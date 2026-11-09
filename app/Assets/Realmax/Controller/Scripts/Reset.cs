using UnityEngine;

namespace Realmax
{
	public class Reset : MonoBehaviour
	{

		public ControllerRotation ControllerRotation;

		void Start()
		{
			Input.backButtonLeavesApp = false;
			ControllerKeyEvent.ReturnKeyDown.AddListener(ResetControllerAndSvr);
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				ResetControllerAndSvr();
			}
		}

		void ResetControllerAndSvr()
		{
			if(ControllerRotation)
			ControllerRotation.ResetController();
			SvrManager.Instance.RecenterTracking();
		}
	}
}
