using UnityEngine;
using System.Collections;

public class patrolAround : MonoBehaviour {

    public GameObject leader;
    public float waypointRadius = 50.0f;
    public float outerRadius = 75.0f;
    public float reachRadius = 20.0f;//don't make this too tight - you don't want it trying to go inside a building

    private Vector3 waypoint;

	// Use this for initialization
	void Start () {
        newWaypoint();
        GetComponent<Boid>().seekEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(transform.position, leader.transform.position) > outerRadius)
        {
            waypoint = leader.transform.position;
            GetComponent<Boid>().seekTargetPosition = waypoint;
        }

        if (Vector3.Distance(transform.position, waypoint) <= reachRadius)
        {
            newWaypoint();
        }
	}

    void newWaypoint()
    {
        //waypoint = leader.transform.position + (Random.insideUnitSphere * waypointRadius);
        waypoint = leader.transform.position + (leader.transform.forward * waypointRadius) + (leader.transform.right * (Random.Range(-1.0f, 1.0f) * waypointRadius));
        waypoint.y = leader.transform.position.y;
        GetComponent<Boid>().seekTargetPosition = waypoint;
    }
}
