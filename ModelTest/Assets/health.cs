using UnityEngine;
using System.Collections;

public class health : MonoBehaviour {

    public float total = 2.0f;//total health
    public float damage = 1.0f;//how much damage is taken from a bullet
    public float current;

	// Use this for initialization
	void Start () {
        current = total;
	}
	
	// Update is called once per frame
	void Update () {
        if (current <= 0.0f)
        {
            //set state machine to dead or something
            //Debug.Log("Dead!");
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "bullet")
        {
            current -= damage;
            Debug.Log("Hit " + gameObject.name + "! Current health: " + current);
        }
    }
}
