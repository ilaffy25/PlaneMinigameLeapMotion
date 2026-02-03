using UnityEngine;

public class RetryGestureListener : MonoBehaviour
{
    private bool hasTriggered = false;

    public void OnRetryPoseDetected()
    {
        if (hasTriggered)
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentState != GameState.GameOver)
            return;

        hasTriggered = true;

        GameManager.Instance.RequestRetry();
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
