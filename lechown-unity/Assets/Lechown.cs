using UnityEngine;
using UnityEngine.UI;

public class Lechown : MonoBehaviour
{
    public static Lechown Instance { get; private set; }

    [Header("Splash Screen")]
    [SerializeField] private GameObject splashScreenRef;

    [Header("Sounds")]
    [SerializeField] private Button soundButton; 
    [SerializeField] private Sprite soundOnSprite; 
    [SerializeField] private Sprite soundOffSprite; 
    private bool isSoundOn; // Current sound state

    [Header("Info")]
    [SerializeField] private GameObject infoCanvas; 


    private void Awake() {
        // Ensure only one instance of Lechown exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: Keeps this object across scenes
    }

    private void Start() {

        if (SceneController.Instance.GetPreviousScene() != null) {
            splashScreenRef.SetActive(false);
        }

        closeInfoCanvas();
        InitializeSoundState();
    }

    public void OnPlayButtonClicked() {
        SceneController.Instance.SwitchScene("Main Scene");
    }

    public void OnInfoButtonClicked() {
        // Show info canvas
        infoCanvas.SetActive(true);
    }

    public void closeInfoCanvas() {
        // Close info canvas
        infoCanvas.SetActive(false);
    }

    void InitializeSoundState()
    {
        // Load the sound state from PlayerPrefs (default to ON if not set)
        isSoundOn = PlayerPrefs.GetInt("SoundState", 1) == 1;
        UpdateSoundStateUI();

        // // Add listener to sound button
        // if (soundButton != null)
        // {
        //     soundButton.onClick.AddListener(ToggleSound);
        // }
        // else
        // {
        //     Debug.LogError("Sound button is not assigned.");
        // }
    }

    public void ToggleSound() {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("SoundState", isSoundOn ? 1 : 0);
        UpdateSoundStateUI();
    }

    void UpdateSoundStateUI() {
        if (soundButton != null)
        {
            Image buttonImage = soundButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = isSoundOn ? soundOnSprite : soundOffSprite;
            }
        }

        // Mute or unmute audio globally
        // AudioListener.volume = isSoundOn ? 1 : 0;
    }
}
