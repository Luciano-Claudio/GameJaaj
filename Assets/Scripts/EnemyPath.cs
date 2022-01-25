using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    public Transform hasGround, hasWall, Foot, Gun;
    public GameObject Player, Bullet;
    public float ShootRange;
    public float ViewRange;
    public float FastChaseTime = 15f;
    public float ChaseTime = 10f;
    public float ShootTime = 1f;
    public float FlipTime = 1f;
    public float JumpTime = 1f;


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
    private bool _look = false, lookOn = false;
    private bool shootOn = false;
    private bool _flip = true;
    private bool _jump = true;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        move = 1;
        mustPatrol = true;
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Physics2D.IgnoreLayerCollision(3, 6);
        if (mustPatrol)
        {
            int i;
            CheckColision();
            i = Random.Range(1, 3000);
            if (i == 1 || _hole || _wall)
            {
                _look = _flip = true;
                _hole = _wall = false;
            }

            if (_look && !lookOn)
                StartCoroutine(Look());
            else if (!lookOn && !_look)
                rb.velocity = new Vector2(_velMove * move, rb.velocity.y);

        }
        _distToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        if (_distToPlayer <= ViewRange && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist && mustPatrol) 
        { 
            if (((Player.transform.position.x > transform.position.x && move > 0)
                || (Player.transform.position.x < transform.position.x && move < 0)) && IsGround() && !_wall)
            {
                mustPatrol = false;
            }
        }

        if ((_distToPlayer <= ShootRange && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist) && !mustPatrol)
        {
            if (((Player.transform.position.x > transform.position.x && move > 0)
                || (Player.transform.position.x < transform.position.x && move < 0)) && IsGround() && !_wall)
            {
                rb.velocity = Vector2.zero;
                if (!shootOn)
                {
                    StartCoroutine(Shoot());
                }
            }
            else if (((Player.transform.position.x > transform.position.x && move < 0)
                || (Player.transform.position.x < transform.position.x && move > 0)) && !mustPatrol)
                Flip();
        }
        if ((_distToPlayer > ShootRange || Mathf.Abs(transform.position.y - Player.transform.position.y) >= yDist || _wall) && !mustPatrol)
        {
            Chase();
            CheckColision();
        }

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }
    private IEnumerator Shoot()//adicionar caixa dps
    {
        shootOn = true;
        anim.SetBool("Shoot", true);
        GameObject bullet = Instantiate(Bullet);
        bullet.transform.position = Gun.position;
        bullet.transform.parent = gameObject.transform;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length/2);
        anim.SetBool("Shoot", false);
        shootOn = false;
    }
    private IEnumerator Look()
    {
        lookOn = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("Look", true);

        if (_distToPlayer <= ViewRange && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist)//adicionar caixa dps
        {
            anim.SetBool("Look", false);
            _look = lookOn = _jump = false;
            mustPatrol = false;

            if (((Player.transform.position.x > transform.position.x && move < 0)
                || (Player.transform.position.x < transform.position.x && move > 0)))
                Flip();
            yield return null;
        }
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime / 5);
        anim.SetBool("Look", false);
        _look = lookOn = false;
        if (_flip)
            Flip();
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
    void Jump()
    {
        if (_jump)
        {
            rb.AddForce(new Vector2(0f, m_JumpForce));
        }
        _jump = true;
    }

    void Chase()
    {
        _fastChase = _distToPlayer > ViewRange ? true : false;
        
        anim.SetBool("Shoot", false);

        if ((_wall || _hole) && IsGround())
        {
            if (JumpTime == 1)
            {
                Jump();
                JumpTime -= Time.deltaTime;
            }
        }
        if(JumpTime < 1 && JumpTime > 0)
            JumpTime -= Time.deltaTime;
        else if(JumpTime <= 0)
            JumpTime = 1f;


        if ((Player.transform.position.x > transform.position.x && move < 0)
            || (Player.transform.position.x < transform.position.x && move > 0))
        {
            if (FlipTime == 1)
            {
                Flip();
                FlipTime -= Time.deltaTime;
            }
            else if(FlipTime <= 0)
                FlipTime = 1;
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
        else//adicionar caixa dps
        {
            FastChaseTime = 15f;
            if (ChaseTime > 0)
                ChaseTime -= Time.deltaTime;
            else
            {
                ChaseTime = 10f;
                mustPatrol = true;
            }
            int i;
            i = Random.Range(1, 1500);
            if (i == 1)
            {
                _flip = false;
                StartCoroutine(Look());
            }
            else
            {
                if(_wall || _hole)
                    rb.velocity = new Vector2(_velMove * 2 * move, rb.velocity.y);
                else
                    rb.velocity = new Vector2(_velMove / 2 * move, rb.velocity.y);
            }
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
