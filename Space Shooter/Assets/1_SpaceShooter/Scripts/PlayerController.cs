using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public GameObject shot;
    public Transform shotSpawn;
    private Quaternion calibrate;

    public Boundary boundary;
    public GameController gameController;
    public static Vector3 playerPosition;

    public Joystick joystick;
    public Joybutton joybutton;

    public float speed, tilt, fireRate, nextFire;


    void Start()
    {
        // Debut de la calibration de l'accelerometre
        CalibrateAccelerometer();

        joystick = FindObjectOfType<Joystick>();
        joybutton = FindObjectOfType<Joybutton>();

        GameObject gameControllerObject = GameObject.FindGameObjectWithTag("GameController");
        if (gameControllerObject != null)
            gameController = gameControllerObject.GetComponent<GameController>();
    }

    //CALIBRATE ACCELEROMETER
    void CalibrateAccelerometer()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrate = Quaternion.Inverse(rotateQuaternion);
    }

    //FIX ACCELEROMETER
    Vector3 FixAcceleration(Vector3 acceleration)
    {
        Vector3 fixedAcceleration = calibrate * acceleration;
        return fixedAcceleration;
    }

    // FIXED UPDATE - Deplacement du vaisseau selon le mode choidi.
    void FixedUpdate ()
	{
        Vector3 movement;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float accelValue = OptionsController.accelValue;

        if (!OptionsController.isAccelSelected)  // En mode tactile 
        {
            moveHorizontal += joystick.Horizontal;
            moveVertical += joystick.Vertical;
            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

            // Gestion du tir des deux armes
            if ((joybutton.Pressed || Input.GetKey(KeyCode.Space)) && Time.time > nextFire) Fire();           
            if (Input.GetButton("Fire2") && gameController.turboEnabled) secondWeapon();
        }
        else // En mode accelerometre
        {
            Vector3 accelerationRaw = Input.acceleration;
            Vector3 acceleration = FixAcceleration(accelerationRaw) * accelValue;
            movement = new Vector3(acceleration.x, 0.0f, acceleration.y);

            // Gestion du tir des deux armes
            if (Input.GetButton("Fire1") && Time.time > nextFire) Fire();
            if (Input.GetButton("Fire1") && gameController.turboEnabled) secondWeapon();
        }

        GetComponent<Rigidbody>().velocity = movement * speed;
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
            0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
        playerPosition = new Vector3(GetComponent<Rigidbody>().position.x, 0.0f, GetComponent<Rigidbody>().position.z);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
    }

    // FIRE - parametres du tir
    void Fire()
    {
        nextFire = Time.time + fireRate;
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        GetComponent<AudioSource>().Play();
    }

    // SECOND WEAPON - Gestion de tir de la deuxieme arme
    public void secondWeapon()
    {
        nextFire = Time.time + fireRate;
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        GetComponent<AudioSource>().Play();
        gameController.decreaseEnergyBar();
    }
}
