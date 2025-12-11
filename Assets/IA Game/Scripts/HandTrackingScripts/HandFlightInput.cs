using UnityEngine;
using System.Collections.Generic;
using Leap;

public class HandFlightInput : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    [Range(0.1f, 5f)]
    public float pitchSensitivity = 1f;

    [Range(0.1f, 5f)]
    public float rollSensitivity = 1f;

    [Range(0.1f, 5f)]
    public float yawSensitivity = 1f;

    [Header("Deadzones")]
    public float pitchDeadzone = 5f;
    public float rollDeadzone = 5f;
    public float yawDeadzone = 5f;

    // Ejes finales equivalentes a Input.GetAxis()
    public float Pitch { get; private set; }
    public float Roll { get; private set; }
    public float Yaw { get; private set; }

    private void Update()
    {
        List<Hand> hands = HandsInfoManager.Instance.GetCurrentHands();
        if (hands == null || hands.Count == 0)
        {
            Pitch = Roll = Yaw = 0;
            return;
        }

        // Tomamos solo mano derecha por ahora
        Hand rightHand = null;
        foreach (var h in hands)
        {
            if (h.IsRight) { rightHand = h; break; }
        }

        if (rightHand == null)
        {
            Pitch = Roll = Yaw = 0;
            return;
        }

        ProcessRightHand(rightHand);
    }

    private void ProcessRightHand(Hand hand)
    {
        Quaternion rot = hand.Rotation;
        Vector3 euler = rot.eulerAngles;

        float x = NormalizeAngle(euler.x);  // Pitch axis
        float y = NormalizeAngle(euler.y);  // Yaw axis
        float z = NormalizeAngle(euler.z);  // Roll axis

        // 1) PITCH → basarnos en inclinación adelante/atrás
        // Palma abajo (neutro) ~ X ≈ -30°, así que normalizamos relativo a eso
        float xRelative = x + 15f;

        float pitchValue = Mathf.Clamp(xRelative / 40f, -1f, 1f);
        pitchValue = ApplyDeadzone(pitchValue, pitchDeadzone / 90f);
        Pitch = pitchValue * pitchSensitivity;

        // 2) ROLL → rotación como volante (Z)
        float rollValue = Mathf.Clamp(z / 45f, -1f, 1f);
        rollValue = ApplyDeadzone(rollValue, rollDeadzone / 90f);
        Roll = rollValue * rollSensitivity;

        // 3) YAW → girar palma izquierda/derecha (Y)
        float yawValue = Mathf.Clamp(y / 45f, -1f, 1f);
        yawValue = ApplyDeadzone(yawValue, yawDeadzone / 90f);
        Yaw = yawValue * yawSensitivity;
    }

    private float ApplyDeadzone(float value, float dead)
    {
        if (Mathf.Abs(value) < dead) return 0f;
        return value;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
