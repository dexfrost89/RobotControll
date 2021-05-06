using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplyObjects : MonoBehaviour
{

    public GameObject template;
    public int copyX, copyZ;
    public float offsetX, offsetZ;

    public bool set_weight;
    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake() {
        for(int i = 0; i < copyX; i++) {
            for(int j = 0; j < copyZ; j++) {
                GameObject instance = Instantiate(template, new Vector3(i * offsetX, template.transform.position.y, j * offsetZ), new Quaternion());
                instance.name = template.name + "_" + i.ToString() + "_"  + j.ToString();
                instance.SetActive(true);

                if(set_weight) {
                    SetWeight(instance);
                }
            }
        }
    }

    

    [Header("Body Parts' Weights")] [Space(10)] public float body;
    public float leg0Upper;
    public float leg0Lower;
    public float leg1Upper;
    public float leg1Lower;
    public float leg2Upper;
    public float leg2Lower;
    public float leg3Upper;
    public float leg3Lower;

    public void SetWeight(GameObject instance) {
        instance.GetComponent<SpiderAgent>().body.gameObject.GetComponent<Rigidbody>().mass = body;
        instance.GetComponent<SpiderAgent>().leg0Upper.gameObject.GetComponent<Rigidbody>().mass = leg0Upper;
        instance.GetComponent<SpiderAgent>().leg0Lower.gameObject.GetComponent<Rigidbody>().mass = leg0Lower;
        instance.GetComponent<SpiderAgent>().leg1Upper.gameObject.GetComponent<Rigidbody>().mass = leg1Upper;
        instance.GetComponent<SpiderAgent>().leg1Lower.gameObject.GetComponent<Rigidbody>().mass = leg1Lower;
        instance.GetComponent<SpiderAgent>().leg2Upper.gameObject.GetComponent<Rigidbody>().mass = leg2Upper;
        instance.GetComponent<SpiderAgent>().leg2Lower.gameObject.GetComponent<Rigidbody>().mass = leg2Lower;
        instance.GetComponent<SpiderAgent>().leg3Upper.gameObject.GetComponent<Rigidbody>().mass = leg3Upper;
        instance.GetComponent<SpiderAgent>().leg3Lower.gameObject.GetComponent<Rigidbody>().mass = leg3Lower;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
