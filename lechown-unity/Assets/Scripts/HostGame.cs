using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class HostGame : NetworkBehaviour
{

    [SerializeField] TextMeshProUGUI iPAddress;
    [SerializeField] TextMeshProUGUI playerCount;

    public string currentPlayerCount;

    public void Start()
    {
        currentPlayerCount = GetPlayerCount().ToString();
    }

    public void Update()
    {
        playerCount.text = currentPlayerCount + "/2 PLAYERS";
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
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (unityTransport != null)
        {
            unityTransport.ConnectionData.Address = localIPAddress;
            unityTransport.ConnectionData.Port = 7777;
            Debug.Log($"Host IP set to: {localIPAddress}");
        }

        // Start the host
        if (NetworkManager.Singleton.StartHost())
        {
            iPAddress.text = localIPAddress;
            Debug.Log("Host started successfully!");
        }
        else
        {
            Debug.LogError("Failed to start host!");
        }

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
}
