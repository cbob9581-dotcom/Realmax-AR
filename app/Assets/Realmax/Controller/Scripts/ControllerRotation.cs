using UnityEngine;

namespace Realmax
{
	public class ControllerRotation : MonoBehaviour
	{

		private Transform ControllerRotatePoint;
		private Transform Shoulder;
		private Quaternion ControllerAngle;
		private Quaternion temp;
		private Quaternion angles;
		private Quaternion deltail;

		void Start()
		{
			Shoulder = transform.Find("Shoulder");
			ControllerRotatePoint = transform.Find("Pont2");
		}
		void FixedUpdate()
		{
			if(ControllerDataCallBack.Instance.IsConnect)
			{
				float x = ControllerDataCallBack.Instance.Pitch;
				float y = ControllerDataCallBack.Instance.Yaw;
				float z = ControllerDataCallBack.Instance.Roll;
				ControllerAngle = Quaternion.Euler(new Vector3(-x, y, -z));
				deltail = Quaternion.Euler(new Vector3(ControllerAngle.eulerAngles.x, ControllerAngle.eulerAngles.y - temp.eulerAngles.y, ControllerAngle.eulerAngles.z));
				ControllerRotatePoint.rotation = deltail;
				SetShoulderAngle(Shoulder, x);
			}
		}
		void SetShoulderAngle(Transform shoulder, float angle)
		{
			float ChangeAngle = 360 - (angle + 90) / 4;
			shoulder.localEulerAngles = Vector3.right * ChangeAngle;
		}
		public void ResetController()
		{
			temp = ControllerAngle;
		}
	}
}
