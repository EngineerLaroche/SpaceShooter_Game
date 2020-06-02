using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	public float speed;
	public static float difficultySpeedMultiplier = 1.0f;

	void Start ()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * speed * difficultySpeedMultiplier;
	}
}
