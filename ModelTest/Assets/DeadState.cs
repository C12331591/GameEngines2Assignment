using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DeadState : State
{
    public DeadState(FSM owner):base(owner)
    {
        
    }

    public override string Description()
    {
        return "Dead State";
    }

    public override void Enter()
    {
        Boid boid = owner.GetComponent<Boid>();
        boid.TurnOffAll();
        //owner.GetComponent<FishParts>().RagDoll();
        owner.GetComponent<Rigidbody>().useGravity = true;
        //Debug.Log(owner.gameObject.name + " Dead state");

    }

    public override void Exit()
    {
        // We never 
    }

    public override void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "obstacle")
        {
            //owner.Destroy(owner.gameObject);
        }
    }
}
