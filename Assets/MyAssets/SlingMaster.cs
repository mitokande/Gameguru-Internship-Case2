using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingMaster : MonoBehaviour
{
    public Animator anim;
    private Vector3 startpos;
    private Vector3 endpos;
    private Vector3 shootforce;
    public Rigidbody rb;
    public GameObject sling;
    public LineRenderer line;
    public LineRenderer trajectoryline;
    private Vector3 cursorScreenPoint;
    public int pointsnumber;
    private Vector3 screenPoint;
    private Rigidbody[] ragdoll;

    public void Start()
    {
        ragdoll = GetComponentsInChildren<Rigidbody>();
        //DISABLING THE RAGDOLL
        foreach(Rigidbody rag in ragdoll)
        {
            rag.isKinematic = true;
            rag.detectCollisions = false;
        }
        rb.detectCollisions = true;
        rb.isKinematic = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        //ENABLE RAGDOLL IF PLAYER HITS A WALL
        if (other.CompareTag("Wall"))
        {
            foreach (Rigidbody rag in ragdoll)
            {
                rag.isKinematic = false;
                rag.detectCollisions = true;

            }
            anim.enabled = false;
            trajectoryline.enabled = false;

        }
    }
    public void OnMouseDown()
    {
        //STARTING AIMING ON MOUSEDOWN
        startpos = Input.mousePosition;
        anim.SetBool("ondrag", true);

        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);


    }
    public void OnMouseDrag()
    {
        //POSITIONING PLAYER POSITION AND CALLING TRAJECTORY FUNCTION
        endpos = Input.mousePosition;
        cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint);
        Vector3 mypos = new Vector3(-Mathf.Clamp(cursorPosition.x, -1.5f, 1.5f), Mathf.Clamp(cursorPosition.y, 0, 1f), Mathf.Clamp(cursorPosition.z, -1, 0.1f));
        transform.position = mypos;

        line.SetPosition(1, new Vector3(sling.transform.position.x, sling.transform.position.y - 0.2f, sling.transform.position.z - 0.5f));
        Trajectory();
    }
    public void OnMouseUp()
    {
        //SHOOTING THE PLAYER BY CALLING THE SLINGSHOT FUNCTION
        endpos = Input.mousePosition;
        Slingshot();
        
        rb.useGravity = true;
        anim.SetBool("ondrag", false);
    }

    public void Slingshot()
    {
        //PLAYER IS SHOT ACCORDING TO HOW MUCH THE MOUSE IS PULLED DOWN
        shootforce = (startpos - endpos)/5f;
        rb.AddForce((-shootforce.x),shootforce.y,shootforce.y*2);

    }


    public void Trajectory()
    {
        trajectoryline.enabled = true;
        Vector3 velocity = (startpos - Input.mousePosition) / rb.mass * Time.fixedDeltaTime;
        List<Vector3> linepoints = new List<Vector3>();
        float timeStepInterval = 0.5f;
        int maxSteps = (int)(pointsnumber / timeStepInterval);
        Vector3 directionVector = transform.up;
        Vector3 launchPosition = transform.position + transform.up;
        
        for (int i = 0; i < maxSteps; ++i)
        {
            //ADJUSTING THE TRAJECTORY LINE FOR EVERY STEP
            Vector3 calculatedPosition = launchPosition + directionVector * velocity.y / 10 * i * timeStepInterval;
            calculatedPosition.y -= timeStepInterval- Physics.gravity.y / 8 * Mathf.Pow(i * timeStepInterval, 2);
            calculatedPosition.z += velocity.z + 15 * i * timeStepInterval;
            calculatedPosition.x -= velocity.x / 10f * i * timeStepInterval;

            linepoints.Add(calculatedPosition);
        }
        trajectoryline.positionCount = linepoints.Count;
        trajectoryline.SetPositions(linepoints.ToArray());
    }
}
