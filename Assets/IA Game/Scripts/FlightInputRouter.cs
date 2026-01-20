using UnityEngine;

public class FlightInputRouter : MonoBehaviour
{
    public ClassicFlightInput classicInput;
    public HandFlightInput handInput;

    public bool useHandInput = true;

    public float Pitch =>
        (useHandInput && handInput ? handInput.Pitch : 0f)
        + (classicInput ? classicInput.Pitch : 0f);

    public float Roll =>
        (useHandInput && handInput ? handInput.Roll : 0f)
        + (classicInput ? classicInput.Roll : 0f);

    public float Yaw =>
        (useHandInput && handInput ? handInput.Yaw : 0f)
        + (classicInput ? classicInput.Yaw : 0f);
}
