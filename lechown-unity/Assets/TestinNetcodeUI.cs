using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TestinNetcodeUI : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button joinGameButton;

    private void Awake(){
        startHostButton.onClick.AddListener(() => {
            Debug.Log("Host Starting");
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        joinGameButton.onClick.AddListener(() => {
            Debug.Log("Joining Game");
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
