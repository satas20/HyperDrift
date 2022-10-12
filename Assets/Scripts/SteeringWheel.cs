using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace GDGames.Inputs
{
	public class SteeringWheel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
	{
		[Tooltip("Max degrees")]
		[SerializeField]
		float maxAngle = 450f;

		[Tooltip("Degrees per second")]
		[SerializeField]
		float releaseSpeed = 350f;

		[SerializeField]
		float deadZone;


		float input;
		public float GetInput() { return input; }

		public event System.Action<float> InputChanged;

		float wheelAngle = 0f;
		float wheelPrevAngle = 0f;
		Vector2 centerPoint;
		RectTransform steeringWheelRect;
		Coroutine releaseWheelCoroutine;


		void Start()
		{
			GetSteeringWheelRectTranform();
		}

		void GetSteeringWheelRectTranform()
        {
			steeringWheelRect = GetComponent<RectTransform>();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			CalculateCenterPoint(eventData);
			CalculateWheelPreviousAngle(eventData);

			if (releaseWheelCoroutine != null)
            {
				StopCoroutine(releaseWheelCoroutine);
            }
		}

		public void OnDrag(PointerEventData eventData)
		{
			CalculateWheelRotation(eventData);
			UpdateWheelTransform();
			CalculateInputAndTriggerEvent();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			OnDrag(eventData);
			releaseWheelCoroutine = StartCoroutine(ReleaseWheel());
		}

		void CalculateCenterPoint(PointerEventData eventData)
        {
			centerPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, steeringWheelRect.position);
		}

		void CalculateWheelPreviousAngle(PointerEventData eventData)
		{
			wheelPrevAngle = Vector2.Angle(Vector2.up, eventData.position - centerPoint);
		}

		void CalculateWheelRotation(PointerEventData eventData)
		{
			Vector2 pointerPos = eventData.position;
			float newWheelAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);
			CalculateWheelAngle(pointerPos, newWheelAngle);
			ClampWheelAngle();
			wheelPrevAngle = newWheelAngle;
		}

		void CalculateWheelAngle(Vector2 pointerPos, float wheelNewAngle)
		{
			if (!isPointerTooCloseToCenter(pointerPos))
			{
				if (pointerPos.x > centerPoint.x)
					wheelAngle += wheelNewAngle - wheelPrevAngle;

				else
					wheelAngle -= wheelNewAngle - wheelPrevAngle;
			}
		}

		bool isPointerTooCloseToCenter(Vector2 pointerPos) { return (pointerPos - centerPoint).sqrMagnitude < deadZone * 10000f; }

		void ClampWheelAngle()
		{
			wheelAngle = Mathf.Clamp(wheelAngle, -maxAngle, maxAngle);
		}

		void UpdateWheelTransform()
		{
			steeringWheelRect.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);
		}

		void CalculateInputAndTriggerEvent()
		{
			input = wheelAngle / maxAngle;
			InputChanged?.Invoke(input);
		}

		IEnumerator ReleaseWheel()
		{
			while (wheelAngle != 0f)
			{
				ReleaseWheelPerFrame();
				UpdateWheelTransform();
				CalculateInputAndTriggerEvent();

				yield return null;
			}
		}

		void ReleaseWheelPerFrame()
		{
			float deltaAngle = releaseSpeed * Time.deltaTime;

			if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
				wheelAngle = 0f;

			else if (wheelAngle > 0f)
				wheelAngle -= deltaAngle;

			else
				wheelAngle += deltaAngle;
		}
	}
}