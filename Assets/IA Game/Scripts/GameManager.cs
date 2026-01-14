using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Idle,
    Playing,
    GameOver
}

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
    public GameObject retryButton;

    private float currentFuel;
    private float score;
    private Vector3 startPosition;

    public GameState CurrentState { get; private set; }

    public bool IsPlaying => CurrentState == GameState.Playing;

    private void Start()
    {
        currentFuel = Mathf.Clamp(startingFuel, 0f, maxFuel);
        startPosition = playerTransform ? playerTransform.position : Vector3.zero;

        UpdateFuelUI();
        UpdateScoreUI();

        if (retryButton != null)
            retryButton.SetActive(false);

        SetStatusText("Fly through the fuel rings!", Color.white);

        SetState(GameState.Playing);
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.Playing:
                UpdatePlaying();
                break;

            case GameState.GameOver:
                UpdateGameOver();
                break;
        }
    }

    private void UpdatePlaying()
    {
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

    private void UpdateGameOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    public void ConsumeFuel(float amount)
    {
        if (CurrentState != GameState.Playing) return;

        currentFuel = Mathf.Max(0f, currentFuel - amount);
        UpdateFuelUI();

        if (currentFuel <= 0f)
        {
            TriggerGameOver("Out of fuel!");
        }
    }

    public void AddFuel(float amount)
    {
        if (CurrentState != GameState.Playing) return;

        currentFuel = Mathf.Clamp(currentFuel + amount, 0f, maxFuel);
        score += scorePerCheckpoint;

        UpdateFuelUI();
        UpdateScoreUI();
        SetStatusText("Checkpoint!", Color.cyan);
    }

    public void TriggerGameOver(string reason)
    {
        if (CurrentState == GameState.GameOver) return;

        SetState(GameState.GameOver);

        SetStatusText($"{reason}\nPress R to retry", Color.red);

        if (retryButton != null)
            retryButton.SetActive(true);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.Playing:
                OnEnterPlaying();
                break;

            case GameState.GameOver:
                OnEnterGameOver();
                break;
        }
    }

    private void OnEnterPlaying()
    {
        // Aquí luego meteremos lógica de inicio, countdown, etc.
    }

    private void OnEnterGameOver()
    {
        // Aquí luego meteremos lógica de UI, animaciones, etc.
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
