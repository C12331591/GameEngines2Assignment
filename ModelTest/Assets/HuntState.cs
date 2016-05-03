using UnityEngine;
using System.Collections;

public class HuntState : State {

    GameObject target;

    public HuntState(FSM machine, GameObject target):base(machine)
    {
        this.target = target;
    }

    public override string Description()
    {
        return "Hunt State";
    }

    public override void Enter()
    {
        owner.GetComponent<Boid>().TurnOffAll();
        //owner.GetComponent<Boid>().seekEnabled = true;
        //owner.GetComponent<Boid>().seekTargetPosition = owner.transform.position + (owner.transform.forward * 10000);
        //owner.GetComponent<Boid>().seekTargetPosition = new Vector3(0, 15, 10000);
        owner.GetComponent<Boid>().pursueEnabled = true;
        owner.GetComponent<Boid>().pursueTarget = target;
        owner.GetComponent<Boid>().channelFindingEnabled = true;
    }

    public override void Exit()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        foreach (Collider col in Physics.OverlapSphere(owner.transform.position, 75.0f))
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
}
