using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    public void Retry()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RequestRetry();
    }

}
