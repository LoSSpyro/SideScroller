using UnityEngine;
using System.Collections;


// Klasse um einen Spawnbereich zu definieren
public class Spawn {
	public Vector2 minSpawn { get; set; }
	public Vector2 maxSpawn { get; set; }

	public Spawn(float minX, float maxX, float minY, float maxY) {
		minSpawn.Set (minX, minY);
		maxSpawn.Set (maxX, maxY);
	}

	public Spawn() {
		minSpawn.Set (0f, 0f);
		maxSpawn.Set (0f, 0f);
	}

	public float RandomX () {
		return Random.Range(minSpawn.x, maxSpawn.x);
	}

	public float RandomY () {
		return Random.Range (minSpawn.y, maxSpawn.y);
	}

	public Vector3 RandomSpawnPosition () {
		return new Vector3 (RandomX (), RandomY());
	}

	override public string ToString() {
		return "Min: (" + minSpawn.x + ", " + minSpawn.y + ") Max: (" + maxSpawn.x + ", " + maxSpawn.y + ")";
	}
}

public class GameController : MonoBehaviour {

	// Referenzen zu dem Spieler, den PickUps und den Platformen
	public GameObject player;
	public GameObject pickUp;
	public GameObject platform;

	// Referenz zum PlayerMovement-Script
	private PlayerMovement playerMovement;

	// Variablen fuer, Anzahl der vorzuspawnend Platformen, Hoehendifferenz und Abgrundbreite sowie Deadzoe
	public int spawnCounter;
	public float downHeightDifference;
	public float upHeightDifference;
	public float minDistance;
	public float maxDistance;
	public float deadZone;

	// Variable um zu zaehlen wie viele PickUps gespawnt sind
	private int pickUpsSpawned = 0;


	// Spawnbereiche und Orientierungspunkt
	private Spawn platformSpawn = new Spawn ();
	private Spawn pickUpSpawn = new Spawn();
	private Vector3 originPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		// Hole die Komponenten
		playerMovement = player.GetComponent<PlayerMovement> ();

		// Initialisere die Spawnbereiche
		platformSpawn.maxSpawn = new Vector2 (maxDistance, upHeightDifference);
		platformSpawn.minSpawn = new Vector2 (minDistance, downHeightDifference);
		// Berechne die Maximale Distanz
		maxDistance = playerMovement.Speed * 2;

		pickUpSpawn.minSpawn = new Vector2 (-7.5f, 0.5f);
		pickUpSpawn.maxSpawn = new Vector2 (0.5f, playerMovement.jumpPower);
		// Starte einen Timer der den Spawnbereich in gewissen Abstaenden aktualisiert, je nachdem wie viele PickUps eingesammelt werden
		StartCoroutine (Timer ());
	}
	
	// Update is called once per frame
	void Update () {
		// Falls der Spieler sich nah genug an der Spawnposition der naechsten Platformen befindet
		if (player.transform.position.x >= originPosition.x - spawnCounter * 7.5f) {
			// Berechne eine neue zufaellige Spawnposition
			Vector3 randomPosition = originPosition + platformSpawn.RandomSpawnPosition () + Vector3.right * 7.5f;

			// Stelle sicher, dass diese sich nicht in der DeadZone befindet
			if (randomPosition.y <= deadZone + 5)
				randomPosition.y = originPosition.y + platformSpawn.RandomY ();

			// Instantiiere dort eine Platform
			Instantiate (platform, randomPosition, Quaternion.Euler (Vector3.up * 90f));

			// Spawne mit einer 50%igen Wahrscheinlichkeit ein weiters PickUp auf der Platform
			while(Random.value > 0.5f) {
				Vector3 pickUpLocation = pickUpSpawn.RandomSpawnPosition ();
				Instantiate (pickUp, randomPosition + pickUpLocation, Quaternion.identity);
				pickUpsSpawned++;
			}

			// Setze den Orientierungspunkt auf die neue Spawnposition
			originPosition = randomPosition;
		}

		// Falls der Spieler sich in der DeadZone befindet, setze ihn zurueck
		if (player.transform.position.y < deadZone) {
			player.SendMessage ("Restart");
		}
	}

	// Aktualisiere den Spawnbereich von Platformen nach einer gewissen Zeit
	IEnumerator Timer () {
		while (true) {
			// Warte die AnhalteDauer von PickUps ab
			yield return new WaitForSeconds (playerMovement.pickUpDuration);

			// Berechne ein Skalar, dass abhaengt von Anzahl der eingesammelten PickUps und gespawnten PickUps
			float scale = 0f;
			if (pickUpsSpawned > 0 && playerMovement.pickUpsCollected > 0) {
				scale = ((float)playerMovement.pickUpsCollected / (float)pickUpsSpawned);
			} else
				scale = .1f;
			// Aktualisere den Spawn
			platformSpawn.minSpawn = new Vector2 (minDistance * scale, downHeightDifference * (1.1f-scale));
			platformSpawn.maxSpawn = new Vector2 (maxDistance * scale, upHeightDifference * (1.1f-scale));
			// Und setze die Werte zurueck
			playerMovement.pickUpsCollected = 0;
			pickUpsSpawned = 0;
		}
	}

}
