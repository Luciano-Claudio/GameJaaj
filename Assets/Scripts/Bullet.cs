using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float Speed = 30f;

    void Start()
    {
        Destroy(gameObject, .3f);
    }

    void Update()
    {
        transform.Translate(new Vector2(1, 0) * Time.deltaTime * Speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
