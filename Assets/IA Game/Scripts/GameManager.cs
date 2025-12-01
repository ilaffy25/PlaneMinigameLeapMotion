using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Fuel")]
    public float startingFuel = 30f;
    public float maxFuel = 60f;

    [Header("Gameplay")]
    public float scorePerCheckpoint = 100f;
    public float distanceScoreMultiplier = 1f;
    public Transform playerTransform;

    [Header("UI")]
    public Slider fuelSlider;
    public Text fuelLabel;
    public Text statusLabel;
    public Text scoreLabel;

    [Header("UI - Game Over")]
    [Tooltip("Botón de reinicio que aparece cuando se acaba la gasolina.")]
    public GameObject retryButton;

    private float currentFuel;
    private bool isPlaying;
    private float score;
    private Vector3 startPosition;

    public bool IsPlaying => isPlaying;

    private void Start()
    {
        currentFuel = Mathf.Clamp(startingFuel, 0f, maxFuel);
        startPosition = playerTransform ? playerTransform.position : Vector3.zero;
        UpdateFuelUI();
        UpdateScoreUI();
        SetStatusText("Fly through the fuel rings!", Color.white);
        isPlaying = true;

        // Asegurar que el botón esté oculto al inicio
        if (retryButton != null)
            retryButton.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        if (currentFuel <= 0f)
        {
            TriggerGameOver("Out of fuel!");
            return;
        }

        if (playerTransform)
        {
            float distanceTravelled = Vector3.Distance(startPosition, playerTransform.position);
            float distanceScore = distanceTravelled * distanceScoreMultiplier * Time.deltaTime;

            if (distanceScore > 0f)
            {
                score += distanceScore;
                UpdateScoreUI();
            }
        }
    }

    public void ConsumeFuel(float amount)
    {
        if (!isPlaying) return;

        currentFuel = Mathf.Max(0f, currentFuel - amount);
        UpdateFuelUI();

        if (currentFuel <= 0f)
        {
            TriggerGameOver("Out of fuel!");
        }
    }

    public void AddFuel(float amount)
    {
        if (!isPlaying) return;

        currentFuel = Mathf.Clamp(currentFuel + amount, 0f, maxFuel);
        score += scorePerCheckpoint;

        UpdateFuelUI();
        UpdateScoreUI();
        SetStatusText("Checkpoint!", Color.cyan);
    }

    public void TriggerGameOver(string reason)
    {
        if (!isPlaying) return;

        isPlaying = false;
        SetStatusText($"{reason}\nPress R to retry", Color.red);

        if (retryButton != null)
            retryButton.SetActive(true);
    }

    private void UpdateFuelUI()
    {
        if (fuelSlider)
        {
            fuelSlider.maxValue = maxFuel;
            fuelSlider.value = currentFuel;
        }

        if (fuelLabel)
            fuelLabel.text = $"Fuel: {currentFuel:0.0}/{maxFuel:0.0}";
    }

    private void UpdateScoreUI()
    {
        if (scoreLabel)
            scoreLabel.text = $"Score: {Mathf.RoundToInt(score)}";
    }

    private void SetStatusText(string message, Color color)
    {
        if (statusLabel)
        {
            statusLabel.text = message;
            statusLabel.color = color;
        }
    }
}
