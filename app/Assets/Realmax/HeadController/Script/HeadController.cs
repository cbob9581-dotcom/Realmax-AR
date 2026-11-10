using UnityEngine;

namespace Realmax
{
	public class HeadController : MonoBehaviour
	{
		private float Timer = 0;
		private float InitialLength;
		private float AutoTriggerTime = 4f;
		private float SuspensionHeight = 0.1f;
		public Transform EndPoint;
		private GameObject CurrentPointTarget;
		public ReticleAnimation ReticleAnimation;

		void Start()
		{
			InitialLength = EndPoint.localPosition.z;
		}

		void Update()
		{
			if(Input.GetKeyDown((KeyCode)10))
			{
				Trigger();
			}
			if(ReticleAnimation.IsPlay)
			{
				Timer += Time.deltaTime;
				if(Timer >= AutoTriggerTime)
				{
					Trigger();
					ReticleAnimation.CloseRotateAnimation();
					Timer = 0;
				}
			}
			else
			{
				Timer = 0;
			}
			HeadControllerRay();

		}
		private void HeadControllerRay()
		{
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				float length = hit.distance;
				EndPoint.localPosition = Vector3.forward * length + hit.normal * SuspensionHeight;
				EndPoint.rotation = Quaternion.LookRotation(hit.normal);
				IControllerPointHandle trigger = hit.collider.GetComponent<IControllerPointHandle>();
				if(trigger != null)
				{
					if(CurrentPointTarget && CurrentPointTarget != hit.collider.gameObject)
					{
						IControllerPointHandle lastTrigger = CurrentPointTarget.GetComponent<IControllerPointHandle>();
						if(lastTrigger != null)
						{
							lastTrigger.OnDeviate();
							ReticleAnimation.CloseRotateAnimation();
						}
					}
					CurrentPointTarget = hit.collider.gameObject;
					ReticleAnimation.OpenRotateAnimation();
					trigger.OnPress();
				}
				else
				{
					if(CurrentPointTarget)
					{
						IControllerPointHandle lastTrigger = CurrentPointTarget.GetComponent<IControllerPointHandle>();
						lastTrigger.OnDeviate();
						ReticleAnimation.CloseRotateAnimation();
						CurrentPointTarget = null;
					}
				}
			}
			else
			{
				EndPoint.localPosition = Vector3.forward * InitialLength;
				EndPoint.localEulerAngles = Vector3.zero;

				if(CurrentPointTarget)
				{
					IControllerPointHandle lastTrigger = CurrentPointTarget.GetComponent<IControllerPointHandle>();
					lastTrigger.OnDeviate();
					ReticleAnimation.CloseRotateAnimation();
					CurrentPointTarget = null;
				}

			}
			EndPoint.localScale = new Vector3(EndPoint.localPosition.z, EndPoint.localPosition.z, 1);
		}

		private void Trigger()
		{
			if(CurrentPointTarget)
			{
				IControllerPointHandle trigger = CurrentPointTarget.GetComponent<IControllerPointHandle>();
				if(trigger != null)
				{
					var spatial=trigger as ISpatialPointerTarget;
                        if(spatial!=null)spatial.TriggerFrom(new Ray(transform.position,transform.forward));else trigger.OnTrigger();
				}
			}
		}
	}
}
