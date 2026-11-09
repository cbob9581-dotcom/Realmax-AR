using UnityEngine;

namespace Realmax
{
	public class ReticleAnimation : MonoBehaviour
	{
		private Material ReticleMaterial;
		private int Index = 0;
		private float Timer = 0;
		private float PlaySpeed = 0.1f;
		private float OffsetRatio = 0.125f;
		private bool IsClosed = true;
		private bool isPlay = false;

		public bool IsPlay
		{
			get
			{
				return isPlay;
			}

			set
			{
				isPlay = value;
			}
		}

		void Start()
		{
			ReticleMaterial = GetComponent<Renderer>().material;
		}

		public void OpenRotateAnimation()
		{
			IsPlay = true;
			IsClosed = false;
		}

		public void CloseRotateAnimation()
		{
			IsPlay = false;
		}

		private void SetReticleMaterial(int offset)
		{
			ReticleMaterial.SetTextureOffset("_MainTex", new Vector2(offset * OffsetRatio, 2));
		}
		void Update()
		{
			if(IsPlay)
			{
				Timer += Time.deltaTime;
				if(Timer > PlaySpeed)
				{
					Index++;
					if(Index >= 1 / OffsetRatio)
						Index = 0;
					SetReticleMaterial(Index);
					Timer = 0;
				}
			}
			else
			{
				if(!IsClosed)
				{
					Timer = 0;
					Index = 0;
					SetReticleMaterial(Index);
					IsClosed = true;
				}
			}
		}
	}
}
