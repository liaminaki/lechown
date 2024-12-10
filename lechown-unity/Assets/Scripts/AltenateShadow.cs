using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AlternateShadow : MonoBehaviour
{
    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // Sprites to alternate between
    public Sprite lechonSprite;
    public Sprite humanSprite;

    // Timer and interval for switching sprites
    private float switchInterval = 0.5f;
    private float timer;

    // Current state (true for lechon, false for human)
    private bool isLechon = true;

    // Scale values for lechon and human
    private Vector3 lechonScale = Vector3.one;
    private Vector3 humanScale = new Vector3(0.1973f, 0.1973f, 0.1973f);

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the initial sprite and scale
        if (spriteRenderer != null && lechonSprite != null)
        {
            spriteRenderer.sprite = lechonSprite;
            transform.localScale = lechonScale;
        }

        // Initialize the timer
        timer = switchInterval;

    }

    void Update()
    {
        // Update the timer
        timer -= Time.deltaTime;

        // Check if it's time to switch sprites and scale
        if (timer <= 0)
        {
            // Reset the timer
            timer = switchInterval;

            // Toggle the sprite and scale
            isLechon = !isLechon;

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = isLechon ? lechonSprite : humanSprite;
                transform.localScale = isLechon ? lechonScale : humanScale;
            }
        }
    }
}
