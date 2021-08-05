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
    private Vector3 screenPoint;
    private Vector3 offset;
    private Rigidbody[] ragdoll;
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
            GetComponent<CapsuleCollider>().enabled = false;

        }
    }
    public void OnMouseDown()
    {
        startpos = Input.mousePosition;
        anim.SetBool("ondrag", true);

        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = (gameObject.transform.position-Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)));


    }
    public void OnMouseDrag()
    {
        // sling.transform.position = endpos;
        Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
        Vector3 mypos = new Vector3(-Mathf.Clamp(cursorPosition.x, -1, 1), Mathf.Clamp(cursorPosition.y, 0, 1f), Mathf.Clamp(cursorPosition.z, -1, 0.1f));
        transform.position = mypos;
        //sling.transform.position = new Vector3(-Mathf.Clamp(cursorPosition.x, -1, 1), Mathf.Clamp(cursorPosition.y, 0, 1f)+1f, Mathf.Clamp(cursorPosition.z, -2f, 0f));
        line.SetPosition(1, new Vector3(sling.transform.position.x,sling.transform.position.y-0.2f,sling.transform.position.z-0.5f));
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
        rb.AddForce((-shootforce.x)/5,shootforce.y,shootforce.y);

    }
}
