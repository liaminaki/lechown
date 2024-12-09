using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject pigPrefab; // Pig player prefab
    [SerializeField] private GameObject manPrefab; // Man player prefab

    private List<GameObject> availablePrefabs;

    private void Start()
    {
        // Check if the prefabs are assigned in the Inspector
        if (pigPrefab == null || manPrefab == null)
        {
            Debug.LogError("One or both player prefabs are not assigned in the Inspector!");
            return; // Exit early if prefabs are missing
        }

        // Initialize the list of available prefabs
        availablePrefabs = new List<GameObject> { pigPrefab, manPrefab };
        OnNetworkSpawn();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Only the server spawns players
        {
            // Ensure there are connected clients
            if (NetworkManager.Singleton.ConnectedClients.Count > 0)
            {
                foreach (var client in NetworkManager.Singleton.ConnectedClients)
                {
                    SpawnPlayer(client.Key);
                }
            }
            else
            {
                Debug.LogWarning("No clients are connected to the server.");
            }
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (availablePrefabs.Count == 0)
        {
            Debug.LogError("No prefabs available for spawning players.");
            return;
        }

        // Randomly choose a prefab from the available list
        int randomIndex = Random.Range(0, availablePrefabs.Count);
        GameObject playerPrefab = availablePrefabs[randomIndex];

        // Remove the chosen prefab from the list so it can't be reassigned
        availablePrefabs.RemoveAt(randomIndex);

        // Debug log which prefab was selected
        Debug.Log($"Selected Player Prefab: {playerPrefab.name}");

        // Instantiate the player prefab
        GameObject playerInstance = Instantiate(playerPrefab);

        // Assign the NetworkObject and spawn it
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"IsOwner: {networkObject.IsOwner}");
        }
        else
        {
            Debug.LogError("Player Prefab is missing a NetworkObject component!");
        }
    }
}
