using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBullet : MonoBehaviour
{

    private float Speed = 30f;
    public Transform player;
    private int direction;
    private CharacterController controller;
    Vector2 dir;
    Vector3 rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = new Vector3(0, 0, 0);
        player = GameObject.Find("Player").GetComponent<Transform>();
        controller = player.GetComponent<CharacterController>();
        direction = player.transform.localScale.x >= 0 ? 1 : -1;

        if (controller._vertical > 0 && controller._horizontal != 0 && controller._isCrouch)
        {
            dir = new Vector2(direction, 0);
            if (controller._horizontal > 0) rot.z = 45;
            else if (controller._horizontal < 0) rot.z = 315;
            transform.eulerAngles = rot;
        }
        else if (controller._vertical > 0 && controller._isCrouch)
        {
            dir = Vector2.right;
            rot.z = 90;
            transform.eulerAngles = rot;
        }
        else
            dir = new Vector2(direction, 0);
        Destroy(gameObject, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * Time.deltaTime * Speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyPath enemy = collision.GetComponent<EnemyPath>();
        if(enemy != null) enemy.LessLife();
        Hittler boss = collision.GetComponent<Hittler>();
        if (boss != null) boss.LessLife();
        Destroy(gameObject);
    }
}
