using UnityEngine;
using System.Collections;

public class playable : MonoBehaviour {

    public float aimSpeed = 2.0f;
    public float aimCentre = 20.0f;
    public float aimRadius = 20.0f;
    public float followDistance = 5.0f;
    private Vector3 follow;
    private Vector2 direction;
    //private Vector2 centreVector;

    public bool invertY = true;
    private int y = 1;

    public GameObject chaseCamera = null;

    // Use this for initialization
    void Start()
    {
        //centreVector = new Vector2(aimCentre, aimCentre);

        follow = transform.position + (Vector3.forward * followDistance);
        GetComponent<Boid>().TurnOffAll();
        GetComponent<FSM>().enabled = false;
        GetComponent<Boid>().seekTargetPosition = follow;
        GetComponent<Boid>().seekEnabled = true;

        if (invertY)
        {
            y = -1;
        }

        if (chaseCamera != null)
        {
            chaseCamera.transform.LookAt(transform.position);

            chaseCamera.GetComponent<FPSController>().enabled = false;
            chaseCamera.GetComponent<Boid>().offsetPursueEnabled = true;
            chaseCamera.GetComponent<Boid>().offsetPursueTarget = gameObject;
        }
    }

    // Update is called once per frame
    void Update () {
        follow = transform.position + (Vector3.forward * followDistance);

        Vector2 cVector;

        if (direction.x < 0)
        {
            cVector.x = -aimCentre;
        }
        else
        {
            cVector.x = aimCentre;
        }

        if (direction.y < 0)
        {
            cVector.y = -aimCentre;
        }
        else
        {
            cVector.y = aimCentre;
        }

        direction -= cVector * Time.deltaTime;

        //bool input = false;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            //follow.y += (aimStrength * Time.deltaTime * y);
            direction.y += aimSpeed * Time.deltaTime * y;
            //input = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //follow.y += (aimStrength * Time.deltaTime * -y);
            direction.y += aimSpeed * Time.deltaTime * -y;
            //input = true;
        }

        follow.y = Mathf.Max(follow.y, 1.5f);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //follow.z -= (aimStrength * Time.deltaTime) * transform.forward.x;
            //follow.x -= (aimStrength * Time.deltaTime) * transform.forward.z;
            direction.x -= aimSpeed * Time.deltaTime;
            //input = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            //follow.z += (aimStrength * Time.deltaTime) * transform.forward.x;
            //follow.x += (aimStrength * Time.deltaTime) * transform.forward.z;
            direction.x += aimSpeed * Time.deltaTime;
            //input = true;
        }

        follow += new Vector3(direction.x * transform.forward.z, direction.y, direction.x * transform.forward.z);
        follow.y = Mathf.Max(follow.y, 1.5f);//so things don't go wacky if you go down to the ground
        //follow = Vector2.ClampMagnitude(follow, 200.0f);

        /*if (!input)
        {
            follow = transform.position + (Vector3.forward * followDistance);
        }*/

        GetComponent<Boid>().seekTargetPosition = follow;

        if (chaseCamera != null)
        {
            chaseCamera.transform.LookAt(transform.position);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            GetComponent<gun>().Shoot();
        }

        //follow += Vector3.forward;
    }
}
