using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveDadCam : MonoBehaviour
{

    public Transform cameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
