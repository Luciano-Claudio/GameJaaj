using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    private Animator anim;
    private Transform player;
    SpriteRenderer spr;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();


        spr.flipX = player.transform.localScale.x < 0 ? true : false;

        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }

}
