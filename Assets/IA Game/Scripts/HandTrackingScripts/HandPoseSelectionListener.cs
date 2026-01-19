using Leap;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseSelectionListener : MonoBehaviour
{
    public HandPoseDetector poseDetector;

    private bool hasSelected = false;

    private void Awake()
    {
        if (!poseDetector)
            Debug.LogError("PoseDetector not assigned!");
    }

    public void OnPoseDetected()
    {
        if (hasSelected) return;

        if (GameManager.Instance.CurrentState != GameState.HandSelection)
            return;

        if (!TryDetectActiveHand(out Chirality detectedHand))
            return;

        hasSelected = true;

        HandSelectionManager.Instance.SelectHand(detectedHand);
    }

    private bool TryDetectActiveHand(out Chirality chirality)
    {
        chirality = Chirality.Left;

        var hands = HandsInfoManager.Instance.GetCurrentHands();

        if (hands == null || hands.Count == 0)
            return false;

        chirality = hands[0].GetChirality();
        return true;
    }

    public void ResetSelection()
    {
        hasSelected = false;
    }
}
