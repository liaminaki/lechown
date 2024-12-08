using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lechown : MonoBehaviour
{
    public static Lechown Instance { get; private set; }
    
    [Header("Username")]
    private const string UsernameKey = "Username";
    public string Username { get; private set; } 

    [Header("UI")]
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button soundButton; 
    [SerializeField] private Sprite soundOnSprite; 
    [SerializeField] private Sprite soundOffSprite; 
    private bool isSoundOn; // Current sound state
    [SerializeField] private GameObject doneButton;

    [Header("Canvas")]
    [SerializeField] private GameObject splashScreenRef;
    [SerializeField] private GameObject infoCanvas; 
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject blackBG;
    [SerializeField] private GameObject usernameCard;


    private void Awake() {
        // Ensure only one instance of Lechown exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject); // Optional: Keeps this object across scenes
    }

    private void Start() {

        if (SceneController.Instance.GetPreviousScene() != null) {
            splashScreenRef.SetActive(false);
        }

        InitUsername();
        blackBG.SetActive(false);
        closeInfoCanvas();
        InitSoundState();
    }

    void Update() {
        UpdateDoneButtonState();
    }

    void InitUsername() {
        
        UpdateUsername();

        // Add listener to save the username when input field value changes
        usernameInputField.onValueChanged.AddListener(OnUsernameChanged);
        usernameInputField.onDeselect.AddListener(OnInputFieldDeselected); // Unfocused
    }

    void UpdateUsername() {

        // Check if a username exists in PlayerPrefs
        if (PlayerPrefs.HasKey(UsernameKey))
        {
            Username = PlayerPrefs.GetString(UsernameKey); 
            usernameInputField.text = Username; 
        }
        else
        {
            Username = ""; 
        }
    }

    void OnInputFieldDeselected(string text) {
        if (infoCanvas.activeSelf)
            UpdateUsername();
    }

    void OnUsernameChanged(string username) {

        // Check if the input is not empty or whitespace
        if (!string.IsNullOrWhiteSpace(username)) {
            Username = username; 
            PlayerPrefs.SetString(UsernameKey, username);
            PlayerPrefs.Save(); 
        }
        
        else {
            Debug.LogWarning("Username cannot be empty."); 
        }
    }

    bool IsUsernameSet() {
        return !string.IsNullOrWhiteSpace(Username);
    }

    void showUsernameCard() {
        usernameCard.SetActive(true); 

        // If username is not set and not in info canvas
        if (!IsUsernameSet() && !infoCanvas.activeSelf) {
            usernameCard.transform.position = new Vector2(0,0); // Move card to position
            blackBG.SetActive(true);
            doneButton.SetActive(true);
        }

        // In info canvas
        else {
            RectTransform usernameCardRectTransform = usernameCard.GetComponent<RectTransform>();
            usernameCardRectTransform.anchoredPosition = new Vector2(-380, -43); 
            doneButton.SetActive(false); 
        }
    }

    void closeUsernameCard() {
        usernameCard.SetActive(false);
    }

    private void OnDestroy() {
        // Remove listener to avoid memory leaks
        usernameInputField.onValueChanged.RemoveListener(OnUsernameChanged);
    }

    public void ClearUsername()
    {
        Username = ""; 
        usernameInputField.text = ""; 
        PlayerPrefs.DeleteKey(UsernameKey);
        UpdateUsername();
        Debug.Log("Username cleared.");
    }

    void UpdateDoneButtonState() {
        Button doneButtonComponent = doneButton.GetComponent<Button>();

        if (string.IsNullOrWhiteSpace(usernameInputField.text)) 
            doneButtonComponent.interactable = false; 

        else
            doneButtonComponent.interactable = true;
    }

    public void OnPlayButtonClicked() {

        if (IsUsernameSet())
            SceneController.Instance.SwitchScene("Main Scene");

        else
            showUsernameCard();
    
    }

    public void OnInfoButtonClicked() {
        infoCanvas.SetActive(true);
        showUsernameCard();
        blackBG.SetActive(true);
        buttons.SetActive(false);
    }

    public void closeInfoCanvas() {
        infoCanvas.SetActive(false);
        closeUsernameCard();
        blackBG.SetActive(false);
        buttons.SetActive(true);
    }

    void InitSoundState() {
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
