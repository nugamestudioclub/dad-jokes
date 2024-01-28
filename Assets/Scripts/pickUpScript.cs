using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class pickUpScript : MonoBehaviour {

	public GameObject dad;
	public Transform itemHoldPos;

	public float throwForce = 500f; //force at which the object is thrown at
	public float pickUpRange = 5f; //how far the dad can pickup the object from

    private float rotationSensitivity = 1f; //how fast/slow the object is rotated in relation to mouse movement
    private GameObject heldObj; //object which we pick up
    private Rigidbody heldObjRb; //rigidbody of object we pick up
    private GameObject lookedAtObj; //object being looked at
    public Material outlineMaterial; //looked at object Material
    private bool canDrop = true; //this is needed so we don't throw/drop object when rotating the object
    private int LayerNumber; //layer index

	private InteractableObject _heldInteractableObject;


    //Reference to script which includes mouse movement of dad (looking around)
    //we want to disable the dad looking around when rotating the object
    //example below 
    //MouseLookScript mouseLookScript;
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer"); //if your holdLayer is named differently make sure to change this ""

        //outlineMaterial = Resources.Load("/dad-jokes/Assets/Art/Shader Graphs_outline.mat", typeof(Material))as Material;

        //mouseLookScript = dad.GetComponent<MouseLookScript>();
    }
    void Update()
    {
        //make interactable item glow
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
            {
                if(hit.transform.gameObject.tag == "canPickUp")
                {
                    ObjectOutline(hit.transform.gameObject);
                }
            }
        if (Input.GetKeyDown(KeyCode.E)) //change E to whichever key you want to press to pick up
        {
            if (heldObj == null) //if currently not holding anything
            {
                //perform raycast to check if dad is looking at object within pickuprange
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    //make sure pickup tag is attached
                    var obj = hit.transform.gameObject;
					//make sure pickup tag is attached
					if( obj.CompareTag("canPickUp") ) {
						//pass in object hit into the PickUpObject function
						PickUpObject(obj);
						_heldInteractableObject = obj.GetComponent<InteractableObject>();
					}
					else if( obj.CompareTag("canInteract") ) {
						InteractWithObject(obj);
					}
					else if( obj.CompareTag("crib") ) {
						if( _heldInteractableObject != null ) {
							InteractWithObject(_heldInteractableObject.gameObject);
							_heldInteractableObject = null;
						}
                    }
                }
            }
            else
            {
                if(canDrop == true)
                {
                    StopClipping(); //prevents object from clipping through walls
                    DropObject();
                }
            }
        }
        if (heldObj != null) //if dad is holding object
        {
            MoveObject(); //keep object position at itemHoldPos
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true) //Mous0 (leftclick) is used to throw, change this if you want another button to be used)
            {
                StopClipping();
                ThrowObject();
            }

		}
	}
	private void InteractWithObject(GameObject obj) {
		if( obj.TryGetComponent<InteractableObject>(out var interactable)
			&& interactable.CanInteract ) {
			Debug.Log($"interacting with {obj.name}");
			interactable.HasInteraction = true;
		}
	}

	void PickUpObject(GameObject pickUpObj) {
		if( pickUpObj.GetComponent<Rigidbody>() ) //make sure the object has a RigidBody
		{
			Debug.Log($"picking up {pickUpObj.name}");

			heldObj = pickUpObj; //assign heldObj to the object that was hit by the raycast (no longer == null)
			heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //assign Rigidbody
			heldObjRb.isKinematic = true;
			heldObjRb.transform.parent = itemHoldPos.transform; //parent object to itemHoldPosition
			heldObj.layer = LayerNumber; //change the object layer to the holdLayer
										 //make sure object doesnt collide with dad, it can cause weird bugs
			Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), dad.GetComponent<Collider>(), true);

		}
	}


	void DropObject() {
		//re-enable collision with dad
		Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), dad.GetComponent<Collider>(), false);
		heldObj.layer = 0; //object assigned back to default layer
		heldObjRb.isKinematic = false;
		heldObj.transform.parent = null; //unparent object
		heldObj = null; //undefine game object
	}
	void MoveObject() {
		//keep object position the same as the itemHoldPosition position
		heldObj.transform.position = itemHoldPos.transform.position;
	}
	void RotateObject() {
		if( Input.GetKey(KeyCode.R) )//hold R key to rotate, change this to whatever key you want
		{
			canDrop = false; //make sure throwing can't occur during rotating

			//disable dad being able to look around
			//mouseLookScript.verticalSensitivity = 0f;
			//mouseLookScript.lateralSensitivity = 0f;

			float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
			float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
			//rotate the object depending on mouse X-Y Axis
			heldObj.transform.Rotate(Vector3.down, XaxisRotation);
			heldObj.transform.Rotate(Vector3.right, YaxisRotation);
		}
		else {
			//re-enable dad being able to look around
			//mouseLookScript.verticalSensitivity = originalvalue;
			//mouseLookScript.lateralSensitivity = originalvalue;
			canDrop = true;
		}
	}
	void ThrowObject() {
		//same as drop function, but add force to object before undefining it
		Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), dad.GetComponent<Collider>(), false);
		heldObj.layer = 0;
		heldObjRb.isKinematic = false;
		heldObj.transform.parent = null;
		heldObjRb.AddForce(transform.forward * throwForce);
		heldObj = null;
	}
	void StopClipping() //function only called when dropping/throwing
	{
		var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //distance from itemHoldPos to the camera
																						  //have to use RaycastAll as object blocks raycast in center screen
																						  //RaycastAll returns array of all colliders hit within the cliprange
		RaycastHit[] hits;
		hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
		//if the array length is greater than 1, meaning it has hit more than just the object we are carrying
		if( hits.Length > 1 ) {
			//change object position to camera position 
			heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above dad 
																						  //if your dad is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
		}
	}
    
    void ObjectOutline(GameObject objectToAddOutlineMaterial)
    {
        //get object and add outline material
        lookedAtObj = objectToAddOutlineMaterial;
        lookedAtObj.GetComponent<MeshRenderer>().material = outlineMaterial;
        
    }
}