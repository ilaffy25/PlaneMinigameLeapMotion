using UnityEngine;

public class ClassicFlightInput : MonoBehaviour, IFlightInput
{
    public float Pitch => Input.GetAxis("Vertical");
    public float Roll => -Input.GetAxis("Horizontal");
    public float Yaw => Input.GetAxis("Yaw");
}
