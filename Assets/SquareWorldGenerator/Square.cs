using CellularAutomaton;
using UnityEngine;

namespace SquareWorldGenerator
{
	public class Square : MonoBehaviour
	{
		[SerializeField] TypeCollection colorCollection;
		[SerializeField] Rules rules;

		public SpriteRenderer sprite;

		int type;
		float grayValue;
		CellType cellType;

		public bool waitForUpdate;
		
		public int Type
		{
			get => type;
			set
			{
				type = value;
				sprite.color = colorCollection.GetColor(value);
			}
		}
		
		public float GrayValue
		{
			get => grayValue;
			set
			{
				this.grayValue = value;
				sprite.color = new Color(this.grayValue, value, value);
			}
		}
		
		public CellType CellType
		{
			get => cellType;
			set
			{
				cellType = value;
				sprite.color = rules.CellTypeToColor(value);
			}
		}

		public void SetColor(Color color)
		{
			sprite.color = color;
		}
	}
}