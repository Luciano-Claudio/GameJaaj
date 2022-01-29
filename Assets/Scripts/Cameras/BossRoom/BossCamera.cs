using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{

    public GameObject myCamera;
    public BoxCollider2D wall;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponent<Transform>();
            player.Translate(Vector2.right);
            CamerasController.instance.EnableCamera(myCamera);
            wall.isTrigger = false;
            this.enabled = false;
        }
    }

}
