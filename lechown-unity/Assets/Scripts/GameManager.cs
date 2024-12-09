using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{   
    public static GameManager Instance { get; private set; } // Static singleton instance

    [Header("Rounds")]
    private const int MAX_ROUNDS = 5;
    private int currentRound = 0;
    [SerializeField] private Sprite[] roundNumImg;
    [SerializeField] private GameObject roundNumRef;
    [SerializeField] private GameObject roundIntroRef;
    // Round transition state
	// Handles when two players collide with each other causing two round skips
    private bool isRoundTransitioning = false;

    [Header("Man")]
    private Player man;
    private Vector2 manStartPos = new Vector2(10, 0);
    
    [Header("Pig")]
    private Player pig;
    private Vector2 pigStartPos = new Vector2(-10, 0);

     [Header("Results")]
    [SerializeField] private GameObject resultCanvasRef;
    [SerializeField] private GameObject[] resultTypes;
    [SerializeField] private GameObject playerSpriteInResult;

    public enum ResultType
    {
        Lose,
        Win,
        Draw
    }

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of GameManager found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
 
        // Optional: Keep the GameManager persistent across scenes
        DontDestroyOnLoad(gameObject); 
    }

    void Start() {

        //Finding the GameObject for Man/Pig
        GameObject manObject = GameObject.Find("man(Clone)");
        GameObject pigObject = GameObject.Find("pig(Clone)");

        if(manObject != null){
            man = manObject.GetComponent<Player>();
            Debug.Log("Man GameObject Found");
        }
        else
            Debug.Log("GameObject named 'Man' not found!");

        if (pigObject != null){
            pig = pigObject.GetComponent<Player>();
            Debug.Log("Pig GameObject Found");
        }   
        else
            Debug.Log("GameObject named 'Pig' not found!");

        showResult(false);
        startNewRound();
    }

    // Resets the state for a new round
    public void startNewRound() {

        if (isRoundTransitioning) return; // Prevent simultaneous transitions

        isRoundTransitioning = true; // Lock transitions
        currentRound++;
        updRoundNumRef();

        // Wait for Round intro screen with countdown
        StartCoroutine(roundIntroDelay());
        
        roundIntroRef.SetActive(true);

        Debug.Log($"Starting Round {currentRound}");

        // Clear walls or any round-specific data
        clearWalls();

        // Reset player positions and states
        resetPlayer(man, manStartPos);
        resetPlayer(pig, pigStartPos);
    
        // Unlock transitions after a short delay
        StartCoroutine(UnlockRoundTransition());
    }

    private void updRoundNumRef () {
        SpriteRenderer renderer = roundNumRef.GetComponent<SpriteRenderer>();
        renderer.sprite = roundNumImg[currentRound - 1];
    }

    private IEnumerator roundIntroDelay() {
        yield return new WaitForSeconds(4f);
        roundIntroRef.SetActive(false);
        pig.startMovement();
        man.startMovement();
    }

    private IEnumerator UnlockRoundTransition()
    {
        yield return new WaitForSeconds(0.5f); // Delay for ensuring all collisions resolve
        isRoundTransitioning = false;
    }

    public void handleCollision() {

        pig.stopMovement();
        man.stopMovement();
        
        // Start the coroutine to handle the delay before starting a  new round or ending a game
        StartCoroutine(HandleCollisionWithDelay());

    }

    // Coroutine to handle the 2-second delay before starting a new round to show dead states
    private IEnumerator HandleCollisionWithDelay()
    {   
        // Delay until new round
        yield return new WaitForSeconds(2f);

        if (pig.lives > 0 && man.lives > 0) 
            startNewRound(); // Call startNewRound after the delay

        // End game if one of them has lost of their lives
        else 
            endGame();

    }

    public void endGame() {
        handleResult();
    }

    void handleResult() {
        showResult(true);
        SpriteRenderer renderer = playerSpriteInResult.GetComponent<SpriteRenderer>();

        // Draw
        if (pig.lives == 0 && man.lives == 0) {
            SetResultType(ResultType.Draw);
        }

        else {
            SetResultType(ResultType.Win);

            if (pig.lives != 0) 
                renderer.sprite = pig.sprite;
            else
                renderer.sprite = man.sprite;
        }
        
    }

    // Resets a player's position and state for the new round
    void resetPlayer(Player player, Vector2 startPos)
    {
        if (player != null)
        {
            player.transform.position = startPos;
            player.resetState(); // Ensure the player script has a method for resetting lives or other states
        }
    }

    void clearWalls() {
		// Find and destroy all wall objects
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }
	}

    void showResult(bool boolean) {
        resultCanvasRef.SetActive(boolean);
    }

    private void SetResultType(ResultType result)
    {
        for (int i = 0; i < resultTypes.Length; i++)
        {
            resultTypes[i].SetActive(i == (int)result);
        }
    }
}
