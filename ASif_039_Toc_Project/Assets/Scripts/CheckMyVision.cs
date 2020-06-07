using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CheckMyVision : MonoBehaviour
{
    
    //How sensitvity we are about

    public enum enumSensitivity {HIGH,LOW};

    // variable to check senstivity

    public enumSensitivity sensitivity = enumSensitivity.HIGH;

    //we are able to see the target now

    public bool targetInSight = false;

    // Field of vision

    public float fieldOfVision = 90f;

    // we ned a reference to our target here as well

    private Transform target = null;

    // Rferenece to our eyes = yet to add
    public Transform myEye = null;

    // My transform Component
    public Transform npcTransform = null;

    // My sphere collider
    private SphereCollider sphereCollider=null;

    // Last known sighting in object

    public Vector3 lastknownsighting = Vector3.zero;
    

    private void Awake()
    {
        npcTransform = GetComponent<Transform>();
        sphereCollider = GetComponent<SphereCollider>();
        lastknownsighting = npcTransform.position;
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); // Okay we shall tag

    }

    bool InMyFieldofVision()
    {
        Vector3 dirToTarget = target.position - myEye.position;

        // Get angle between forward and view direction
        float angle = Vector3.Angle(myEye.forward, dirToTarget);
        // Let us check if within field of view

        if (angle <= fieldOfVision)
            return true;
        else
            return false;

    }

    // We need a function to check line of sight

    bool ClearLineofSight() {

        RaycastHit hit;

        if (Physics.Raycast(myEye.position, (target.position - myEye.position).normalized,
             out hit, sphereCollider.radius))
        {
            if(hit.transform.CompareTag("Player"))
            {
                return true;
            }

        }
        return false;
    
    }

   
    
   private void OnTriggerStay(Collider other)
    {
      
        UpdateSight();
        if (targetInSight)
            lastknownsighting = target.position;
    }

    private void OnTriggerExit(Collider other)
    {

        if (!other.CompareTag("Player"))
            return;
        targetInSight = false;
    }
        void UpdateSight()
    {
        switch(sensitivity)
        {
            case enumSensitivity.HIGH:
                targetInSight = InMyFieldofVision() && ClearLineofSight();
                break;
            case enumSensitivity.LOW:
                targetInSight = InMyFieldofVision() || ClearLineofSight();
                break;

        }

    }

    
  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
