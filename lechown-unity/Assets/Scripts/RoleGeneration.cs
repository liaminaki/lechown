using UnityEngine;
using Unity.Netcode;

public class RoleGeneration : MonoBehaviour
{
    [Header("Character GameObjects")]
    public GameObject pigCharacter;   // Assign the Pig character GameObject in the Inspector
    public GameObject manCharacter;  // Assign the Man character GameObject in the Inspector

    private void Start()
    {
        // Ensure both characters start hidden
        pigCharacter.SetActive(false);
        manCharacter.SetActive(false);

        // Get the client ID of the current player
        ulong clientId = NetworkManager.Singleton.LocalClientId;

        // Retrieve the role of this client
        HostGame.Role assignedRole = HostGame.Instance.GetRole(clientId);

        // Show the corresponding character for the assigned role
        ShowCharacterForRole(assignedRole);
    }

    private void ShowCharacterForRole(HostGame.Role role)
    {
        switch (role)
        {
            case HostGame.Role.Pig:
                pigCharacter.SetActive(true);
                manCharacter.SetActive(false);
                break;

            case HostGame.Role.Man:
                manCharacter.SetActive(true);
                pigCharacter.SetActive(false);
                break;

            default:
                Debug.LogError($"Unknown role: {role}");
                pigCharacter.SetActive(false);
                manCharacter.SetActive(false);
                break;
        }
    }
}