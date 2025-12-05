using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject hudUI;

    public bool IsPaused { get; private set; } = false;

    private MenuManager menuManager;

    private void Awake()
    {
        menuManager = FindObjectOfType<MenuManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        if (hudUI != null) hudUI.SetActive(false);

        Time.timeScale = 0f;
        IsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (hudUI != null) hudUI.SetActive(true);

        Time.timeScale = 1f;
        IsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);

        if (menuManager != null)
        {
            menuManager.ShowSettingsFromPause();
        }
        else
        {
            Debug.LogWarning("PauseManager: MenuManager not found in scene.");
        }
    }
    public void ShowPauseMenu()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (hudUI != null) hudUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (menuManager != null)
        {
            menuManager.ShowMainMenuFromGame();
        }
        else
        {
            Debug.LogWarning("PauseManager: MenuManager not found when trying to QuitToMainMenu.");
        }
    }
}
