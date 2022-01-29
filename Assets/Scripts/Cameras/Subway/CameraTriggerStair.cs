using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTriggerStair : MonoBehaviour
{
    public GameObject myCamera;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {     
            CamerasController.instance.EnableCamera(myCamera);
        }
    }
}
