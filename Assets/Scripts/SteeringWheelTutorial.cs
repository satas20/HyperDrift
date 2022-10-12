using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SteeringWheelTutorial : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[Header("Steering Wheel Max Angle")]
	[SerializeField]
	private float maxAngle = 450f;

	[Header("Degrees Per Second")]
	[SerializeField]
	private float releaseSpeed = 350f;

	public static float steeringInput;

	private void ApplyInput()
	{
		//Set the car's input value equal to the steeringInput
		//For example: UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SetAxis("Horizontal", steeringInput);
	}


    //You don't need to modify anything under this comment

    
	private RectTransform wheel;

	private void Start()
	{
		wheel = GetComponent<RectTransform>();
	}

	#region Events

	public void OnPointerDown(PointerEventData eventData)
	{
		StartCalculatingWheelRotation(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{

		CalculateWheelRotation(eventData);

		UpdateWheelImage();

		CalculateInput();

		ApplyInput();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnDrag(eventData);

		StartCoroutine(ReleaseWheel());
	}

	#endregion

	#region Calculations

	private float wheelAngle = 0f;
	private float wheelPrevAngle = 0f;

	private Vector2 centerPoint;

	private void StartCalculatingWheelRotation(PointerEventData eventData)
	{
		centerPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, wheel.position);
		wheelPrevAngle = Vector2.Angle(Vector2.up, eventData.position - centerPoint);
	}

	private void CalculateWheelRotation(PointerEventData eventData)
	{
		Vector2 pointerPos = eventData.position;

		float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);

		// Do nothing if the pointer is too close to the center of the wheel
		if ((pointerPos - centerPoint).sqrMagnitude >= 400f)
		{
			if (pointerPos.x > centerPoint.x)
				wheelAngle += wheelNewAngle - wheelPrevAngle;

			else
				wheelAngle -= wheelNewAngle - wheelPrevAngle;
		}

		// Make sure wheel angle never exceeds maximumSteeringAngle
		wheelAngle = Mathf.Clamp(wheelAngle, -maxAngle, maxAngle);
		wheelPrevAngle = wheelNewAngle;
	}

	private IEnumerator ReleaseWheel()
	{
		while (wheelAngle != 0f)
		{
			float deltaAngle = releaseSpeed * Time.deltaTime;

			if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
				wheelAngle = 0f;

			else if (wheelAngle > 0f)
				wheelAngle -= deltaAngle;

			else
				wheelAngle += deltaAngle;


			UpdateWheelImage();

			CalculateInput();

			ApplyInput();

			yield return null;
		}
	}

	private void CalculateInput()
	{
		steeringInput = wheelAngle / maxAngle;
	}

	private void UpdateWheelImage()
	{
		wheel.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);
	}

	#endregion
}
