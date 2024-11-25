using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
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

	// Use this for initialization
	void Start () {
		// Initial Movement Direction
		GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
		spawnWall ();
		prevKey = upKey;
	}
	
	// Update is called once per frame
	void Update () {
		// Check for key presses
		if (Input.GetKeyDown (upKey) && prevKey != downKey) {
			GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed;
			spawnWall ();
			prevKey = upKey;
		} else if (Input.GetKeyDown (downKey) && prevKey != upKey) {
			GetComponent<Rigidbody2D>().linearVelocity = -Vector2.up * speed;
			spawnWall ();
			prevKey = downKey;
		} else if (Input.GetKeyDown (rightKey) && prevKey != leftKey) {
			GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * speed;
			spawnWall ();
			prevKey = rightKey;
		} else if (Input.GetKeyDown (leftKey) && prevKey != rightKey) {
			GetComponent<Rigidbody2D>().linearVelocity = -Vector2.right * speed;
			spawnWall ();
			prevKey = leftKey;
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
		}
	}
}
