using UnityEngine;

namespace Game
{
	public class Hit : MonoBehaviour
	{
		[SerializeField] float[] durations;  
		[SerializeField] Color[] colors;
		[SerializeField] Color[] emissionColors;
		
		[Space]
		[SerializeField] MeshRenderer meshRenderer; 
		
		MaterialPropertyBlock block;
		float timeStart;
		int step;
		
		
		static readonly int Color1 = Shader.PropertyToID("_Color");
		static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
		

		void Start()
		{
			step = 1;
			timeStart = Time.time;
			block = new MaterialPropertyBlock();

			Update();
		}

		void Update()
		{
			if (step < colors.Length)
			{
				float t = Mathf.Clamp01((Time.time - timeStart) / durations[step-1]);
				meshRenderer.GetPropertyBlock(block);
				block.SetColor(Color1, Color.Lerp(colors[step - 1], colors[step], t));
				block.SetColor(EmissionColor, Color.Lerp(emissionColors[step - 1], emissionColors[step], t));
				meshRenderer.SetPropertyBlock(block);

				if (t >= 1)
				{
					step++;
					timeStart = Time.time;
				}
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}