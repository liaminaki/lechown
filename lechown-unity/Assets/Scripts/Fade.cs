using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    void Start()
    {
        // Start the fade process after 4 seconds
        StartCoroutine(DelayedFade());
    }

    IEnumerator DelayedFade()
    {
        // Wait for 4 seconds
        yield return new WaitForSeconds(4);

        // Start the fading process
        StartCoroutine(DoFade());

        // Move to the next scene
        SceneManager.LoadScene("YouAreThe");
    }

    IEnumerator DoFade(){
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha<1){
            canvasGroup.alpha += Time.deltaTime / 2;
            yield return null;
        }

        canvasGroup.interactable = false;
        yield return null;
    }
}
