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
    }

    IEnumerator DoFade(){
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while (canvasGroup.alpha<1){
            canvasGroup.alpha += Time.deltaTime / 3;
            yield return null;
        }

        canvasGroup.interactable = false;
        
        // Move to the next scene
        SceneManager.LoadScene("YouAreThe");
        yield return null;
    }
}
