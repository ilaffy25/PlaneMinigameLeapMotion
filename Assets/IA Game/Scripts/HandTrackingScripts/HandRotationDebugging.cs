using System.Collections.Generic;
using UnityEngine;
using Leap;

public class HandRotationDebugging : MonoBehaviour
{
    [Header("Debug Options")]
    public bool debugLogRotations = true;
    public bool debugPalmOrientation = true;

    [Header("Detection Settings")]
    public float palmThreshold = 45f; // Grados para considerar arriba / abajo

    private void Update()
    {
        List<Hand> hands = HandsInfoManager.Instance.GetCurrentHands();
        if (hands == null || hands.Count == 0)
        {
            Debug.Log("No hands detected");
            return;
        }

        foreach (Hand hand in hands)
        {
            Quaternion rotation = hand.Rotation;
            Vector3 euler = rotation.eulerAngles;

            // Normalizar ángulos a [-180, 180]
            float x = NormalizeAngle(euler.x);
            float y = NormalizeAngle(euler.y);
            float z = NormalizeAngle(euler.z);

            if (debugLogRotations)
            {
                Debug.Log(
                    $"[Hand: {(hand.IsRight ? "Right" : "Left")}]\n" +
                    $"Rotation Euler → X:{x:F1}  Y:{y:F1}  Z:{z:F1}"
                );
            }

            if (debugPalmOrientation)
            {
                DetectPalmOrientation(hand, z);
            }
        }
    }

    private void DetectPalmOrientation(Hand hand, float zRot)
{
    // Normalizamos el ángulo a [-180, 180]
    float z = NormalizeAngle(zRot);

    bool palmDown = Mathf.Abs(z) < 20f;                // z ∈ [-20, +20]
    bool palmUp = Mathf.Abs(Mathf.Abs(z) - 180f) < 20f; // z cerca de ±180°

    if (palmDown)
    {
        Debug.Log($"{(hand.IsRight ? "Right" : "Left")} palm → DOWN");
    }
    else if (palmUp)
    {
        Debug.Log($"{(hand.IsRight ? "Right" : "Left")} palm → UP");
    }
    else
    {
        Debug.Log($"{(hand.IsRight ? "Right" : "Left")} palm → SIDE");
    }
}


    // Normaliza 0–360 a -180 a 180
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
