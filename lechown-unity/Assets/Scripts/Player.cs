using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Player : NetworkBehaviour {
	// Player Sprite
	public Sprite sprite;
	
	// Lives
	private const int MAX_LIVES = 3;
	public int lives = MAX_LIVES;
	private GameObject[] livesUI;
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

	// Vector2 lastWallEnd;

	// Last Wall's End
    private NetworkVariable<Vector2> lastWallEnd = new NetworkVariable<Vector2>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Wall Network ID
    private NetworkVariable<ulong> wallNetworkId = new NetworkVariable<ulong>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

		if (gameObject.name == "man(Clone)"){
			//find the "man lives Game Object
			GameObject manLives = GameObject.Find("man lives");

			if (manLives != null){
				//Get all children of man lives
				int childCount = manLives.transform.childCount;
				livesUI = new GameObject[childCount];

				for (int i = 0; i < childCount; i++){
					livesUI[i] = manLives.transform.GetChild(i).gameObject;
				}

				Debug.Log("Man Lives initialized");
			}
			else{
				Debug.Log("Man Lives is null");
			}
		}
		else if (gameObject.name == "pig(Clone)"){
			//find the "man lives Game Object
			GameObject pigLives = GameObject.Find("pig lives");

			if (pigLives != null){
				//Get all children of man lives
				int childCount = pigLives.transform.childCount;
				livesUI = new GameObject[childCount];

				for (int i = 0; i < childCount; i++){
					livesUI[i] = pigLives.transform.GetChild(i).gameObject;
				}

				Debug.Log("Pig Lives initialized");
			}
			else{
				Debug.Log("Pig Lives is null");
			}
		}

		getSprite();
		animator = GetComponent<Animator>();
		updateLivesUI();
		stopMovement();

		wallNetworkId.OnValueChanged += OnWallNetworkIdChanged;
	}
	
	// Update is called once per frame
	void Update () {

		if (!canMove) return;
		if (!IsOwner) return;

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

		// Ensure the collider is updated on every frame, both server and client
		if (wall != null) {
			// Adjust the collider's fit every frame using lastWallEnd and current position
			FitColliderServerRpc(lastWallEnd.Value, transform.position);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void StartMovementServerRpc() {
		startMovement();
		StartMovementClientRpc();
	}

	[ClientRpc]
	void StartMovementClientRpc() {
		if (IsServer) return;
		startMovement();
	}

	public void startMovement() {
		canMove = true;
		moveUp();
	}

	[ServerRpc(RequireOwnership = false)]
	public void ResetStateServerRpc(Vector2 startPos) {
		resetState(startPos);
		ResetStateClientRpc(startPos);
	}

	[ClientRpc]
	void ResetStateClientRpc(Vector2 startPos) {
		if (IsServer) return;
		resetState(startPos);
	}

	public void resetState(Vector2 startPos) {
		// Star pos
		transform.position = startPos;

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

	[ServerRpc(RequireOwnership = false)]
    private void SpawnWallServerRpc(Vector2 position, ServerRpcParams rpcParams = default) {

		// Only spawn a wall if we're on the server
        if (!IsServer) return;

        GameObject wallInstance = Instantiate(wallPrefab, position, Quaternion.identity);
        NetworkObject wallNetworkObject = wallInstance.GetComponent<NetworkObject>();
		wallInstance.tag = "Wall";
        wallNetworkObject.Spawn();

		// Get the collider of the wall (to fit between positions)
    	// wall = wallInstance.GetComponent<Collider2D>();

        // Update wall and lastWallEnd
        wallNetworkId.Value = wallNetworkObject.NetworkObjectId;
        lastWallEnd.Value = position;

		

	}

	[ServerRpc(RequireOwnership = false)]
	void FitColliderServerRpc(Vector2 lastWallEndPosition, Vector3 playerPosition) {
		if (!IsServer) return;

		// Update the collider based on the last wall position and player position
		if (wall != null) {
			fitColliderBetween(wall, lastWallEndPosition, playerPosition);
		}

	}

	void OnWallNetworkIdChanged(ulong previousValue, ulong newValue) {
		if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(newValue, out NetworkObject wallObject)) {
			wall = wallObject.GetComponent<Collider2D>();
		}
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

		// Ensure this only runs on the server or host
        if (!IsServer) return;

		if (co != wall && !isDead) { // && canMove // canMove = !isDead, this make sure it wont trigger again when alr dead
			// OnPlayerDead();
			OnPlayerDeadServerRpc();
			GameManager.Instance.handleCollision();
		}
	}

	// ServerRpc that handles the death logic
	[ServerRpc(RequireOwnership = false)]
	void OnPlayerDeadServerRpc()
	{
		// Call the death handling function on the server
		OnPlayerDead();
		
		// Optionally, use a ClientRpc to update other clients (for example, to show a death animation)
		OnPlayerDeadClientRpc();
	}

	[ClientRpc]
	void OnPlayerDeadClientRpc()
	{
		// Only execute on clients, not on the server
		if (IsServer) return;

		OnPlayerDead();
	}

	void OnPlayerDead() {
		animator.SetBool("IsDead", true);
		isDead = true;

		stopMovement();

		updateAnim(lastMoveX, lastMoveY);
		reduceLife();

		print("Dead - Server Side");
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
		if (!IsOwner) return; 
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
		SpawnWallServerRpc(transform.position);
		
		prevKey = upKey;

		moveY = 1f;

		lastMoveX = 0f;
		lastMoveY = moveY;
	}

	void moveDown() {
		if (!IsOwner) return; 
		GetComponent<Rigidbody2D>().linearVelocity = -Vector2.up * speed;
		SpawnWallServerRpc(transform.position);
		
		prevKey = downKey;

		moveY = -1f;

		lastMoveX = 0f;
		lastMoveY = moveY;
	}

	void moveLeft() {
		if (!IsOwner) return; 
		GetComponent<Rigidbody2D>().linearVelocity = -Vector2.right * speed;
		SpawnWallServerRpc(transform.position);
		
		prevKey = leftKey;

		moveX = -1f;

		lastMoveX = moveX;
		lastMoveY = 0f;
	}

	void moveRight() {
		if (!IsOwner) return; 
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * speed;
		SpawnWallServerRpc(transform.position);
		
		prevKey = rightKey;

		moveX = 1f;
		
		lastMoveX = moveX;
		lastMoveY = 0f;
	}

	// ServerRpc that handles stop movement
	[ServerRpc(RequireOwnership = false)]
	public void StopMovementServerRpc()
	{
		stopMovement();
		StopMovementClientRpc();
	}

	[ClientRpc]
	void StopMovementClientRpc() {
		if (IsServer) return;
		stopMovement();
	}
	
	void stopMovement() {
		canMove = false;
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

		// Reset movement inputs to ensure no unintended movement
		moveX = 0f;
		moveY = 0f;
		lastMoveX = 0f;
		lastMoveY = 0f;
	}

}
