using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private GameObject menuUI;

    [SerializeField] private GameObject menuWindow;
    [SerializeField] private GameObject settingsWindow;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    private MonoBehaviour[] movementScripts;
    private PauseManager pauseManager;

    private bool openedFromPause = false;

    private void Awake()
    {
        pauseManager = FindObjectOfType<PauseManager>();
    }

    private void Start()
    {
        if (player != null)
        {
            movementScripts = player.GetComponentsInChildren<MonoBehaviour>(true);
        }

        SetPlayerControls(false);
        Time.timeScale = 1f; 

        if (menuUI != null)        menuUI.SetActive(true);
        if (menuWindow != null)    menuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

   
    public void StartGame()
    {
        if (menuUI != null) menuUI.SetActive(false);

        SetPlayerControls(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }
    }

    public void OpenSettings()
    {
        openedFromPause = false;

        if (menuWindow != null)     menuWindow.SetActive(false);
        if (settingsWindow != null) settingsWindow.SetActive(true);
    }

    public void ShowSettingsFromPause()
    {
        openedFromPause = true;

        if (menuUI != null)         menuUI.SetActive(true);
        if (menuWindow != null)     menuWindow.SetActive(false);
        if (settingsWindow != null) settingsWindow.SetActive(true);

    }

    
    public void CloseSettings()
    {
        if (settingsWindow != null) settingsWindow.SetActive(false);

        if (openedFromPause)
        {
            openedFromPause = false;

            if (menuUI != null) menuUI.SetActive(false); // Hide main menu canvas

            if (pauseManager != null && pauseManager.IsPaused)
            {
                pauseManager.ShowPauseMenu();
            }
        }
        else
        {
            // We came from Main Menu
            if (menuWindow != null) menuWindow.SetActive(true);
        }
    }
    public void ShowMainMenuFromGame()
    {
        SetPlayerControls(false);

        if (menuUI != null)         menuUI.SetActive(true);
        if (menuWindow != null)     menuWindow.SetActive(true);
        if (settingsWindow != null) settingsWindow.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void SetPlayerControls(bool enable)
    {
        if (movementScripts == null) return;

        foreach (var script in movementScripts)
        {
            if (script == null) continue;

            if (script is Animator) continue;

            string name = script.GetType().Name;

            if (name.Contains("Move") ||
                name.Contains("Look") ||
                name.Contains("Shoot") ||
                name.Contains("Collect") ||
                name.Contains("Rifle") ||
                name.Contains("Gun"))
            {
                script.enabled = enable;
            }
        }
    }
}
