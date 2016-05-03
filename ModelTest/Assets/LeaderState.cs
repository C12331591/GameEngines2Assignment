using UnityEngine;
using System.Collections;

public class LeaderState : State {

    public LeaderState(FSM machine):base(machine)
    {
        
    }

    public override string Description()
    {
        return "Leader State";
    }

    public override void Enter()
    {
        owner.GetComponent<Boid>().TurnOffAll();
        owner.GetComponent<Boid>().seekEnabled = true;
        //owner.GetComponent<Boid>().seekTargetPosition = owner.transform.position + (owner.transform.forward * 10000);
        owner.GetComponent<Boid>().seekTargetPosition = new Vector3(0, 15, 10000);
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
