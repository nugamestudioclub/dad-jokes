using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dadMovementScript : MonoBehaviour
{

   
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation = 0;
    float yRotation = 0;

    private Vector3 babyPos;

    private bool aKeyHasBeenPressed = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GetBabyPos();
        transform.LookAt(babyPos);


    }

    // Update is called once per frame
    private void Update()
    {
        //get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //rotate cam and orientation
        if(Input.anyKey)
        {
            aKeyHasBeenPressed = true;
        }
        if(aKeyHasBeenPressed)
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }

    private void GetBabyPos()
    {
    GameObject baby = GameObject.Find("BabySwaddle");
    Transform babyTransform = baby.transform;
    //get baby pos
    babyPos = babyTransform.position;
    }
}
