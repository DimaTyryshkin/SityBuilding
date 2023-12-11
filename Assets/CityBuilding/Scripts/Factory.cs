using UnityEngine;

namespace Game
{
	public class Factory : MonoBehaviour
	{
		[SerializeField] ResourceStorage inStorage;
		[SerializeField] ResourceStorage outStorage;
		[SerializeField] StatusBar progressBar;
		[SerializeField] float processTime = 10; 

		float timeStartProcess;
		float progress;
		bool isProcess;

		public string InResourceName => inStorage.ResourceName;
		public string OutResourceName => outStorage.ResourceName;

		public int InResourceValue => inStorage.Value;
		public int OutResourceValue => outStorage.Value;

		public void Init()
		{
			inStorage.SetModes(StorageOrder.Factory, StorageAccessRules.Put);
			outStorage.SetModes(StorageOrder.Factory, StorageAccessRules.Get);
		}

		void Update()
		{
			if (isProcess)
			{
				progress = (Time.time - timeStartProcess) / processTime;
				if (progress >= 1)
				{
					outStorage.Value++;
					isProcess = false;
				}
				else
				{
					progressBar.SetProgress(progress);
				}
			}
			else
			{
				if (inStorage.Value == 0)
				{
					progressBar.SetEmpty();
				}
				else
				{
					if (outStorage.Value < 10)
					{
						inStorage.Value--;
						isProcess = true;
						timeStartProcess = Time.time;
					}
					else
					{
						progressBar.SetLock();
					}
				}
			}
		}
	}
}