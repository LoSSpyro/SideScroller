using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {


	void Update ()
	{
		// Rotiere die PowerUps
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
	}
}
