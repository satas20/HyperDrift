using UnityEngine;
using GDGames.Inputs;

public class InputPrinter : MonoBehaviour
{
    SteeringWheel steeringWheel;


    void Awake()
    {
        steeringWheel = FindObjectOfType<SteeringWheel>();
    }

    void OnEnable()
    {
        if (steeringWheel == null) return;

        steeringWheel.InputChanged += PrintInput;
    }

    void OnDisable()
    {
        if (steeringWheel == null) return;

        steeringWheel.InputChanged -= PrintInput;
    }


    void PrintInput(float input)
    {
        Debug.Log(input);
    }
}