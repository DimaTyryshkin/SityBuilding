using GamePackages.Core.Validation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GlobalStrategy.CoreLogic
{
	public class Delivery : MonoBehaviour
	{
		[SerializeField,IsntNull] MeshRenderer meshRenderer;
		[SerializeField, IsntNull] ProductsData productsData;
		
		City city;
		float speed;
		Products products;
		Vector3 target;
		UnityAction<City, Products> deliveryCallBack;

		public void Init(City city, Vector3 target, float speed, Products products, UnityAction<City, Products> deliveryCallBack)
		{
			Assert.IsNotNull(city);

			this.deliveryCallBack = deliveryCallBack;
			this.city = city;
			this.target = target;
			this.speed = speed;
			this.products = products;

			meshRenderer.material.color = productsData.GetColor(products.ProductIndex);
		}

		void Update()
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
			if (Vector3.Distance(transform.position, target) < Mathf.Epsilon)
			{
				deliveryCallBack.Invoke(city,products);
				//city.Add(products);
				Destroy(gameObject);
			}
		}
	}
} 