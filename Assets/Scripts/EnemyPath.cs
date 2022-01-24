using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    public Transform hasGround, hasWall, Foot;
    public GameObject Player;
    public float range;
    public float FastChaseTime = 15f;
    public float ChaseTime = 10f;
    public float ShootTime = 1f;
    public float FlipTime = 2f;
    public float JumpTime = 2f;


    [SerializeField] private float m_JumpForce = 800f;
    [SerializeField] private float yDist;
    private float _velMove = 2f;
    private float _distToPlayer;
    private Rigidbody2D rb;
    private int move;
    private bool mustPatrol;
    [SerializeField] private bool _hole;
    [SerializeField] private bool _wall;
    private Animator anim;
    [SerializeField] private bool _fastChase;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        move = 1;
        mustPatrol = true;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Physics2D.IgnoreLayerCollision(3, 6);
        if (mustPatrol)
        {
            int i;
            rb.velocity = new Vector2(_velMove * move, rb.velocity.y);
            
            CheckColision();
            i = Random.Range(1, 3000);
            if (i == 1 || _hole || _wall)
            {
                Flip();
                _hole = _wall = false;
            }
        }
        _distToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        if (_distToPlayer <= range && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist)
        {
            if (((Player.transform.position.x > transform.position.x && move > 0)
                || (Player.transform.position.x < transform.position.x && move < 0)) && IsGround() && !_wall)
            {
                mustPatrol = false;
                rb.velocity = Vector2.zero;
                Shoot();
            }
            else if (((Player.transform.position.x > transform.position.x && move < 0)
                || (Player.transform.position.x < transform.position.x && move > 0)) && !mustPatrol)
            {
                Flip();
            }
        }
        if ((_distToPlayer > range  || Mathf.Abs(transform.position.y - Player.transform.position.y) >= yDist || _wall) && !mustPatrol)
        {
            Chase();
            CheckColision();
        }
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void CheckColision()
    {
        _hole = !Physics2D.Raycast(hasGround.position, Vector2.down, 0.1f) ? true : false;
        _wall = Physics2D.Raycast(hasWall.position, new Vector2(move, 0), 0.1f) ? true : false;
    }

    bool IsGround()
    {
        if (Physics2D.Raycast(Foot.position, Vector2.down, 0.1f))
            return true;
        else return false;
    }

    void Flip()
    {
        move *= -1;
        Vector3 temp = transform.localScale;

        temp.x = move > 0 ? Mathf.Abs(temp.x) : -Mathf.Abs(temp.x);
        transform.localScale = temp;
    }
    void Shoot()
    {
        anim.SetBool("Shoot", true);
    }
    void Chase()
    {
        _fastChase = _distToPlayer > range ? true : false;
        
        anim.SetBool("Shoot", false);

        if ((_wall || _hole) && IsGround())
        {
            if (JumpTime == 1)
            {
                rb.AddForce(new Vector2(0f, m_JumpForce));
                JumpTime -= Time.deltaTime;
            }
            else if (JumpTime <= 0)
                JumpTime = 1;
            else
                JumpTime -= Time.deltaTime;
        }

        if ((Player.transform.position.x > transform.position.x && move < 0)
            || (Player.transform.position.x < transform.position.x && move > 0))
        {
            if (FlipTime == 2)
            {
                Flip();
                FlipTime -= Time.deltaTime;
            }
            else if(FlipTime <= 0)
                FlipTime = 2;
            else
                FlipTime -= Time.deltaTime;
        }
        if (_fastChase)
        {
            ChaseTime = 10f;
            if (FastChaseTime > 0)
                FastChaseTime -= Time.deltaTime;
            else
            {
                FastChaseTime = 15f;
                mustPatrol = true;
            }
            rb.velocity = new Vector2(_velMove * 2 * move, rb.velocity.y);
        }
        else
        {
            FastChaseTime = 15f;
            if (ChaseTime > 0)
                ChaseTime -= Time.deltaTime;
            else
            {
                ChaseTime = 10f;
                mustPatrol = true;
            }
            rb.velocity = new Vector2(_velMove / 2 * move, rb.velocity.y);
        }  
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("entrou");
        }
    }
}
