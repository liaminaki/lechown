/*using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class JoinGame : MonoBehaviour
{

    [SerializeField] private InputField ipInputField;

    private void Awake()
    {
        string ipAddress = ipInputField.text;
        Network.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ipAddress,      //The IP address entered by the player
            7777            //Port number
        );

        NetworkManager.Singleton.StartClient();
    }

}
*/