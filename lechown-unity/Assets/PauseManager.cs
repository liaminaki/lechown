using UnityEngine;
using Unity.Netcode;

public class PauseManager : NetworkBehaviour
{
    private bool isPaused = false;
    private ulong pausingClientId = 0; // Tracks which client triggered the pause

    public GameObject pausePanelForTriggeringClient; // Panel for the client who paused
    public GameObject pausePanelForOtherClients;    // Panel for other clients

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed");
            RequestTogglePauseServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTogglePauseServerRpc(ulong clientId)
    {
        Debug.Log($"Pause requested by client {clientId}");
        pausingClientId = clientId; // Track the client who paused
        TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Debug.Log($"Toggling pause: {isPaused}");
        UpdatePauseStateClientRpc(isPaused, pausingClientId);
    }

    [ClientRpc]
    private void UpdatePauseStateClientRpc(bool pauseState, ulong pausingClientId)
    {
        isPaused = pauseState;

        // Enable appropriate panels based on which client triggered the pause
        if (NetworkManager.Singleton.LocalClientId == pausingClientId)
        {
            pausePanelForTriggeringClient.SetActive(isPaused);
            pausePanelForOtherClients.SetActive(false);
        }
        else
        {
            pausePanelForTriggeringClient.SetActive(false);
            pausePanelForOtherClients.SetActive(isPaused);
        }

        // Freeze or resume the game
        Time.timeScale = isPaused ? 0 : 1;
        Debug.Log($"Game paused: {isPaused}, Pausing Client ID: {pausingClientId}");
    }

    // Function to resume the game
    public void ResumeGame()
    {
        if (isPaused)
        {
            Debug.Log("Resume button clicked");
            RequestTogglePauseServerRpc(pausingClientId); // Call the pause toggle via ServerRpc
        }
    }

    // Example function for switching scenes
    public void GoHome()
    {
        SceneController.Instance.SwitchScene("Home");
    }
}
