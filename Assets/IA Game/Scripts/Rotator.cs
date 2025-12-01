using UnityEngine;

/// <summary>
/// Rotates a checkpoint or collectible for better visibility.
/// </summary>
public class Rotator : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second.")]
    public Vector3 rotationSpeed = new Vector3(0f, 60f, 0f);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
