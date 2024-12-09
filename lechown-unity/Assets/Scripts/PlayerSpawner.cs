using UnityEngine;
using Unity.Netcode;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject pigPrefab; // Pig player prefab
    [SerializeField] private GameObject manPrefab; // Man player prefab


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
        // Randomly choose between the pig and man prefabs
        GameObject playerPrefab = Random.Range(0, 2) == 0 ? pigPrefab : manPrefab;

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
