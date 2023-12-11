using UnityEngine;

namespace Game
{
	public class StatusBarToStoreAdapter:MonoBehaviour
	{ 
		[SerializeField] StatusBar statusBar;
		[SerializeField] ResourceStorage resourceStorage;

		void Start()
		{
			Draw();
			resourceStorage.ValueChanged += OnValueChanged;
		}

		void OnValueChanged(ResourceStorage storage, int arg1)
		{
			Draw();
		}

		void Draw()
		{
			int value = resourceStorage.Value;
			 
			if (value == 10)
				statusBar.SetLock();
			else
				statusBar.SetProgress((float)value / 10f);
		}
	}
}