using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; } // Static singleton instance

    [Header("Rounds")]
    private const int MAX_ROUNDS = 5;
    public NetworkVariable<int> currentRound = new NetworkVariable<int>(0);
    [SerializeField] private Sprite[] roundNumImg;
    [SerializeField] private GameObject roundNumRef;
    [SerializeField] private GameObject roundIntroRef;
    // Round transition state
    // Handles when two players collide with each other causing two round skips
    private bool isRoundTransitioning = false;

    [Header("Man")]
    public Player man;
    private Vector2 manStartPos = new Vector2(10, 0);

    [Header("Pig")]
    public Player pig;
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

            if (IsServer)
            {
                NetworkObject networkObject = GetComponent<NetworkObject>();

                if (networkObject != null)
                {
                    networkObject.Spawn();
                }
                else
                {
                    Debug.LogError("GameManager does not have a NetworkObject component or already spawned!");
                }
            }
        }
        else
        {
            Debug.LogError("Multiple instances of GameManager found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

    }

    void Start()
    {
        InitPlayerObjects();
        InitializeResultUI();
        showResult(false);
        startNewRound();
        // StartCoroutine(StartNewRoundWithDelay());
    }

    IEnumerator StartNewRoundWithDelay()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        startNewRound();
    }

    void InitPlayerObjects()
    {
        //Finding the GameObject for Man/Pig
        GameObject manObject = GameObject.Find("man(Clone)");
        GameObject pigObject = GameObject.Find("pig(Clone)");

        if (manObject != null)
        {
            man = manObject.GetComponent<Player>();
            Debug.Log("Man GameObject Found");
        }
        else
            Debug.Log("GameObject named 'Man' not found!");

        if (pigObject != null)
        {
            pig = pigObject.GetComponent<Player>();
            Debug.Log("Pig GameObject Found");
        }
        else
            Debug.Log("GameObject named 'Pig' not found!");
    }

    // Resets the state for a new round
    public void startNewRound()
    {

        if (isRoundTransitioning) return; // Prevent simultaneous transitions

        isRoundTransitioning = true; // Lock transitions

        if (IsServer)
            currentRound.Value++;

        // Wait for Round intro screen with countdown
        StartCoroutine(roundIntroDelay());

        ShowRoundIntroServerRpc(true, currentRound.Value);

        Debug.Log($"Starting Round {currentRound.Value}");

        // Clear walls or any round-specific data
        clearWalls();

        // Reset player positions and states
        resetPlayer(man, manStartPos);
        resetPlayer(pig, pigStartPos);

        // Unlock transitions after a short delay
        StartCoroutine(UnlockRoundTransition());
    }

    [ServerRpc(RequireOwnership = false)]
    void ShowRoundIntroServerRpc(bool boolean, int round)
    {
        showRoundIntro(boolean, round);
        ShowRoundIntroClientRpc(boolean, round);
    }

    [ClientRpc]
    void ShowRoundIntroClientRpc(bool boolean, int round)
    {
        if (IsServer) return;
        showRoundIntro(boolean, round);
    }

    void showRoundIntro(bool boolean, int round)
    {
        if (boolean)
            updRoundNumRef(round);
        roundIntroRef.SetActive(boolean);
    }

    private void updRoundNumRef(int round)
    {
        SpriteRenderer renderer = roundNumRef.GetComponent<SpriteRenderer>();
        renderer.sprite = roundNumImg[round - 1];
    }

    private IEnumerator roundIntroDelay()
    {
        yield return new WaitForSeconds(4f);
        ShowRoundIntroServerRpc(false, 0);
        pig.StartMovementServerRpc();
        man.StartMovementServerRpc();
    }

    private IEnumerator UnlockRoundTransition()
    {
        yield return new WaitForSeconds(0.5f); // Delay for ensuring all collisions resolve
        isRoundTransitioning = false;
    }

    public void handleCollision()
    {

        pig.StopMovementServerRpc();
        man.StopMovementServerRpc();

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

    public void endGame()
    {
        clearWalls();

        if (IsServer)
        {
            // Determine results and send to clients
            ResultType resultTypeForPig = pig.lives > 0 ? ResultType.Win : (man.lives > 0 ? ResultType.Lose : ResultType.Draw);
            ResultType resultTypeForMan = man.lives > 0 ? ResultType.Win : (pig.lives > 0 ? ResultType.Lose : ResultType.Draw);

            // Notify clients
            DisplayResultClientRpc(resultTypeForPig, resultTypeForMan);

            // Handle result display for the server if it is also a player
            DisplayResultServerRpc(resultTypeForPig, resultTypeForMan);
        }
    }

    [ClientRpc]
    private void DisplayResultClientRpc(ResultType pigResult, ResultType manResult)
    {
        if (IsServer) return;
        displayResult(pigResult, manResult);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisplayResultServerRpc(ResultType pigResult, ResultType manResult)
    {
        displayResult(pigResult, manResult);
        DisplayResultClientRpc(pigResult, manResult);
    }

    void displayResult(ResultType pigResult, ResultType manResult)
    {
        if (NetworkManager.Singleton.LocalClientId == pig.OwnerClientId)
            ShowResultUI(pigResult, pig.sprite);

        else if (NetworkManager.Singleton.LocalClientId == man.OwnerClientId)
            ShowResultUI(manResult, man.sprite);
    }

    // Show the result UI with the appropriate type and sprite
    void ShowResultUI(ResultType result, Sprite playerSprite)
    {
        showResult(true);

        // Update the result UI
        SetResultType(result);

        SpriteRenderer renderer = playerSpriteInResult.GetComponent<SpriteRenderer>();

        if (result == ResultType.Win || result == ResultType.Lose)
            renderer.sprite = playerSprite;

        else
            renderer.sprite = null;

        // Hide the game objects for man and pig
        man.gameObject.SetActive(false);
        pig.gameObject.SetActive(false);


    }

    // Resets a player's position and state for the new round
    void resetPlayer(Player player, Vector2 startPos)
    {
        if (player != null)
        {
            // player.transform.position = startPos;
            player.ResetStateServerRpc(startPos); // Ensure the player script has a method for resetting lives or other states
        }
    }

    void clearWalls()
    {
        // Find and destroy all wall objects
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            Destroy(wall);
        }
    }

    void showResult(bool boolean)
    {
        resultCanvasRef.SetActive(boolean);
    }

    void InitializeResultUI()
    {
        for (int i = 0; i < resultTypes.Length; i++)
        {
            resultTypes[i].SetActive(false);
        }
    }

    private void SetResultType(ResultType result)
    {
        for (int i = 0; i < resultTypes.Length; i++)
        {
            resultTypes[i].SetActive(i == (int)result);
        }
    }

    public void GoHome()
    {
        // Clean up the network session
        NetworkManager.Singleton.Shutdown();

        // Destroy the NetworkManager to avoid carrying it into the menu scene
        Destroy(NetworkManager.Singleton.gameObject);

        // Load the menu scene
        SceneController.Instance.SwitchScene("Home");
    }
}
