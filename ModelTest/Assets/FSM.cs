using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour {
    //public float hunger = 0;
    //public float maxHunger = 30.0f;

    State state = null;

    public GameObject leader;
    public GameObject target;
    public string enemy = "enemy";

    State normalState;

	// Use this for initialization
	void Start () {
        if (GetComponent<playable>().enabled)
        {
            normalState = new PlayingState(this);
            SwitchState(normalState);
        }
        else if (leader != null)
        {
            normalState = new patrolAroundState(this, leader);
            SwitchState(normalState);
        }
        else if (target != null)
        {
            normalState = new HuntState(this, target);
            SwitchState(normalState);
        }
        else
        {
            normalState = new LeaderState(this);
            SwitchState(normalState);
        }
        //SwitchState(new IdleState(this));
        //StartCoroutine("Consume");    
	}

    public void returnToNormal()
    {
        SwitchState(normalState);
    }

    /*System.Collections.IEnumerator Consume()
    {
        while (hunger < maxHunger)
        {
            hunger++;
            // Change to black the more hungry we are
            //Color spawnColor = GetComponent<FishParts>().spawnColor;
            GetComponent<Boid>().maxSpeed = ((maxHunger - hunger) / maxHunger) * 5.0f;
            for (int j = 0; j < transform.childCount; j++)
            {                
                //transform.GetChild(j).GetComponent<Renderer>().material.color = Color.Lerp(spawnColor, Color.black, hunger / maxHunger);
            }
            yield return new WaitForSeconds(1.0f);
        }
        SwitchState(new DeadState(this));
    }*/

    public void SwitchState(State state)
    {
        if (this.state != null)
        {
            this.state.Exit();
        }

        this.state = state;

        if (this.state != null)
        {
            this.state.Enter();
        }
    }


    // Update is called once per frame
    void Update () {
        if (state != null)
        {
            state.Update();
        }

        if (GetComponent<health>().current <= 0)
        {
            SwitchState(new DeadState(this));
        }
	}

    void OnTriggerEnter(Collider other)
    {
        /*if ((other.gameObject.tag == "food"))
        {
            if ((food == null) && (hunger > 10))
            {
                food = other.gameObject;
                SwitchState(new ChaseFoodState(this, other.gameObject));
            }

        }*/
        /*if ((other.gameObject.tag == "obstacle"))
        {
            //Debug.Log("Hit (t)!");
            SwitchState(new DeadState(this));
        }*/
    }

    void OnCollisionEnter(Collision col)
    {
        if ((col.gameObject.tag == "obstacle"))
        {
            //Debug.Log("Hit! (c)");
            //SwitchState(new DeadState(this));
        }
    }

    /*
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == hunger)
        {
            GetComponent<Boid>().seekTargetPosition = hunger.transform.position;
        }       
    }
    */

    //GameObject food;
    /*void OnTriggerExit(Collider other)
    {
        if ((other.gameObject == food))
        {
            food = null;            
            SwitchState(new IdleState(this));
        }
    }
    */
}
