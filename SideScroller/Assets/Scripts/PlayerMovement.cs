using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	public float jumpPower;
	public float pickUpLasttime;

	private bool grounded = true;
	private bool jump = false;

	private Vector3 startPosition;
	private float startSpeed;
	private List<GameObject> collectedPickups = new List<GameObject>();

	[HideInInspector]
	public float collectionSpeed = 0f;
	[HideInInspector]
	public float jumpRange = 0f;
	[HideInInspector]
	public bool restart = false;

	private Rigidbody rb;
	private GameController gameController;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameController> ();
		startPosition = transform.position;
		startSpeed = maxSpeed;
		jumpRange = 2 * maxSpeed;
	}

	void Update() {
		if (Input.GetKey (KeyCode.UpArrow) && grounded)
			jump = true;

		collectionSpeed += Time.deltaTime;
	}

	void FixedUpdate () {
		float h = Input.GetAxis ("Horizontal");

		if (h * rb.velocity.x < maxSpeed)
			rb.AddForce (Vector3.right * h * speed);

		if (Mathf.Abs (rb.velocity.x) > maxSpeed)
			rb.velocity = new Vector3 (Mathf.Sign (rb.velocity.x) * maxSpeed, rb.velocity.y);

		if (jump) {
			rb.velocity += (Vector3.up * Mathf.Sqrt(-2f * Physics.gravity.y * jumpPower));
			jump = false;
		}
		
	}

	void OnCollisionEnter (Collision collision) {
			grounded = true;
	}

	void OnCollisionExit (Collision collision) {
		grounded = false;
	}

	void OnTriggerEnter(Collider other) {
		other.gameObject.SetActive (false);
		collectionSpeed = 0f;
		collectedPickups.Add (other.gameObject);
		StartCoroutine (IncreaseMoveSpeed());
	}

	IEnumerator IncreaseMoveSpeed() {
		maxSpeed++;
		jumpRange += 2;
		yield return new WaitForSeconds (pickUpLasttime);
		if (restart)
			yield break;
		maxSpeed--;
		jumpRange -= 2;
	}

	public void Restart() {
		restart = false;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.position = startPosition;
		foreach (GameObject pickUp in collectedPickups) {
			pickUp.SetActive (true);
		}
		collectedPickups = new List<GameObject> ();
		maxSpeed = startSpeed;

	}
}
