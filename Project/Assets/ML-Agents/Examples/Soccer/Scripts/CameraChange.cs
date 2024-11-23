using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class CameraChange : MonoBehaviour
{
  	public Camera[] cameras;
	private int currentCameraIndex;
	
	// Use this for initialization
	void Start () {
		currentCameraIndex = 0;
		
		//Turn all cameras off, except the first default one
		for (int i=1; i<cameras.Length; i++) 
		{
			cameras[i].gameObject.SetActive(false);
        }
        if (cameras.Length > 0)
        {
            cameras[currentCameraIndex].gameObject.SetActive(true);
        }
        
    }
    void Update() {
        //Press the 'C' key to cycle through cameras in the array
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCameraIndex ++;
            //If cameraIndex is in bounds, set this camera active and last one inactive
            if (currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex-1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
            //If last camera, cycle back to first camera
            else
            {
                cameras[currentCameraIndex-1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
        }
    }
    
}
