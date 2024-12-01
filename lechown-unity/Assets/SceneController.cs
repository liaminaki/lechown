using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static string previousScene;
    public static SceneController Instance;

    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Preserve across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchScene(string sceneName)
    {
        // Store the current scene as the previous scene
        previousScene = SceneManager.GetActiveScene().name;

        // Load the new scene
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("No previous scene to return to.");
        }
    }

    public string GetPreviousScene()
    {
        return previousScene;
    }
}
