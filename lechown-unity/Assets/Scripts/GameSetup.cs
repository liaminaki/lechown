using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            hostGame.SetActive(true);
            Hide();
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
