using UnityEngine;
using System.Collections;

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

	public string ToString() {
		return "Min: (" + minSpawn.x + ", " + minSpawn.y + ") Max: (" + maxSpawn.x + ", " + maxSpawn.y + ")";
	}
}

public class GameController : MonoBehaviour {

	public GameObject player;
	public GameObject pickUp;
	public GameObject platform;

	private PlayerMovement playerMovement;

	public int spawnCounter = 3;
	public float downHeightDifference;
	public float upHeightDifference;
	public float minDistance;
	public float maxDistance;
	public float deadZone;


	private Spawn platformSpawn = new Spawn ();
	private Spawn pickUpSpawn = new Spawn();
	private Vector3 originPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		playerMovement = player.GetComponent<PlayerMovement> ();
		platformSpawn.maxSpawn = new Vector2 (maxDistance, upHeightDifference);
		platformSpawn.minSpawn = new Vector2 (minDistance, downHeightDifference);

		pickUpSpawn.minSpawn = new Vector2 (-7.5f, 0.5f);
		pickUpSpawn.maxSpawn = new Vector2 (0.5f, playerMovement.jumpPower);
		Debug.Log (pickUpSpawn.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x >= originPosition.x - spawnCounter * 7.5f) {
			Vector3 randomPosition = originPosition + platformSpawn.RandomSpawnPosition () + Vector3.right * 7.5f;
			if (randomPosition.y <= deadZone + 5)
				randomPosition.y = originPosition.y + platformSpawn.RandomY ();
			Instantiate (platform, randomPosition, Quaternion.Euler (Vector3.up * 90f));

			while(Random.value > 0.5f) {
				Vector3 pickUpLocation = pickUpSpawn.RandomSpawnPosition ();
				Debug.Log (pickUpLocation);
				Instantiate (pickUp, randomPosition + pickUpLocation, Quaternion.identity);
			}

			originPosition = randomPosition;
		}

		if (player.transform.position.y < deadZone) {
			player.SendMessage ("Restart");
		}
	}
}
