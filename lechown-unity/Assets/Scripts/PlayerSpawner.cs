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
                    // Get the role of the client and spawn the appropriate player prefab
                    HostGame.Role clientRole = HostGame.Instance.GetRole(client.Key);
                    SpawnPlayer(client.Key, clientRole);
                }
            }
            else
            {
                Debug.LogWarning("No clients are connected to the server.");
            }
        }
    }

    private void SpawnPlayer(ulong clientId, HostGame.Role clientRole)
    {
        GameObject playerPrefab = null;

        // Select the player prefab based on the client's role
        switch (clientRole)
        {
            case HostGame.Role.Pig:
                playerPrefab = pigPrefab;
                break;

            case HostGame.Role.Man:
                playerPrefab = manPrefab;
                break;

            default:
                Debug.LogError($"Unknown role: {clientRole}");
                return;
        }

        if (playerPrefab != null)
        {
            // Instantiate the correct player prefab based on the role
            GameObject playerInstance = Instantiate(playerPrefab);

            // Assign the NetworkObject and spawn it
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
                Debug.Log($"Spawned player with role {clientRole} as {playerPrefab.name}. IsOwner: {networkObject.IsOwner}");
            }
            else
            {
                Debug.LogError("Player Prefab is missing a NetworkObject component!");
            }
        }
    }
}
