using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Assign your Player prefab in the Inspector

    public override void OnNetworkSpawn()
    {
        if (IsServer) // Only the server spawns players
        {
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnPlayer(clientId);
            }
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        // Instantiate the player prefab
        GameObject playerInstance = Instantiate(playerPrefab);
        

        // Assign the NetworkObject and spawn it
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.SpawnAsPlayerObject(clientId);
            Debug.Log($"IsOwner: {GetComponent<NetworkObject>().IsOwner}");

        }
        else
        {
            Debug.LogError("Player Prefab is missing a NetworkObject component!");
        }
    }
}
