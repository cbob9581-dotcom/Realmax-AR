using UnityEngine;

namespace Realmax
{

	public class ControllerRay : MonoBehaviour
	{

		private float RayLineLimitLengh = 2f;
		private float RecticleScale_MinDis = 1;
		private float RecticleScale_MaxDis = 10;
		private float RecticleScale_Min = 0.03f;
		private float RecticleScale_Max = 1;
		private GameObject CurrrentRayPointTarget;

		private LineRenderer HandRayLine;

		public Transform ControllerMode;
		public Transform RayEndPoint;
		public Transform RayStartPoint;
		public Transform Reticle;
		[HideInInspector] public bool EnableRayLine = true;
		[HideInInspector] private bool IsHit = false;
		[HideInInspector] public float RayLimitLengh = 1000;
		[HideInInspector] private RaycastHit CurrentRayHit;
		void Start()
		{
			HandRayLine = transform.GetComponentInChildren<LineRenderer>();
			ControllerKeyEvent.TouchPadKeyDown.AddListener(RayClick);
		}

		void Update()
		{
			UpdateControllerRay();
			if(IsHit)
			{
				if(CurrrentRayPointTarget)
				{
					if(CurrrentRayPointTarget != CurrentRayHit.collider.gameObject)
					{
						IControllerPointHandle laseTrigger = CurrrentRayPointTarget.GetComponent<IControllerPointHandle>();
						if(laseTrigger != null)
						{
							laseTrigger.OnDeviate();
						}
					}
				}
				CurrrentRayPointTarget = CurrentRayHit.collider.gameObject;
				IControllerPointHandle trigger = CurrrentRayPointTarget.GetComponent<IControllerPointHandle>();
				if(trigger != null)
				{
					trigger.OnPress();
				}
			}
			else
			{
				if(CurrrentRayPointTarget != null)
				{
					IControllerPointHandle trigger = CurrrentRayPointTarget.GetComponent<IControllerPointHandle>();
					if(trigger != null)
					{
						trigger.OnDeviate();
						CurrrentRayPointTarget = null;
					}
				}

			}
		}

		private void RayClick()
		{
			if(IsHit)
			{
				IControllerPointHandle trigger = CurrentRayHit.collider.GetComponent<IControllerPointHandle>();
				if(trigger != null)
				{
					var spatial=trigger as ISpatialPointerTarget;
                        if(spatial!=null)spatial.TriggerFrom(new Ray(RayStartPoint.position,ControllerMode.forward));else trigger.OnTrigger();
				}
			}
		}

		private void UpdateControllerRay()
		{
			Ray ray = new Ray(RayStartPoint.position, ControllerMode.forward);
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo, RayLimitLengh))
			{
				CurrentRayHit = hitInfo;
				IsHit = true;
			}
			else
			{
				IsHit = false;
			}
			SetRayLineLength(hitInfo);
		}

		private void SetRayLineFar()
		{
			HandRayLine.SetPosition(1, new Vector3(0, 0, 0));
			RayEndPoint.transform.localPosition = new Vector3(0, 1000, 0);
			Reticle.transform.localPosition = new Vector3(0, 1000, 0);
		}

		private void SetRayLineLength(RaycastHit hitInfo)
		{
			if(EnableRayLine)
			{
				if(!IsLimitRayLenght)
				{
					if(hitInfo.collider)
					{
						float Dis = Vector3.Distance(RayStartPoint.position, hitInfo.point);
						HandRayLine.SetPosition(1, new Vector3(0, Dis, 0));
						RayEndPoint.position = hitInfo.point;
						Reticle.position = hitInfo.point + hitInfo.normal * 0.01f;
						Reticle.LookAt(hitInfo.normal);
						Quaternion q = Quaternion.LookRotation(hitInfo.normal);
						Reticle.rotation = q;
						Reticle.localEulerAngles = new Vector3(Reticle.localEulerAngles.x + 180, Reticle.localEulerAngles.y, Reticle.localEulerAngles.z);
						SetRecticleScale(Dis);
					}
					else
					{
						SetRayLinelength(1000);
					}
				}
				else
				{
					SetRayLinelength(RayLineLimitLengh);
				}
			}
			else
			{
				SetRayLineFar();
			}
		}
		private bool IsLimitRayLenght = false;

		public void SetRayLinelength(float Length)
		{
			RayEndPoint.transform.localPosition = new Vector3(0, 0, Length + RayStartPoint.localPosition.z);
			Reticle.transform.localPosition = new Vector3(0, 0, Length + RayStartPoint.localPosition.z);
			HandRayLine.SetPosition(1, new Vector3(0, Length, 0));
			SetRecticleScale(Length);
		}

		public void DisableLimitRayLength()
		{
			if(IsLimitRayLenght)
				IsLimitRayLenght = false;
		}

		private void SetRecticleScale(float distance)
		{
			if(distance < RecticleScale_MinDis)
			{
				Reticle.localScale = Vector3.one * RecticleScale_Min;
			}
			else if(distance > RecticleScale_MaxDis)
			{
				Reticle.localScale = Vector3.one * RecticleScale_Max;
			}
			else
			{
				float offset = RecticleScale_Min + distance / (RecticleScale_MaxDis - RecticleScale_MinDis) * (RecticleScale_Max - RecticleScale_Min);
				Reticle.localScale = Vector3.one * offset;
			}
		}
	}
}
