using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject player;

	// Der Offset vom Spieler zur Kamera
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - player.transform.position;

	}

	// Update is called once per frame
	void LateUpdate () {
		// Setze die Kamera an diese bestimmte Position
		transform.position = player.transform.position + offset + Vector3.up * 2;
	}
}
