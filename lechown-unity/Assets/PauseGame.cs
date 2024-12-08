using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pausePanel; // Reference to the pause screen panel
    private bool isPaused = false;

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the paused state
            if (isPaused)
                ResumeGame();
            else
                Pause();
        }
    }

    void Pause()
    {
        // Pause the game
        Time.timeScale = 0f;
        isPaused = true;

        // Show the pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        // Resume the game
        Time.timeScale = 1f;
        isPaused = false;

        // Hide the pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("Game Resumed");
    }

    public void GoHome()
    {
        SceneController.Instance.SwitchScene("Home");
    }
}
