using UnityEngine;
using System.Collections;

public class patrolAroundState : State {

    public GameObject leader;
    public float waypointRadius = 50.0f;
    public float outerRadius = 75.0f;
    public float reachRadius = 20.0f;//don't make this too tight - you don't want it trying to go inside a building

    private Vector3 waypoint;

    public patrolAroundState(FSM machine, GameObject leader):base(machine)
    {
        this.leader = leader;
    }

    public override string Description()
    {
        return "Patrol Around State";
    }

    public override void Enter()
    {
        owner.GetComponent<Boid>().TurnOffAll();
        newWaypoint();
        owner.GetComponent<Boid>().seekEnabled = true;
        leader = owner.GetComponent<FSM>().leader;
        owner.GetComponent<Boid>().channelFindingEnabled = true;
    }

    public override void Exit()
    {
        
    }

    // Use this for initialization
    /*void Start () {
	
	}*/

    // Update is called once per frame
    public override void Update () {
        if (Vector3.Distance(owner.transform.position, leader.transform.position) > outerRadius)
        {
            waypoint = leader.transform.position;
            owner.GetComponent<Boid>().seekTargetPosition = waypoint;
        }

        if (Vector3.Distance(owner.transform.position, waypoint) <= reachRadius)
        {
            newWaypoint();
        }

        foreach(Collider col in Physics.OverlapSphere(owner.transform.position, outerRadius))
        {
            if (col.tag == owner.GetComponent<FSM>().enemy && col.gameObject.GetComponent<health>().current > 0)
            {
                Vector3 diff = col.gameObject.transform.position - owner.transform.position;
                float dot = Vector3.Dot(diff, owner.transform.forward);

                if (dot > 0.0f)
                {
                    owner.SwitchState(new AttackState(owner, col.gameObject));
                }
            }
        }
    }

    void newWaypoint()
    {
        //waypoint = leader.transform.position + (Random.insideUnitSphere * waypointRadius);
        waypoint = leader.transform.position + (leader.transform.forward * waypointRadius) + (leader.transform.right * (Random.Range(-1.0f, 1.0f) * waypointRadius));
        waypoint.y = leader.transform.position.y;
        owner.GetComponent<Boid>().seekTargetPosition = waypoint;
    }
}
