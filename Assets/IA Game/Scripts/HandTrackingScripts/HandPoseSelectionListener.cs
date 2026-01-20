using Leap;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseSelectionListener : MonoBehaviour
{
    [Header("Config")]
    public Chirality handType; // Left o Right

    private bool hasSelected = false;

    public void OnPoseDetected()
    {
        if (hasSelected)
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.HandSelection)
            return;

        hasSelected = true;

        Debug.Log($"Pose detected by {handType} hand");

        if (HandSelectionManager.Instance != null)
            HandSelectionManager.Instance.SelectHand(handType);
    }

    public void ResetSelection()
    {
        hasSelected = false;
    }
}
