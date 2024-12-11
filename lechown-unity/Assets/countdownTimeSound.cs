using UnityEngine;

public class countdownTimeSound : MonoBehaviour
{

    public Sounds soundsMan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundsMan = FindObjectOfType<Sounds>();
    }

    public void PlayCountdown()
    {
        print("countdownshouldplay");
        soundsMan.PlayCountdown();
    }


}
