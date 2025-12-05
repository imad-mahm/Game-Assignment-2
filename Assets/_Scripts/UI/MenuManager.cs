using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Windows")]
    public GameObject menuWindow;        // main menu buttons
    public GameObject settingsWindow;    // settings panel

    private void Start()
    {
        // make sure we are showing the menu when the scene loads
        menuWindow.SetActive(true);
        settingsWindow.SetActive(false);

        // play the menu music every time we open this scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();   // play menu music
        }

        Cursor.lockState = CursorLockMode.None;      // unlock mouse in menu
        Cursor.visible = true;
    }

    public void StartGame()
    {
        // switch to gameplay scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();   // play gameplay music when starting
        }

        LoadNextLevel();  // load the game scene
    }

    private void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
    }

    public void OpenSettings()
    {
        // hide main buttons and show settings panel
        menuWindow.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void CloseSettings()
    {
        // go back to main menu
        settingsWindow.SetActive(false);
        menuWindow.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}