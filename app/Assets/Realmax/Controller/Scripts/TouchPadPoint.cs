using UnityEngine;

namespace Realmax
{
	public class TouchPadPoint : MonoBehaviour
	{

		private Transform point;
		private MeshRenderer PointmeshRenderer;
		private void Start()
		{
			point = transform.GetChild(0);
			PointmeshRenderer = point.GetComponent<MeshRenderer>();
		}
		void Update()
		{
			float x = ControllerDataCallBack.Instance.TouchPadX;
			float y = ControllerDataCallBack.Instance.TouchPadY;
			if(x == 0 && y == 0)
			{
				PointmeshRenderer.enabled = false;
				return;
			}
			PointmeshRenderer.enabled = true;
			float PadLength = 1000;
			float Pointx = (x - 500) / PadLength;
			float Pointy = -(y - 500) / PadLength;
			Vector3 pos = new Vector3(Pointx, Pointy, 0.01f);
			point.localPosition = pos;
		}
	}
}
