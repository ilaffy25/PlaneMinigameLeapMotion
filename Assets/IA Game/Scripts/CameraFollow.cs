using UnityEngine;

/// <summary>
/// Smoothly follows the aircraft using a spring-like offset.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Tooltip("Transform to follow, usually the player's plane.")]
    public Transform target;

    [Tooltip("Offset relative to the target's local space.")]
    public Vector3 followOffset = new Vector3(0f, 6f, -12f);

    [Tooltip("Interpolation speed for the camera's position.")]
    public float positionLerpSpeed = 4f;

    [Tooltip("Interpolation speed for the camera's rotation.")]
    public float rotationLerpSpeed = 6f;

    private void LateUpdate()
    {
        if (!target)
        {
            return;
        }

        // Determine desired position in world space relative to the plane's orientation.
        Vector3 desiredPosition = target.TransformPoint(followOffset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionLerpSpeed);

        // Look slightly ahead of the plane for a cinematic framing.
        Vector3 lookPoint = target.position + target.forward * 20f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * rotationLerpSpeed);
    }
}
