using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuUI;        // root canvas of start menu
    [SerializeField] private GameObject menuWindow;    // main menu window
    [SerializeField] private GameObject settingsWindow;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    private MonoBehaviour[] movementScripts;

    private void Start()
    {
        movementScripts = player.GetComponentsInChildren<MonoBehaviour>(true);

        foreach (var script in movementScripts)
        {
            if (script is Camera) continue;
            if (script is Animator) continue;

            if (script.GetType().Name.Contains("Move") ||
                script.GetType().Name.Contains("Look") ||
                script.GetType().Name.Contains("Shoot") ||
                script.GetType().Name.Contains("Collect") ||
                script.GetType().Name.Contains("Rifle") ||
                script.GetType().Name.Contains("Gun"))
            {
                script.enabled = false;
            }
        }

        menuUI.SetActive(true);
        menuWindow.SetActive(true);
        settingsWindow.SetActive(false);
    }

    public void StartGame()
    {
        menuUI.SetActive(false);

        AudioManager.Instance.PlayGameplayMusic();

        foreach (var script in movementScripts)
        {
            if (script == null) continue;

            if (script.GetType().Name.Contains("Move") ||
                script.GetType().Name.Contains("Look") ||
                script.GetType().Name.Contains("Shoot") ||
                script.GetType().Name.Contains("Collect") ||
                script.GetType().Name.Contains("Rifle") ||
                script.GetType().Name.Contains("Gun"))
            {
                script.enabled = true;
            }
        }
    }

    public void OpenSettings()
    {
        menuWindow.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void ShowSettingsFromPause()
    {
        menuUI.SetActive(true);

        menuWindow.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsWindow.SetActive(false);

        PauseManager pause = FindObjectOfType<PauseManager>();

        if (pause != null && pause.IsPaused)
        {
            menuUI.SetActive(false);
            pause.ShowPauseMenu();
        }
        else
        {
            menuWindow.SetActive(true);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
