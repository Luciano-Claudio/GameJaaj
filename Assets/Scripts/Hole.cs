using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private EnemyPath Enemy;

    private void Awake()
    {
        Enemy = GetComponentInParent<EnemyPath>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole"))
        {
            Enemy._obstacleHole = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole"))
        {
            Enemy._obstacleHole = false;
        }
    }
}
