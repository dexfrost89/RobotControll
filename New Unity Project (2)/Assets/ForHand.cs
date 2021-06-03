using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgentsExamples;

public class ForHand : MonoBehaviour
{
    public ArmAgent myAgent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "target")
        {
            //other.GetComponent<TargetController>().SetHand(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {/*
        foreach(var point in collision.contacts)
        {
            if(point.point.y < 4.5f)
            {
                //myAgent.AddReward(-1);
                //Debug.Log("wow");
            }
        }*/
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
