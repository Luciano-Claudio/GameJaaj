using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittler : MonoBehaviour
{
    public bool IsRange = false;

    [SerializeField] private Transform Resp1;
    [SerializeField] private Transform Resp2;
    [SerializeField] private GameObject Soldier;
    private Animator animator;
    private bool callOn = false, atk1On = false, atk2On = false;
    private int call = 1;
    private GameObject Player;
    private CharacterController playerControl;
    [SerializeField] private int life = 10;
    private bool _dead = false;
    [SerializeField] private float time = 2f;
    [SerializeField]private float _distToPlayer;
    void Start()
    {
        animator = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");
        playerControl = Player.GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        if (life <= 0) _dead = true;
        if (_dead)
        {
            animator.SetBool("IsDead", true);
            animator.SetTrigger("Dead");
            this.enabled = false;
        }
        _distToPlayer = Vector2.Distance(transform.position, Player.transform.position);
        if (_distToPlayer < 12.5f)
        {
            IsRange = _distToPlayer <= 1.2f ? true : false;

            if (time <= 0)
            {
                if (IsRange && !callOn && !atk1On && !atk2On)
                {
                    int x;
                    x = Random.Range(1, 5);
                    if (x == 1) StartCoroutine(Call());
                    else StartCoroutine(Atk1());
                }
                else if (!IsRange && !callOn && !atk1On && !atk2On)
                {
                    StartCoroutine(Call());
                }
                time = Random.Range(2, 7);
            }
            else
            {
                time -= Time.deltaTime;
            }
        }
    }

    public void LessLife()
    {
        life--;
        animator.SetTrigger("Damaged");
    }
    IEnumerator Call()
    {
        if (!callOn)
        {
            callOn = atk1On = atk2On = false;
            callOn = true;
            StopCoroutine(Atk1());
            StopCoroutine(Atk2());
            animator.SetBool("Atk1", false);
            animator.SetBool("Atk2", false);
            atk1On = atk2On = false;


            animator.SetBool("Call", true);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            if (call == 1)
            {
                GameObject enemy = Instantiate(Soldier, Resp1.position, Quaternion.identity);
                enemy.GetComponent<EnemyPath>().Spawn = true;
                call = 2;
            }
            else
            {
                GameObject enemy = Instantiate(Soldier, Resp1.position, Quaternion.identity);
                enemy.GetComponent<EnemyPath>().Spawn = true;
                call = 1;
            }
            animator.SetBool("Call", false);
            callOn = false;
        }
    }
    IEnumerator Atk1()
    {
        if (!atk1On && !atk2On)
        {
            callOn = atk1On = atk2On = false;
            atk1On = true;
            animator.SetBool("Atk1", true);
            yield return new WaitForSeconds(.8f);
            if (IsRange) playerControl.LessLife();
            yield return new WaitForSeconds(.5f);
            animator.SetBool("Atk1", false);
            int i;
            i = Random.Range(1, 5);
            if (i == 1) StartCoroutine(Atk2());
            atk1On = false;
        }
    }
    IEnumerator Atk2()
    {
        if (!atk2On)
        {
            callOn = atk1On = atk2On = false;
            atk1On = true;
            animator.SetBool("Atk2", true);
            yield return new WaitForSeconds(.3f);
            if (IsRange) playerControl.LessLife();
            yield return new WaitForSeconds(.3f);
            animator.SetBool("Atk2", false);
            atk2On = false;
        }
    }

}
