using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	// Player Sprite
	public Sprite sprite;
	
	// Lives
	private const int MAX_LIVES = 3;
	public int lives = MAX_LIVES;
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

	// Track the last non-zero movement direction for dead state
	private float lastMoveX = 0f;
	private float lastMoveY = 0f;

	// Track if allowed to move
	private bool canMove = false;
	private bool isDead = false;

	// Use this for initialization
	void Start () {
		getSprite();
		animator = GetComponent<Animator>();
		updateLivesUI();
		stopMovement();
	}
	
	// Update is called once per frame
	void Update () {

		if (!canMove) return;

		moveX = 0f;
		moveY = 0f;

		// Check for key presses
		if (Input.GetKeyDown (upKey) && prevKey != downKey)
			moveUp();

		else if (Input.GetKeyDown (downKey) && prevKey != upKey)
			moveDown();

		else if (Input.GetKeyDown (rightKey) && prevKey != leftKey)
			moveRight();

		else if (Input.GetKeyDown (leftKey) && prevKey != rightKey)
			moveLeft();
		
		// Only update animator if moving
		if (moveX != 0 || moveY != 0) {
			updateAnim(moveX, moveY);
		}

		fitColliderBetween (wall, lastWallEnd, transform.position);
	}

	public void startMovement() {
		canMove = true;
		moveUp();
	}

	public void resetState() {
		// Initial Movement Direction
		moveX = 0f;
		moveY = 0f;
		lastMoveX = 0f;
		lastMoveY = 0f;
		animator.SetBool("IsDead", false);
		isDead = false;

		// Update play to look up without moving
		updateAnim(moveX, 1f);
	}

	private void getSprite() {
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		sprite = spriteRenderer.sprite;	
	}

	void updateAnim(float _moveX, float _moveY) {

		animator.SetFloat("X", _moveX);
		animator.SetFloat("Y", _moveY);
		
	}

	void spawnWall() {
		// Save last wall's position
		lastWallEnd = transform.position;

		// Spawn a new Lightwall
		GameObject g = (GameObject)Instantiate (wallPrefab, transform.position, Quaternion.identity);
		wall = g.GetComponent<Collider2D>();

		g.tag = "Wall";
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
		if (co != wall && !isDead) { // && canMove // canMove = !isDead, this make sure it wont trigger again when alr dead
			OnPlayerDead();
			GameManager.Instance.handleCollision();
		}
	}

	void OnPlayerDead() {
		animator.SetBool("IsDead", true);
		isDead = true;

		updateAnim(lastMoveX, lastMoveY);
		reduceLife();

		print("Dead");
	}

	// Updates the game state (call this when a player loses all lives)
    // public void OnPlayerOutOfLives()
    // {	
	// 	if (lives == 0)
	// 		GameManager.Instance.endGame();
    // }

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

	void moveUp() {
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
		spawnWall ();
		prevKey = upKey;

		moveY = 1f;

		lastMoveX = 0f;
		lastMoveY = moveY;
	}

	void moveDown() {
		GetComponent<Rigidbody2D>().linearVelocity = -Vector2.up * speed;
		spawnWall ();
		prevKey = downKey;

		moveY = -1f;

		lastMoveX = 0f;
		lastMoveY = moveY;
	}

	void moveLeft() {
		GetComponent<Rigidbody2D>().linearVelocity = -Vector2.right * speed;
		spawnWall ();
		prevKey = leftKey;

		moveX = -1f;

		lastMoveX = moveX;
		lastMoveY = 0f;
	}

	void moveRight() {
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * speed;
		spawnWall ();
		prevKey = rightKey;

		moveX = 1f;
		
		lastMoveX = moveX;
		lastMoveY = 0f;
	}

	public void stopMovement() {
		canMove = false;
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

		// Reset movement inputs to ensure no unintended movement
		moveX = 0f;
		moveY = 0f;
		lastMoveX = 0f;
		lastMoveY = 0f;
	}

}
