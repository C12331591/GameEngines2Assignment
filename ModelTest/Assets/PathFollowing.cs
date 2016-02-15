using UnityEngine;
using System.Collections;

public class PathFollowing : MonoBehaviour {

    private Vector3[] waypoints;
    public int numWaypoints = 5;
    public float area = 50.0f;
    private int currentWaypoint = 0;

	// Use this for initialization
	void Start () {
        waypoints = new Vector3[numWaypoints];

        for(int i = 0; i < numWaypoints; i++)
        {
            Vector3 newPoint = new Vector3();
            float theta = ((Mathf.PI * 2) / numWaypoints) * i;
            newPoint.x = Mathf.Sin(theta) * Random.Range(0.5f, area);
            newPoint.z = Mathf.Cos(theta) * Random.Range(0.5f, area);
            newPoint.y = 5.0f;

            waypoints[i] = newPoint;
        }

        this.GetComponent<Boid>().seekEnabled = true;
        this.GetComponent<Boid>().seekTargetPosition = waypoints[0];
    }
	
	// Update is called once per frame
	void Update () {
	    if (Vector3.Distance(transform.position, waypoints[currentWaypoint]) < 2.0f)
        {
            currentWaypoint = ++currentWaypoint % numWaypoints;

            this.GetComponent<Boid>().seekTargetPosition = waypoints[currentWaypoint];
        }
	}
}
