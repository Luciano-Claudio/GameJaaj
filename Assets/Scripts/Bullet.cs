using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed = 2f;
    Transform EnemyGun;
    int direction;

    void Start()
    {
        EnemyGun = GetComponentInParent<Transform>();
        direction = EnemyGun.transform.localScale.x <= 0 ? 1 : -1;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(new Vector2(0, direction) * Time.deltaTime * Speed);
    }
}
