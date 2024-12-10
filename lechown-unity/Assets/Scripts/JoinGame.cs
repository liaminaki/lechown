using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
/*using System.Text.RegularExpressions;
using System.Linq;*/
  

public class JoinGame : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI inputField;
    [SerializeField] private Button joinGameButton;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] UnityTransport transport;
    [SerializeField] private Button backButton;
    [SerializeField] GameObject gameOption;
    [SerializeField] GameObject gameLobby;

    private HostGame hostGameScript;
    string iPAddress;

    //private const string IpPattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

    private void Awake()
    {
        hostGameScript = gameLobby.GetComponent<HostGame>();
        statusText.gameObject.SetActive(false);
        joinGameButton.onClick.AddListener(() =>
        {
            JoinGameClicked();
        });

        backButton.onClick.AddListener(() => {
            gameOption.SetActive(true);
            gameObject.SetActive(false);
        } );
    }

    private void JoinGameClicked()
    {
        iPAddress = inputField.text.Trim();
        Debug.Log("current IP: " +iPAddress);

        /*if (string.IsNullOrEmpty(iPAddress))
        {
            Debug.LogError("Invalid IP Address");
            statusText.gameObject.SetActive(true);
            statusText.color = Color.red;
            statusText.text = "IP Address cannot be empty";
            return;
        }

*//*        string restrictedIPAddress = "127.0.0.1"; // for testing purposes in local machine use this!!!*/

/*        if (!IsValidIPAddress(iPAddress))
        {
            Debug.LogError("Invalid IP Address");
            statusText.gameObject.SetActive(true);
            statusText.color = Color.red;
            statusText.text = "Invalid IP Address";
            return;
        }*/

       if (transport != null)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                iPAddress,  // IP Address inputted
                7777        // Port
                );
            Debug.Log($"Attempting to connect to host at {iPAddress}");
        }

        // Start the client and try to connect to the server (host)
        NetworkManager.Singleton.StartClient();

        // Optionally show a "Connecting..." message while the client is attempting to join.
        statusText.gameObject.SetActive(true);
        statusText.color = Color.green;
        statusText.text = "Connecting...";



        // Register network events to check if the connection is successful or fails
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // Function to validate the input IP address using regex
    /*private bool IsValidIPAddress(string ipAddress)
    {
        // Remove invisible characters or non-printing characters
        ipAddress = string.Concat(ipAddress.Where(c => !char.IsControl(c))); // Remove control characters

        // Match the IP address against the IPv4 regex pattern
        bool isValid = Regex.IsMatch(ipAddress, IpPattern);
        Debug.Log($"IP Address '{ipAddress}' is valid: {isValid}");
        return isValid;
    }*/

    private void OnClientConnected(ulong clientId)
    {
        // This callback is called when the client successfully connects to the host
        Debug.Log("Client connected to the host.");
        statusText.text = "Connected to host!";

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            // Send the player's name and initial status to the host
            SendPlayerNameToHost();
        }

        gameLobby.SetActive(true);
        hostGameScript.SetIPAddress(iPAddress);
        gameObject.SetActive(false);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        // This callback is called if the client disconnects from the host
        Debug.LogError("Failed to connect or disconnected.");
        statusText.color = Color.red;
        statusText.text = "Failed to connect to host.";
    }

    private void SendPlayerNameToHost()
    {
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            AddPlayerServerRpc("Client", "NOT READY");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerServerRpc(string playerName, string status)
    {
        Debug.Log($"Player {playerName} joined the game with status: {status}");
    }

    // Make sure to clean up
    /*    private void OnDestroy()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }*/
}
