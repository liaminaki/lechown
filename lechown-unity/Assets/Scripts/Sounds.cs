using UnityEngine;
using UnityEngine.SceneManagement;

public class Sounds : MonoBehaviour
{
    private static Sounds instance;
    public AudioSource abgmtsource;
    public AudioSource bgmSource;  // Reference to the AudioSource component
    public AudioClip[] bgmTracks;  // Array of music tracks
    private string currentSceneName;

    public AudioClip buttonClickAudio;
    public AudioClip turn;
    public AudioClip collision;
    public AudioClip Win;
    public AudioClip Lose;
    public AudioClip play;
    public AudioClip countDown;

    public AudioSource sfxSource;
    void Awake()
    {
        // Ensure only one instance of Sounds exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    void Start()
    {
        // Initialize the scene and play the initial background music
        currentSceneName = SceneManager.GetActiveScene().name;
        //PlayMusicForScene(currentSceneName);
    }

    void Update()
    {
        // Check if the scene has changed
        if (SceneManager.GetActiveScene().name != currentSceneName)
        {
            print("working");
            currentSceneName = SceneManager.GetActiveScene().name;
            PlayMusicForScene(currentSceneName);
        }
    }

    // Play the background music based on the current scene
    public void PlayMusicForScene(string sceneName)
    {
        // Find the track for the scene (you can expand this to support more scenes)
        AudioClip newTrack = null;
        bool rolegen = false;

        switch (sceneName)
        {
            case "Home":
                newTrack = bgmTracks[0]; // Example: Main Scene music
                break;
            case "Main Scene":
                newTrack = bgmTracks[1]; // Example: Menu Scene music
                break;
            case "randomGen":
                newTrack = bgmTracks[2];
                rolegen = true;
                break;
            case "YouAreThe":
                newTrack = bgmTracks[3];
                rolegen = true;
                break;
            // Add more cases for other scenes as needed
            default:
                newTrack = bgmTracks[0]; // Default track
                break;
        }

        if (newTrack != null && bgmSource != null)
        {
            if (rolegen)
            {
                abgmtsource.clip = newTrack;
                abgmtsource.Play();
            }
            else
            {
                bgmSource.clip = newTrack;  // Change the music clip
                bgmSource.Play();           // Play the new music
            }
        }
        else
        {
            Debug.LogError("No AudioSource or Music Track assigned.");
        }
        rolegen = false;
    }

    public void PlayButtonClickAudio()
    {
        sfxSource.clip = buttonClickAudio;
        sfxSource.Play();
    }

    public void PlayWin()
    {
        sfxSource.clip = Win;
        sfxSource.Play();
    }

    public void PlayLose()
    {
        sfxSource.clip = Lose;
        sfxSource.Play();
    }

    public void PlayCollision()
    {
        sfxSource.clip = collision;
        sfxSource.Play();
    }

    public void PlayTurn()
    {
        sfxSource.clip = turn;
        sfxSource.Play();
    }

    public void PlayPlay()
    {
        sfxSource.clip = play;
        sfxSource.Play();
    }

    public void PlayCountdown()
    {
        sfxSource.clip = countDown;
        sfxSource.Play();
    }
}
