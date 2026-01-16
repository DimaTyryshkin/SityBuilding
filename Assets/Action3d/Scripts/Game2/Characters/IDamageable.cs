using UnityEngine;

namespace Game
{
	public interface IDamageable
	{
		void ApplyDamage(Damage damage);
	}
	
	public class Damage
	{
		public int damage;
		public Vector3 worldPoint;
		public Vector3 damageDir;
	}
}