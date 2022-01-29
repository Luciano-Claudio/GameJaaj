using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    GameObject player;
    CharacterController playerCh;
    Animator anim;
    int life;
    bool hide = false;
    bool playerOn = false;
    BoxCollider2D trigger;
    CapsuleCollider2D top;
    float _distToPlayer;

    private void Start()
    {
        player = GameObject.Find("Player");
        playerCh = player.GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        trigger = GetComponent<BoxCollider2D>();
        top = GetComponent<CapsuleCollider2D>();
        life = 3;
    }
    private void FixedUpdate()
    {
        if(transform.position.x > player.transform.position.x)
        {
            if (Mathf.Abs(transform.position.x - player.transform.position.x) <= 0.5f)
                playerOn = true;
            else playerOn = false;
        }
        else
        {
            if (Mathf.Abs(transform.position.x - player.transform.position.x) <= 0.2f)
                playerOn = true;
            else playerOn = false;
        }
        anim.SetInteger("Life", life);
        if (life == 0)
        {
            trigger.enabled = false;
            top.enabled = false;
        }
        if (playerOn)
        {
            if (playerCh._isCrouch) playerCh.ChangeHide(true);
            else playerCh.ChangeHide(false);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            top.enabled = false;
            //playerOn = true;
        }
        if (collision.CompareTag("Bullet"))
        {
            life--;
            if(!hide) Destroy(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerOn = false;
            StartCoroutine(EnableTop());
        }
    }

    IEnumerator EnableTop()
    {
        yield return new WaitForSeconds(0.2f);
        top.enabled = true;
    }
}
