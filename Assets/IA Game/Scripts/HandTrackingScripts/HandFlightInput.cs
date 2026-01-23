using UnityEngine;
using System.Collections.Generic;
using Leap;

public class HandFlightInput : MonoBehaviour, IFlightInput
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
        if (GameManager.Instance == null)
        {
            ResetAxes();
            return;
        }

        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            ResetAxes();
            return;
        }

        if (HandSelectionManager.Instance == null || !HandSelectionManager.Instance.HasSelectedHand)
        {
            ResetAxes();
            return;
        }

        List<Hand> hands = HandsInfoManager.Instance.GetCurrentHands();
        if (hands == null || hands.Count == 0)
        {
            ResetAxes();
            return;
        }

        Chirality targetHand = GameManager.Instance.SelectedHand;
        Hand activeHand = null;

        foreach (var h in hands)
        {
            if (h.GetChirality() == targetHand)
            {
                activeHand = h;
                break;
            }
        }

        if (activeHand == null)
        {
            ResetAxes();
            return;
        }

        ProcessHand(activeHand);
    }

    private void ProcessHand(Hand hand)
    {
        Quaternion rot = hand.Rotation;
        Vector3 euler = rot.eulerAngles;

        float x = NormalizeAngle(euler.x); // Pitch
        float y = NormalizeAngle(euler.y); // Yaw
        float z = NormalizeAngle(euler.z); // Roll

        // PITCH
        float xRelative = x + 10f;
        float pitchValue = Mathf.Clamp(xRelative / 40f, -1f, 1f);
        pitchValue = ApplyDeadzone(pitchValue, pitchDeadzone / 90f);
        Pitch = pitchValue * pitchSensitivity;

        // ROLL
        float rollValue = Mathf.Clamp(z / 45f, -1f, 1f);
        rollValue = ApplyDeadzone(rollValue, rollDeadzone / 90f);
        Roll = rollValue * rollSensitivity;

        // YAW
        float yawValue = Mathf.Clamp(y / 45f, -1f, 1f);
        yawValue = ApplyDeadzone(yawValue, yawDeadzone / 90f);
        Yaw = yawValue * yawSensitivity;
    }

    private void ResetAxes()
    {
        Pitch = 0f;
        Roll = 0f;
        Yaw = 0f;
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
