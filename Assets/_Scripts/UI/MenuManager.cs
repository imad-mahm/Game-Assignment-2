using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("UI Windows")]
    public GameObject menuWindow;        // main menu buttons
    public GameObject settingsWindow;    // settings panel

    private void Start()
    {
        menuWindow.SetActive(true);
        settingsWindow.SetActive(false);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic(); 
        }

        Cursor.lockState = CursorLockMode.None;     
        Cursor.visible = true;
    }

    public void StartGame()
    {
        // switch to gameplay scene
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        LoadNextLevel(); 
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
        menuWindow.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsWindow.SetActive(false);
        menuWindow.SetActive(true);
    }
    
    public void LoadGame()
    {
        if (!SaveManager.SaveExists())
        {
            Debug.LogWarning("No save file found to load.");
            return;
        }

        PlayerPrefs.SetInt("LoadGameFlag", 1);

        // Switch to gameplay music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
        }

        LoadNextLevel();
    }


    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}