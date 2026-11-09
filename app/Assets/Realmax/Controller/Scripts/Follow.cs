using UnityEngine;

namespace Realmax
{
	public class Follow : MonoBehaviour
	{
		public Transform Target;
		public Vector3 PostionOffset;

		public bool IsOpenYawFollow;
		public bool IsOpenPostionFollow = true;
		private Vector3 Angle;

		void Update()
		{
			if(Target)
			{
				if(IsOpenPostionFollow)
					transform.position = Target.position + PostionOffset;
				if(IsOpenYawFollow)
				{
					Angle = transform.eulerAngles;
					Angle.y = Target.eulerAngles.y;
					transform.eulerAngles = Angle;
				}
			}
		}
	}
}
