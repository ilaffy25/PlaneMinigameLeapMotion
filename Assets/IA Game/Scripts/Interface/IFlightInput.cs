using UnityEngine;

public interface IFlightInput
{
    float Pitch { get; }
    float Roll { get; }
    float Yaw { get; }
}
