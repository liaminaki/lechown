using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private GameObject joinGame;
    [SerializeField] private GameObject hostGame;

    private HostGame hostGameScript;
    //private JoinGame joinGameScript;

    private void Awake(){

        joinGame.SetActive(false);
        hostGame.SetActive(false);

        //Getting the script of each Game Object
        hostGameScript = hostGame.GetComponent<HostGame>();
        //joinGAmeScript = joinGame.GetComponent<JoinGame>();

        startHostButton.onClick.AddListener(() =>
        {
            if (hostGameScript != null)
            {
                hostGameScript.StartHostGame(); // Call StartHostGame from the script
                hostGame.SetActive(true);
                Hide();
            }
            else
            {
                Debug.LogError("HostGame script is not attached to the hostGame GameObject.");
            }
        });

        joinGameButton.onClick.AddListener(() => {
            joinGame.SetActive(true);
            Hide();
        });
    }

    private void Hide(){
        gameObject.SetActive(false);
    }
}
