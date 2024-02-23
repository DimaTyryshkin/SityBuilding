using System;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class CurvesSet
	{
		public float speed;
		public AnimationCurve flyLegCurveX;
		public AnimationCurve flyLegCurveY; 
	}

	public class Test : MonoBehaviour
	{
		[SerializeField] float speed = 1;
		[SerializeField] float legsMoveSpeed = 1;
		[SerializeField] float stepSize = 1;
		[SerializeField] AnimationCurve stepSizeFormSpeedCurve; 
		[SerializeField] CurvesSet walk;
		[SerializeField] CurvesSet run; 
		[SerializeField] Transform[] points;

 
		int[,] legs; 
		int legsAmount;
		float stepSizeFinal;

		void Start()
		{
			legsAmount = points.Length;
			legs = new int[legsAmount, 2];
		}

		void Update()
		{
			stepSizeFinal = stepSize * stepSizeFormSpeedCurve.Evaluate(speed);
  
			transform.position += new Vector3(
				Input.GetAxis("Horizontal") * Time.deltaTime * speed,
				Input.GetAxis("Vertical") * Time.deltaTime * speed * 0.3f,
				0);

			float centerX = ToStepSpace(transform.position.x);


			int step = (int)Mathf.Floor(centerX);
			float t = centerX - step;

			Vector3 pR;
			Vector3 pL;

			if (
				step % 2 == 0)
			{
				int r0 = step;
				int l0 = step - 1;

				pR = ProcessLeg(centerX, r0);
				pL = ProcessLeg(centerX, l0);
				//pL = new Vector3(l0 + 2, 0, 0);

			}
			else
			{
				int l0 = step;
				int r0 = step - 1;

				pL = ProcessLeg(centerX, l0);
				pR = ProcessLeg(centerX, r0);
				//pR = new Vector3(l0, 0, 0);
			}



			Vector3 p0 = new Vector3(FromStepSpace(pR.x), pR.y, 0);
			Vector3 p1 = new Vector3(FromStepSpace(pL.x), pL.y, 0);

			points[0].position = p0;
			points[1].position = p1;
		}

		Vector2 ProcessLeg(float centerX, int step)
		{
			float t = (centerX - step) / 2f;


			float x0 = walk.flyLegCurveX.Evaluate(t);
			float x1 = run.flyLegCurveX.Evaluate(t);
			float lerpFactor = Mathf.InverseLerp(walk.speed, run.speed, speed);
			float x = Mathf.Lerp(x0, x1, lerpFactor);
			x = step + x * 2f;
			
			float y0 = walk.flyLegCurveY.Evaluate(t);
			float y1 = run.flyLegCurveY.Evaluate(t);
			lerpFactor = Mathf.InverseLerp(walk.speed, run.speed, speed);
			float y = Mathf.Lerp(y0, y1, lerpFactor);
			

			return new Vector3(x, y);
		}

		float ToStepSpace(float x) => x / stepSizeFinal;
		float FromStepSpace(float x) => x * stepSizeFinal;
	}
}