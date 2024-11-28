using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance { get; private set; } // Static singleton instance

    // Rounds
    private const int MAX_ROUNDS = 5;
    private int currentRound = 0;

    // Man 
    [SerializeField] private Player man;
    private Vector2 manStartPos = new Vector2(10, 0);
    
    // Pig
    [SerializeField] private Player pig;
    private Vector2 pigStartPos = new Vector2(-10, 0);

    // Round transition state
	// Handles when two players collide with each other causing two round skips
    private bool isRoundTransitioning = false;

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
        startNewRound();
    }

    // Resets the state for a new round
    public void startNewRound() {

        if (isRoundTransitioning) return; // Prevent simultaneous transitions

        isRoundTransitioning = true; // Lock transitions
        currentRound++;

        // Wait for Round intro screen with countdown
        StartCoroutine(roundIntroDelay());

        /*

            Insert here code for
            1. Current round Number
            2. 3-sec countdown
        */
        Debug.Log($"Starting Round {currentRound}");

        // Clear walls or any round-specific data
        clearWalls();

        // Reset player positions and states
        resetPlayer(man, manStartPos);
        resetPlayer(pig, pigStartPos);
    
        // Unlock transitions after a short delay
        StartCoroutine(UnlockRoundTransition());
    }

    private IEnumerator roundIntroDelay() {
        yield return new WaitForSeconds(3f);
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
        
        if (pig.lives > 0 && man.lives > 0) {
            // Start the coroutine to handle the delay before starting a  new round
            StartCoroutine(HandleCollisionWithDelay());
        }

        else 
            endGame();
    }

    // Coroutine to handle the 2-second delay before starting a new round to show dead states
    private IEnumerator HandleCollisionWithDelay()
    {   
        // Delay to ensure player has switched to dead state
        // yield return new WaitForSecondsRealtime(0.15f);

        // // Freeze the game by setting time scale to 0
        // Time.timeScale = 0;

        // Delay until new round
        yield return new WaitForSecondsRealtime(2f);

        // // Unfreeze the game by setting time scale back to 1
        // Time.timeScale = 1f;

        startNewRound(); // Call startNewRound after the delay
    }

    public void endGame() {
        Debug.Log("Game Over!");
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
}
