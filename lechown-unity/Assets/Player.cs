using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// Lives
	private const int MAX_LIVES = 3;
	private int lives = MAX_LIVES;
	[SerializeField] private GameObject[] livesUI;
	[SerializeField] private Sprite lifeUI;
	[SerializeField] private Sprite noLifeUI;

	// Movement keys (customizable in inspector)
	public KeyCode upKey;
	public KeyCode downKey;
	public KeyCode rightKey;
	public KeyCode leftKey;

	// Movement Speed
	public float speed = 16;

	// Wall Prefab
	public GameObject wallPrefab;

	// Current Wall
	Collider2D wall;

	// Last Wall's End
	Vector2 lastWallEnd;

	// Previous input
	private KeyCode prevKey;

	// Animator
	private Animator animator;

	// Compute movement based on key states
	float moveX = 0f;
	float moveY = 0f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		updateLivesUI();
		// Initial Movement Direction
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
		spawnWall ();
		prevKey = upKey;
	}
	
	// Update is called once per frame
	void Update () {
		moveX = 0f;
		moveY = 0f;

		// Check for key presses
		if (Input.GetKeyDown (upKey) && prevKey != downKey) {
			GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
			spawnWall ();
			moveY = 1f;
			prevKey = upKey;
		} else if (Input.GetKeyDown (downKey) && prevKey != upKey) {
			GetComponent<Rigidbody2D>().linearVelocity = -Vector2.up * speed;
			spawnWall ();
			moveY = -1f;
			prevKey = downKey;
		} else if (Input.GetKeyDown (rightKey) && prevKey != leftKey) {
			GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * speed;
			spawnWall ();
			moveX = 1f;
			prevKey = rightKey;
		} else if (Input.GetKeyDown (leftKey) && prevKey != rightKey) {
			GetComponent<Rigidbody2D>().linearVelocity = -Vector2.right * speed;
			spawnWall ();
			moveX = -1f;
			prevKey = leftKey;
		}


		// Vector2 movement = new Vector2(moveX, moveY).normalized;
		print(moveX);
		print(moveY);

		if (moveX != 0 || moveY != 0) {
			animator.SetFloat("X", moveX);
			animator.SetFloat("Y", moveY);
		}
		
		fitColliderBetween (wall, lastWallEnd, transform.position);
	}

	void spawnWall() {
		// Save last wall's position
		lastWallEnd = transform.position;

		// Spawn a new Lightwall
		GameObject g = (GameObject)Instantiate (wallPrefab, transform.position, Quaternion.identity);
		wall = g.GetComponent<Collider2D>();
	}

	void fitColliderBetween(Collider2D co, Vector2 a, Vector2 b) {
		// Calculate the Center Position
		co.transform.position = a + (b - a) * 0.5f;
		int wallScale = 2;

		// Scale it (horizontally or vertically)
		float dist = Vector2.Distance (a, b);
		if (a.x != b.x)
			co.transform.localScale = new Vector2 (dist + wallScale, wallScale);
		else
			co.transform.localScale = new Vector2 (wallScale, dist + wallScale);
	}

	void OnTriggerEnter2D(Collider2D co) {
		if (co != wall) {
			print ("Player lost:" + name);
			Destroy (gameObject);

			// reduceLife();
		}
	}


    void updateLivesUI() {
        // Ensure livesUI array is valid
        if (livesUI == null || livesUI.Length != MAX_LIVES) {
            Debug.LogError("Lives UI array is not properly set up.");
            return;
        }

        // Update UI based on the current lives
        for (int i = 0; i < MAX_LIVES; i++) {
            SpriteRenderer renderer = livesUI[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = i < lives ? lifeUI : noLifeUI;
            }
            else
            {
                Debug.LogError($"Missing SpriteRenderer on livesUI[{i}] GameObject.");
            }
        }
    }

    public void reduceLife() {
        // Reduce life and update the UI
        if (lives > 0) {
            lives--;
            updateLivesUI();
        }
        
		else {
            Debug.Log("Player is already out of lives!");
        }
    }

}
