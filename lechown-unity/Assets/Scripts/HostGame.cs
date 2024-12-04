using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using UnityEngine.SceneManagement;

public class HostGame : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI iPAddress;
    [SerializeField] TextMeshProUGUI playerCount;
    [SerializeField] Button backButton;
    [SerializeField] Button startButton;
    [SerializeField] GameObject gameOption;

    [SerializeField] private Transform playerListContainer;
    [SerializeField] private Transform playerTemplate;

    public string currentPlayerCount;

    // Max number of players allowed (host + 1 client)
    private const int MaxPlayers = 2;


    public void Awake()
    {
        currentPlayerCount = GetPlayerCount().ToString();
        playerTemplate.gameObject.SetActive(false);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Ensure the player list is updated even if the client is already connected
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
        {
            UpdatePlayerList();
        }

    }

    private void Update()
    {
        playerCount.text = GetPlayerCount().ToString() + "/2 PLAYERS";

        //Disable Start button if less than 2 players are connected
        startButton.interactable = GetPlayerCount() == 2;

        backButton.onClick.AddListener(() =>
        {
            gameOption.SetActive(true);
            gameObject.SetActive(false);
        });

        // Check if this client is the host and show/hide the start button
        if (NetworkManager.Singleton.IsHost)
        {
            startButton.gameObject.SetActive(true);  // Show start button for host
        }
        else
        {
            startButton.gameObject.SetActive(false); // Hide start button for non-host clients
        }

        startButton.onClick.AddListener(() =>
        {
            string scene = "Main Scene";
            SceneManager.LoadScene(scene);
        });
    } 

    public void StartHostGame()
    {
        // Get local IP Address
        string localIPAddress = GetLocalIPAddress();

        if (string.IsNullOrEmpty(localIPAddress))
        {
            Debug.LogError("Failed to retrieve local IP address. Host not started.");
            return;
        }

        // Configure Unity Transport to Use the Device IP Address
        /*NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            localIPAddress, // IP Address of the Device
            7777            // Port
        );*/

        // Start the host
        NetworkManager.Singleton.StartHost();
        iPAddress.text = localIPAddress;
        Debug.Log("Host started successfully!");

        UpdatePlayerList();
    }

    public void SetIPAddress(string clientInputIPAddress)
    {
        iPAddress.text = clientInputIPAddress;
    }

    private string GetLocalIPAddress()
    {
        try
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // Check for IPv4 address
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new System.Exception("No IPv4 network adapters with an IP address in the system!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error retrieving IP address: {ex.Message}");
            return null;
        }
    }

    private int GetPlayerCount()
    {
        return NetworkManager.Singleton.ConnectedClients.Count;
    }

    private void OnClientConnected(ulong clientId)
    {
        // A new client has connected, update the player list
        Debug.Log($"Client {clientId} connected!");

        // Check if the number of connected players exceeds the max allowed
        if (GetPlayerCount() > MaxPlayers)
        {
            // Disconnect the client if the max number of players is exceeded
            NetworkManager.Singleton.DisconnectClient(clientId);
            Debug.Log($"Client {clientId} was disconnected because the maximum number of players has been reached.");
        }

        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Clear existing player entries to avoid duplicates
        foreach (Transform child in playerListContainer)
        {
            if (child != playerTemplate) // Skip the template
            {
                Destroy(child.gameObject);
            }
        }

        // Add players dynamically to the list
        float templateHeight = 120f;
        int index = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Transform playerTransform = Instantiate(playerTemplate, playerListContainer);
            RectTransform playerRectTransform = playerTransform.GetComponent<RectTransform>();
            playerRectTransform.anchoredPosition = new Vector2(0, -templateHeight * index);
            playerTransform.gameObject.SetActive(true);

            // Set player name and status
            playerTransform.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = $"Player {index + 1}";

            index++;
        }
    }
}
