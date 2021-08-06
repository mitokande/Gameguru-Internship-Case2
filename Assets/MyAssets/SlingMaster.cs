using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingMaster : MonoBehaviour
{
    public Animator anim;
    public Vector3 startpos;
    public Vector3 endpos;
    public Vector3 shootforce;
    public Rigidbody rb;
    public GameObject sling;
    public LineRenderer line;
    public LineRenderer trajectoryline;
    
    public int pointsnumber;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Rigidbody[] ragdoll;
    bool ishot = true;
    public void Start()
    {
        ragdoll = GetComponentsInChildren<Rigidbody>();
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
        startpos = Input.mousePosition;
        anim.SetBool("ondrag", true);
        ishot = false;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = (gameObject.transform.position-Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)));


    }
    public void OnMouseDrag()
    {

        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) ;
        Vector3 mypos = new Vector3(-Mathf.Clamp(cursorPosition.x, -1, 1), Mathf.Clamp(cursorPosition.y, 0, 1f), Mathf.Clamp(cursorPosition.z, -1, 0.1f));
        transform.position = mypos;

        line.SetPosition(1, new Vector3(sling.transform.position.x,sling.transform.position.y-0.2f,sling.transform.position.z-0.5f));
        Trajectory();
    }
    public void OnMouseUp()
    {

        endpos = Input.mousePosition;
        Slingshot();
        
        rb.useGravity = true;
        anim.SetBool("ondrag", false);
    }

    public void Slingshot()
    {
        shootforce =   startpos - endpos ;
        shootforce /= 5;
        ishot = true;
        rb.AddForce((-shootforce.x)/2,shootforce.y,shootforce.y);

    }

    private void Update()
    {
        if (!ishot)
        {
            trajectoryline.enabled = true;
            Trajectory();
        }

    }

    public void Trajectory()
    {
        Vector3 velocity = (screenPoint-Input.mousePosition) / rb.mass * Time.fixedDeltaTime;
        List<Vector3> linepoints = new List<Vector3>();
        float timeStepInterval = 0.1f;
        int maxSteps = (int)(pointsnumber / timeStepInterval);
        Vector3 directionVector = transform.up;
        Vector3 launchPosition = transform.position + transform.up;
       
        for (int i = 0; i < maxSteps; ++i)
        {

            Vector3 calculatedPosition = launchPosition + directionVector * velocity.y/10 * i * timeStepInterval; 
            calculatedPosition.y += Physics.gravity.y / 12 * Mathf.Pow(i * timeStepInterval, 2); 
            calculatedPosition.z += velocity.z + 5 * Mathf.Pow(i * timeStepInterval, 2);
            calculatedPosition.x -= Mathf.Clamp(velocity.x, -12, 12)/ 5 * i * timeStepInterval;
            
            linepoints.Add(calculatedPosition);
        }
        trajectoryline.positionCount = linepoints.Count;
        trajectoryline.SetPositions(linepoints.ToArray());
    }
}
