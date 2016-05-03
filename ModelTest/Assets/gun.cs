using UnityEngine;
using System.Collections;

public class gun : MonoBehaviour {

    public GameObject bullet;
    public float rateOfFire = 100.0f;//milliseconds
    public float ahead = 5.0f;
    private float elapsed;

	// Use this for initialization
	void Start () {
        elapsed = rateOfFire;
	}
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;
	}

    public void Shoot()
    {
        if (elapsed >= (rateOfFire / 1000.0f))
        {
            elapsed = 0.0f;

            //GameObject newBullet = GameObject.Instantiate(bullet);
            GameObject newBullet = (GameObject)Instantiate(bullet);
            newBullet.transform.rotation = transform.rotation;
            newBullet.transform.position = transform.position + (transform.forward * ahead);
            newBullet.GetComponent<Bullet>().setDirection(GetComponent<Boid>().velocity);
            newBullet.SetActive(true);
        }
    }
}
