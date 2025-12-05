using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public GameObject hudUI; 

    private bool isPaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        Debug.Log("PAUSE GAME CALLED");
        pauseMenuUI.SetActive(true);
        hudUI.SetActive(false);

        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        hudUI.SetActive(true);

        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        pauseMenuUI.SetActive(false);
        hudUI.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();

        SceneManager.LoadScene("Main Menu");
    }
}