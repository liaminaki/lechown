using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameSetup : NetworkBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private TextMeshProGUI playersCountText;
    /*    [SerializeField] private GameObject joinGame;
        [SerializeField] private GameObject hostGame;*/

    private NetworkVariable<int> playersNum = new NetworkVariable<int>();

    private void Awake(){

        joinGame.SetActive(false);
        hostGame.SetActive(false);

        startHostButton.onClick.AddListener(() => {
            //Debug.Log("Host Starting");
            //NetworkManager.Singleton.StartHost();
            /*hostGame.SetActive(true);*/
            Hide();
        });

        joinGameButton.onClick.AddListener(() => {
            //Debug.Log("Joining Game");
            //NetworkManager.Singleton.StartClient();
            /*joinGame.SetActive(true);*/
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!IsServer) return;
        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
        playersCountText.text = playersNum.Value.ToString();
    }
}
