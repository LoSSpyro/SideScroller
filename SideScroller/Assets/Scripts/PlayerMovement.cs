using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour {

	// Setzbare Variablen fuer Beschleunigung, Geschwindigkeit, Sprungkraft und Dauer der PickUps
	public float acceleration;
	public float Speed;
	public float jumpPower;
	public float pickUpDuration;

	// Der Input von der Tastatur (-1 = links, +1 = rechts)
	private float h;

	// Boolean ob Charakter springt, und Bodenkontakt hat
	private bool grounded;
	private bool jump = false;

	// Die StartPosition, der StartSpeed und die PickUps, die eingesammelt worden sind
	private Vector3 startPosition;
	private float startSpeed;
	private List<GameObject> collectedPickups = new List<GameObject>();

	// Anzahl von PickUps die in einem bestimmten Intervall eingesammelt worden sind
	[HideInInspector]
	public int pickUpsCollected { get; set; }
	[HideInInspector]
	public float jumpRange = 0f;

	// Rigidbody fuer Physics und Animator fuer Animationen
	private Rigidbody rb;


	void Start () {
		// Hol die Komponenten
		rb = GetComponent<Rigidbody> ();
		// Setze Startposition auf momentane Position
		startPosition = transform.position;

		// Spieler hat Bodenkontakt
		grounded = true;
		// Die Geschwindigkeit am Anfang
		startSpeed = Speed;
		// Die Sprungreichweite (ca., Genaue Formel unbekannt)
		jumpRange = 2 * Speed;

		// Fuehe staendig eine Idle Animation aus
		InvokeRepeating("AnimateIdle", 8f, 15f);
	}

	void Update() {
		// Hol den Input vom pieler
		h  = Input.GetAxis ("Horizontal");

		// Falls Spieler Bodenkontakt hat und nach oben drueckt
		if (Input.GetKey (KeyCode.UpArrow) && grounded)
			jump = true;
	}

	void FixedUpdate () {

		// Falls die Geschwindigkeit unter der maximal erreichbaren Geschwindigkeit liegt
		if (h * rb.velocity.x < Speed)
			// Fuege dem Spieler eine Beschleunigung hinzu
			rb.AddForce (Vector3.right * h * acceleration);

		// Falls die Geschwindikeit ein Schwellenwert erreicht
		if (Mathf.Abs (rb.velocity.x) > Speed)
			// Setze den Spieler auf die maximale Geschwindigkeit
			rb.velocity = new Vector3 (Mathf.Sign (rb.velocity.x) * Speed, rb.velocity.y);

		// Falls der Spieler springt
		if (jump) {
			// Fuege eine vertikale Kraft hinzu
			rb.velocity += (Vector3.up * Mathf.Sqrt(-2f * Physics.gravity.y * jumpPower));
			jump = false;
		}
		
	}

	// Wenn der Spieler in einen Trigger eintritt
	void OnTriggerEnter(Collider other) {
		// Wenn das Objekt ein PickUp ist
		if (other.gameObject.CompareTag ("PickUp")) {
			// Deaktiviere das PickUp
			other.gameObject.SetActive (false);
			// erhoehe die Anzahl an eingesammelten PickUps
			pickUpsCollected++;
			// Und fuege das PickUp der Liste hinzu
			collectedPickups.Add (other.gameObject);
			// Und erhoehe die Geschwindigkeit fuer eine gewisse Zeit
			StartCoroutine (IncreaseMoveSpeed ());
		}

		// Wenn das andere Objekt eine Platform ist
		if (other.gameObject.CompareTag ("Platform")) {
			// Hat der Spieler Bodenkontakt
			grounded = true;
		}
	}

	// Wenn der Spieler einen Trigger verlaesst
	void OnTriggerExit(Collider other) {
		// Hat er keinen Bodenkontakt mehr
		if (other.gameObject.CompareTag ("Platform")) {
			grounded = false;
		}

	}

	// Erhoehe Geschwindigkeit fuer eine gewisse Zeit
	IEnumerator IncreaseMoveSpeed() {
		// erhoehe Geschwindigkeit
		Speed++;
		// erhoehe Sprungreichweite
		jumpRange += 2;
		// warte pickUpDuration lang
		yield return new WaitForSeconds (pickUpDuration);
		// verringere Speed und jumpRange wieder
		Speed--;
		jumpRange -= 2;
	}

	// Wenn der Spieler in die DeadZone eintritt
	public void Restart() {
		// Setze Winkel-, Geschwindigkeit und Position zurueck
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.position = startPosition;
		// Aktiviere die eingesammelten PickUps wieder
		foreach (GameObject pickUp in collectedPickups) {
			pickUp.SetActive (true);
		}
		// und loesche die Liste
		collectedPickups = new List<GameObject> ();
		// Setze die eingesammelten PickUps zurueck
		pickUpsCollected = 0;
		// Geschwindigkeit = StartSpeed, Bodenkontakt ist da
		Speed = startSpeed;
		grounded = true;
		// und stoppe alle momentan laufenden PickUps
		StopAllCoroutines();
	}
}
