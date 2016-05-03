using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float speed = 100.0f;
    public float decay = 2.0f;
    private Vector3 velocity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //transform.Translate(transform.forward * (speed * Time.deltaTime));//constant speed
        transform.position += velocity * Time.deltaTime;

        decay -= Time.deltaTime;

        if (decay <= 0.0f)
        {
            Destroy(gameObject);
        }
	}

    void OnCollisionEnter(Collision col)
    {
        //destroy the bullet here, but the other part will need to do stuff too.
        Destroy(this.gameObject);
    }

    public void setDirection(Vector3 v)
    {
        velocity = v.normalized * speed;
    }
}
