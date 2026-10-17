using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    Paused,
    Won,
    Lost
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        SetGameState(GameState.Playing);
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.pKey.wasPressedThisFrame)
            return;

        if (CurrentState == GameState.Playing)
            PauseGame();
        else if (CurrentState == GameState.Paused)
            ResumeGame();
    }

    public void SetGameState(GameState newState)
    {
        CurrentState = newState;

        Time.timeScale = newState == GameState.Paused ||
                         newState == GameState.Won ||
                         newState == GameState.Lost
            ? 0f
            : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(newState == GameState.Paused);

        if (winPanel != null)
            winPanel.SetActive(newState == GameState.Won);

        if (losePanel != null)
            losePanel.SetActive(newState == GameState.Lost);
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
            return;

        SetGameState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
            return;

        SetGameState(GameState.Playing);
    }

    public void WinGame()
    {
        if (CurrentState != GameState.Playing)
            return;

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.victory, 0.8f);

        SetGameState(GameState.Won);
    }

    public void LoseGame()
    {
        if (CurrentState != GameState.Playing)
            return;

        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.Library.defeat, 0.8f);

        SetGameState(GameState.Lost);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
