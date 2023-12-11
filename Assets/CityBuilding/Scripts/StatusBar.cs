using NaughtyAttributes;
using UnityEngine;

namespace Game
{
	public class StatusBar : MonoBehaviour
	{
		enum StateEnum
		{
			NotSet = 0,
			Empty = 1,
			InProgress = 2,
			Lock = 3,
		}

		[SerializeField] Transform bar;
		[SerializeField] GameObject lockMarker;
		[SerializeField] GameObject emptyMarker;

		StateEnum state;

		StateEnum State
		{
			get => state;
			set
			{
				if(state==value)
					return;

				state = value;
				
				bar.gameObject.SetActive(false);
				lockMarker.SetActive(false);
				emptyMarker.SetActive(false);
				
				switch (state)
				{
					case StateEnum.InProgress:
						bar.gameObject.SetActive(true);
						break;
					
					case StateEnum.Empty:
						emptyMarker.SetActive(true);
						break;
					
					case StateEnum.Lock:
						lockMarker.SetActive(true);
						break;
				}
			}
		}

		public void SetProgress(int value, int maxValue)
		{
			SetProgress(value / ((float)maxValue + float.Epsilon));
		}

		public void SetProgress(float value)
		{
			State = StateEnum.InProgress;

			bar.localScale = new Vector3(Mathf.Clamp01(value), 1, 1);
		}

		[Button()]
		public void SetLock()
		{
			State = StateEnum.Lock;
		}
		
		[Button()]
		public void SetEmpty()
		{
			State = StateEnum.Empty;
		}


		[Button()]
		void SetProgressTo50()
		{
			SetProgress(0.5f);
		}
	}
}