using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class GameSetup : NetworkBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private TextMeshProUGUI playersCountText;
    /*    [SerializeField] private GameObject joinGame;
        [SerializeField] private GameObject hostGame;*/

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);

    private void Awake(){

        /*joinGame.SetActive(false);
        hostGame.SetActive(false);*/

        startHostButton.onClick.AddListener(() => {
            //Debug.Log("Host Starting");
            NetworkManager.Singleton.StartHost();
            /*hostGame.SetActive(true);*/
            Hide();
        });

        joinGameButton.onClick.AddListener(() => {
            //Debug.Log("Joining Game");
            NetworkManager.Singleton.StartClient();
            /*joinGame.SetActive(true);*/
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void Update()
    {
        playersCountText.text = playersNum.Value.ToString() + "/2 PLAYERS";
        if (!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
        
    }
}
