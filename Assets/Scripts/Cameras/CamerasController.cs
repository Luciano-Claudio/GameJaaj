using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerasController : MonoBehaviour
{
    public static CamerasController instance;

    public GameObject[] cameras;
    private void Awake()
    {
        instance = this;
    }

    public void EnableCamera(GameObject camera)
    {
        if (camera.activeInHierarchy)
            return;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].SetActive(false);
        }

        camera.SetActive(true);
    }

}
