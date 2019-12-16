using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateCameraLeft()
    {
        transform.Rotate(0, 90f, 0);
    }

    public void RotateCameraRight()
    {
        transform.Rotate(0, -90f, 0);       
    }
}
