using Leap;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Boot,
    HandSelection,
    Countdown,
    Playing,
    Paused,
    GameOver
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;

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


    public Chirality SelectedHand { get; private set; }

    public GameState CurrentState { get; private set; }

    public bool IsPlaying => CurrentState == GameState.Playing;

    private void SetUIPanels(
    bool showWelcome,
    bool showGameplay,
    bool showGameOver
)
    {
        if (welcomePanel) welcomePanel.SetActive(showWelcome);
        if (gameplayPanel) gameplayPanel.SetActive(showGameplay);
        if (gameOverPanel) gameOverPanel.SetActive(showGameOver);
    }

    // Countdown
    private float countdownTimer = 3f;
    private float currentCountdown;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentFuel = Mathf.Clamp(startingFuel, 0f, maxFuel);
        startPosition = playerTransform ? playerTransform.position : Vector3.zero;

        UpdateFuelUI();
        UpdateScoreUI();

        if (retryButton != null)
            retryButton.SetActive(false);

        SetState(GameState.HandSelection);
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case GameState.HandSelection:
                UpdateHandSelection();
                break;

            case GameState.Countdown:
                UpdateCountdown();
                break;

            case GameState.Playing:
                UpdatePlaying();
                break;

            case GameState.GameOver:
                UpdateGameOver();
                break;
        }
    }

    // =======================
    // STATE: HAND SELECTION
    // =======================
    private void OnEnterHandSelection()
    {
        SetStatusText("Select your hand", Color.white);

        SetUIPanels(
        showWelcome: true,
        showGameplay: false,
        showGameOver: false
    );
        if (retryButton != null)
            retryButton.SetActive(false);

        if (HandSelectionManager.Instance != null)
            HandSelectionManager.Instance.ResetSelection();
    }

    private void UpdateHandSelection()
    {
        // TEMP: tecla para simular selección
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Aquí luego conectamos el gesto
            StartCountdown();
        }
    }
    public void OnHandSelected(Chirality hand)
    {
        SelectedHand = hand;
        Debug.Log("GameManager received hand: " + hand);
        StartCountdown();
    }


    // =======================
    // STATE: COUNTDOWN
    // =======================
    private void OnEnterCountdown()
    {
        currentCountdown = countdownTimer;

        // Ocultamos el welcome cuando ya hay mano seleccionada
        SetUIPanels(
            showWelcome: false,
            showGameplay: false,
            showGameOver: false
        );
    }

    private void UpdateCountdown()
    {
        currentCountdown -= Time.deltaTime;

        SetStatusText($"Starting in {Mathf.CeilToInt(currentCountdown)}", Color.yellow);

        if (currentCountdown <= 0f)
        {
            SetState(GameState.Playing);
        }
    }

    // =======================
    // STATE: PLAYING
    // =======================
    private void OnEnterPlaying()
    {
        SetStatusText("Fly through the fuel rings!", Color.white);

        SetUIPanels(
        showWelcome: false,
        showGameplay: true,
        showGameOver: false
    );
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

    // =======================
    // STATE: GAME OVER
    // =======================
    private void OnEnterGameOver()
    {
        SetUIPanels(
       showWelcome: false,
       showGameplay: false,
       showGameOver: true
   );

        if (retryButton != null)
            retryButton.SetActive(true);
    }

    private void UpdateGameOver()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    // =======================
    // STATE MANAGEMENT
    // =======================
    public void SetState(GameState newState)
    {
        CurrentState = newState;

        switch (CurrentState)
        {
            case GameState.HandSelection:
                OnEnterHandSelection();
                break;

            case GameState.Countdown:
                OnEnterCountdown();
                break;

            case GameState.Playing:
                OnEnterPlaying();
                break;

            case GameState.GameOver:
                OnEnterGameOver();
                break;
        }
        Debug.Log($"[GameManager] State change: {CurrentState} → {newState}");
    }

    public void StartCountdown()
    {
        SetState(GameState.Countdown);
    }

    // =======================
    // GAMEPLAY
    // =======================
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

        SetStatusText($"{reason}\nPress R to retry", Color.green);
        SetState(GameState.GameOver);
    }

    // =======================
    // UTILS
    // =======================
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
