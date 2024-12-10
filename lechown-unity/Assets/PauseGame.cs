using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseGame : MonoBehaviour
{
    public GameObject pausePanel; // Reference to the pause screen panel
    private bool isPaused = false;

    // NetworkVariable is initialized properly
    private NetworkVariable<bool> isMultGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerPausedDictionary;
    private NetworkManager networkManager;  // Reference to NetworkManager

    private void Awake()
    {
        // Try to find the NetworkManager manually if Singleton is not set
        networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            // If Singleton is still null, search for the NetworkManager GameObject
            networkManager = FindObjectOfType<NetworkManager>();
            if (networkManager == null)
            {
                Debug.LogError("NetworkManager is not found in the scene!");
            }
        }

        playerPausedDictionary = new Dictionary<ulong, bool>();

        // Ensure NetworkVariable is initialized
        if (isMultGamePaused == null)
        {
            isMultGamePaused = new NetworkVariable<bool>(false);
        }

        // Subscribe to the NetworkVariable's value change to react on pause/unpause
        isMultGamePaused.OnValueChanged += OnPauseStateChanged;
    }

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
        if (networkManager.IsServer)  // Ensure only the server handles this
        {
            PauseGameServerRpc();
            Time.timeScale = 0f;
            isPaused = true;

            // Show the pause panel
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
            }

            Debug.Log("Game Paused");
        }
    }

    public void ResumeGame()
    {
        // Resume the game
        if (networkManager.IsServer)  // Ensure only the server handles this
        {
            UnpauseGameServerRpc();
            Time.timeScale = 1f;
            isPaused = false;

            // Hide the pause panel
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }

            Debug.Log("Game Resumed");
        }
    }

    public void GoHome()
    {
        SceneController.Instance.SwitchScene("Home");
    }

    private void OnEnable()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback += OnClientConnected;
            networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDisable()
    {
        if (networkManager != null)
        {
            networkManager.OnClientConnectedCallback -= OnClientConnected;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        // Unsubscribe when this object is disabled
        isMultGamePaused.OnValueChanged -= OnPauseStateChanged;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!playerPausedDictionary.ContainsKey(clientId))
            playerPausedDictionary[clientId] = false;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (playerPausedDictionary.ContainsKey(clientId))
            playerPausedDictionary.Remove(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Update the global pause state for all clients
        isMultGamePaused.Value = true;
        TestGamePausedState();  // Check if any player is paused
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        // Update the global pause state for all clients
        isMultGamePaused.Value = false;
        TestGamePausedState();  // Check if any player is paused
    }

    private void TestGamePausedState()
    {
        // Ensure the networkManager is not null before checking connected clients
        if (networkManager != null)
        {
            bool isAnyPlayerPaused = false;

            // Loop through all connected clients and check if any is paused
            foreach (var client in networkManager.ConnectedClientsList)
            {
                ulong clientId = client.ClientId;
                if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
                {
                    isAnyPlayerPaused = true;
                    break;
                }
            }

            // Update the global paused state based on player statuses
            isMultGamePaused.Value = isAnyPlayerPaused;
        }
        else
        {
            Debug.LogError("NetworkManager is not initialized.");
        }
    }

    // This method is called when the pause state changes
    private void OnPauseStateChanged(bool previousValue, bool newValue)
    {
        if (!networkManager.IsServer)  // Only handle this on clients
        {
            // Pause or unpause based on the global state
            if (newValue)
            {
                Time.timeScale = 0f;
                isPaused = true;
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                }
            }
            else
            {
                Time.timeScale = 1f;
                isPaused = false;
                if (pausePanel != null)
                {
                    pausePanel.SetActive(false);
                }
            }
        }
    }
}
