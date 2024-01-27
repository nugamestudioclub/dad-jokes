using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dadMovementScript : MonoBehaviour
{

    public GameObject dad;

    // Start is called before the first frame update
    void Start()
    {
        dad = GameObject.Find("Cube");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
