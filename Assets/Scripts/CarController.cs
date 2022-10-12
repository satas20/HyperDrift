using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] float carSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float dragAmount;
    [SerializeField] float steerAngle;
    [SerializeField] float traction;
    float steerInput;
    public Transform lfw;
    public Transform rfw;

    Vector3 moveVec;
    Vector3 rotVec;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        steerInput = SteeringWheelTutorial.steeringInput ;
        //Debug.Log("steering input"+steerInput + "hor input" + Input.GetAxis("Horizontal"));


        //Moving Car
        moveVec += transform.forward * carSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        //moveVec += transform.forward * carSpeed  * Time.deltaTime;
        transform.position += moveVec * Time.deltaTime;
        //Adding Drag
        moveVec *= dragAmount;
        //Limiting speed
        moveVec = Vector3.ClampMagnitude(moveVec, maxSpeed);
        //Rotating Car
        transform.Rotate(Vector3.up * steerInput * steerAngle*Time.deltaTime*moveVec.magnitude); // steer input
        transform.Rotate(Vector3.up * steerAngle * Input.GetAxis("Horizontal") * Time.deltaTime * moveVec.magnitude); // input system input

        //Making drift more natural
        moveVec = Vector3.Lerp(moveVec.normalized, transform.forward, traction * Time.deltaTime) * moveVec.magnitude;

        //Adjusting rotation vector
        rotVec += new Vector3(0,Input.GetAxis("Horizontal"), 0);
        //Limiting rotation Vector
        rotVec = Vector3.ClampMagnitude(rotVec, steerAngle);
        //Rotating wheels
        lfw.localRotation = Quaternion.Euler(rotVec);
        rfw.localRotation = Quaternion.Euler(rotVec);

        if (Input.GetKeyDown("r")){
            transform.position = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.Euler(0,0,0);

        }
    }

}
