using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    private Hittler hittler;
    private void Start()
    {
        hittler = GetComponentInParent<Hittler>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hittler.enabled = true;
        }
    }
}
