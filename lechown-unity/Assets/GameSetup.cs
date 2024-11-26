using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private GameObject joinGame;
    [SerializeField] private GameObject hostGame;

    private void Awake(){

        joinGame.SetActive(false);
        hostGame.SetActive(false);

        startHostButton.onClick.AddListener(() => {
            //Debug.Log("Host Starting");
            //NetworkManager.Singleton.StartHost();
            hostGame.SetActive(true);
            Hide();
        });

        joinGameButton.onClick.AddListener(() => {
            //Debug.Log("Joining Game");
            //NetworkManager.Singleton.StartClient();
            joinGame.SetActive(true);
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
