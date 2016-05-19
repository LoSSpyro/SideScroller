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
	private List<GameObject> collectedPickups = new List<GameObject>();

	[HideInInspector]
	public float collectionSpeed = 0f;
	[HideInInspector]
	public float jumpRange = 0f;

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		startPosition = transform.position;

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
		jumpRange = transform.position.x - jumpRange;
		Debug.Log ("Jump Range = " + jumpRange);
	}

	void OnCollisionExit (Collision collision) {
		grounded = false;
		jumpRange = transform.position.x;
	}

	void OnTriggerEnter(Collider other) {
		other.gameObject.SetActive (false);
		collectionSpeed = 0f;
		collectedPickups.Add (other.gameObject);
		StartCoroutine (IncreaseMoveSpeed());
	}

	IEnumerator IncreaseMoveSpeed() {
		maxSpeed++;
		yield return new WaitForSeconds (pickUpLasttime);
		maxSpeed--;
	}

	public void Restart() {
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.position = new Vector3 (-5f, 0.5f, 0f);
		foreach (GameObject pickUp in collectedPickups) {
			pickUp.SetActive (true);
		}

	}
}
