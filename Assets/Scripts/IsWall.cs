using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWall : MonoBehaviour
{
    private EnemyPath Enemy;

    private void Awake()
    {
        Enemy = GetComponentInParent<EnemyPath>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Enemy._wall = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Enemy._wall = false;
        }
    }
}
