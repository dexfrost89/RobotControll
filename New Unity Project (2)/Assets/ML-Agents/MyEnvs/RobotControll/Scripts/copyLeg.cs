using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyLeg : MonoBehaviour
{
    public GameObject leg;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<BodyAgent>().leg1 = leg.GetComponent<LegAgent>();
        GameObject leg1 = Instantiate(leg, new Vector3(-0.6f, 0f, 0f) + transform.position, Quaternion.Euler(0f, 180f, 0f), transform);
        gameObject.GetComponent<BodyAgent>().leg2 = leg1.GetComponent<LegAgent>();
        GameObject leg2 = Instantiate(leg, new Vector3(0f, 0f, 0.6f) + transform.position, Quaternion.Euler(0f, -90f, 0f), transform);
        gameObject.GetComponent<BodyAgent>().leg3 = leg2.GetComponent<LegAgent>();
        GameObject leg3 = Instantiate(leg, new Vector3(0f, 0f, -0.6f) + transform.position, Quaternion.Euler(0f, 90f, 0f), transform);
        gameObject.GetComponent<BodyAgent>().leg4 = leg3.GetComponent<LegAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
