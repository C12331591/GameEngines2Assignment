using UnityEngine;
using System.Collections;

public class AttackState : State {

    GameObject target;

    public AttackState(FSM machine, GameObject target):base(machine)
    {
        this.target = target;
    }

    public override string Description()
    {
        return "Attack State";
    }

    public override void Enter()
    {
        /*newWaypoint();
        owner.GetComponent<Boid>().seekEnabled = true;
        leader = owner.GetComponent<FSM>().leader;*/
        owner.GetComponent<Boid>().TurnOffAll();
        //owner.GetComponent<Boid>().pursueEnabled = true;
        //owner.GetComponent<Boid>().pursueTarget = target;
        owner.GetComponent<Boid>().seekEnabled = true;
        owner.GetComponent<Boid>().seekTargetPosition = target.transform.position;
    }

    public override void Exit()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        /*if (Vector3.Distance(owner.transform.position, leader.transform.position) > outerRadius)
        {
            waypoint = leader.transform.position;
            owner.GetComponent<Boid>().seekTargetPosition = waypoint;
        }

        if (Vector3.Distance(owner.transform.position, waypoint) <= reachRadius)
        {
            newWaypoint();
        }

        foreach (Collider col in Physics.OverlapSphere(owner.transform.position, outerRadius))
        {
            if (col.tag == owner.GetComponent<FSM>().enemy)
            {
                owner.SwitchState();
            }
        }*/

        owner.GetComponent<Boid>().seekTargetPosition = target.transform.position;

        if (target == null || target.GetComponent<health>().current <= 0)
        {
            owner.GetComponent<FSM>().returnToNormal();
        }

        Vector3 diff = target.transform.position - owner.transform.position;
        float dot = Vector3.Dot(diff, owner.transform.forward);

        if (dot > 0.5f)
        {
            owner.GetComponent<gun>().Shoot();
        }

        if (dot < 0.5f)
        {
            owner.GetComponent<FSM>().returnToNormal();
        }
    }
}
