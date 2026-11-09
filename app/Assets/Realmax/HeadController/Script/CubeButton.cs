using UnityEngine;

namespace Realmax
{
	public class CubeButton : MonoBehaviour, IControllerPointHandle
	{

		public void OnTrigger()
		{
			GetComponent<MeshRenderer>().material.color = RandomColor();
		}
		public void OnPress()
		{
			transform.localScale = Vector3.one * 1f;
		}

		public void OnDeviate()
		{
			transform.localScale = Vector3.one * 0.5f;
		}

		Color RandomColor()
		{
			float r = Random.Range(0f, 1f);
			float g = Random.Range(0f, 1f);
			float b = Random.Range(0f, 1f);
			Color color = new Color(r, g, b);
			return color;
		}
	}
}