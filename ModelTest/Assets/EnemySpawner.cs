using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public GameObject leader;
    public GameObject other;
    public GameObject player;
    public int others = 2;
    public float distance = 100.0f;
    public float interval = 10.0f;
    private float elapsed = 0.0f;

	// Use this for initialization
	void Start () {
	    
    }
	
	// Update is called once per frame
	void Update () {
        elapsed += Time.deltaTime;

        if (elapsed >= interval)
        {
            elapsed = 0.0f;

            spawn(player.transform.position + (player.transform.forward * distance));
        }
	}

    void spawn(Vector3 location)
    {
        GameObject newLeader = GameObject.Instantiate(leader);
        newLeader.SetActive(true);
        newLeader.transform.position = new Vector3(location.x, 20.0f, location.z);

        for (int i = 0; i < others; i++)
        {
            GameObject newOther = GameObject.Instantiate(other);
            newOther.SetActive(true);
            newOther.GetComponent<FSM>().leader = newLeader;
            newOther.transform.position = newLeader.transform.position + (Vector3)(Random.insideUnitCircle * 10.0f);
        }
    }
}
