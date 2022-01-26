using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    public Transform hasGround, hasWall, FootOne, FootTwo, Gun;
    public GameObject Bullet;
    private GameObject Player;
    public float ShootRange;
    public float ViewRange;
    public float FastChaseTime = 15f;
    public float ChaseTime = 10f;
    public float ShootTime = 1f;
    public float FlipTime = 1f;
    public float JumpTime = 1f;


    [SerializeField] private float m_JumpForce = 500f;
    [SerializeField] private float yDist;
    private float _velMove = 2f;
    private float _distToPlayer;
    private Rigidbody2D rb;
    private int move;
    private bool mustPatrol;
    private bool _hole;
    private bool _wall;
    public bool _obstacleHole = false;
    private Animator anim;
    [SerializeField] private bool _fastChase;
    private bool _look = false, lookOn = false;
    private bool shootOn = false;
    private bool _flip = true;
    private bool fallOn = false;
    private bool _fall = false;
    private bool _prepairJump = false;
    private bool prepairOn = false;
    private float changeChase = .2f;
    private bool _chaseChange = false;
    private bool _canChaseChange = false;

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

    void Update()
    {
        _distToPlayer = Vector2.Distance(transform.position, Player.transform.position);

        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("IsJump", !IsGround());
        anim.SetFloat("yVelocity", rb.velocity.y);

        if (_prepairJump && IsGround() && !prepairOn) StartCoroutine(PrepairJump());
        else if (_fall && IsGround() && !fallOn) StartCoroutine(Fall());
    }
    void FixedUpdate()
    {
        Physics2D.IgnoreLayerCollision(3, 6);


        if (_distToPlayer <= ViewRange && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist && mustPatrol)
        {
            if (((Player.transform.position.x > transform.position.x && move > 0)
                || (Player.transform.position.x < transform.position.x && move < 0)) && IsGround() && !_wall)
            {
                mustPatrol = false;
            }
        }

        if (mustPatrol)
        {
            int i;
            CheckColision();
            i = Random.Range(1, 6000);
            if (i == 1 || _hole || _wall)
            {
                _look = _flip = true;
                _hole = _wall = false;
            }

            if (_look && !lookOn && IsGround())
                StartCoroutine(Look());
            else if (!lookOn && !_look)
                rb.velocity = new Vector2(_velMove * move, rb.velocity.y);
        }
        else
        {
            if ((Player.transform.position.x > transform.position.x && move < 0)
                       || (Player.transform.position.x < transform.position.x && move > 0))
                Flip();

            if (_distToPlayer <= ShootRange && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist && !_wall)
            {
                if (IsGround())
                {
                    if (!shootOn)
                        StartCoroutine(Shoot());
                }
            }
            else if (_distToPlayer > ShootRange || Mathf.Abs(transform.position.y - Player.transform.position.y) >= yDist || _wall)
            {
                CheckColision();
                Chase();
            }
        }
    }
    private IEnumerator Look()
    {
        if (!fallOn && !prepairOn && !shootOn && !lookOn)
        {
            lookOn = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("Look", true);

            if (_distToPlayer <= ViewRange && Mathf.Abs(transform.position.y - Player.transform.position.y) <= yDist && mustPatrol)//adicionar caixa dps
            {
                anim.SetBool("Look", false);
                _look = lookOn = false;
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
    }
    private IEnumerator Shoot()//adicionar caixa dps
    {
        if (!fallOn && !prepairOn && !shootOn)
        {
            shootOn = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("Shoot", true);
            GameObject bullet = Instantiate(Bullet);
            bullet.transform.position = Gun.position;
            bullet.transform.parent = gameObject.transform;
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length / 2);
            rb.velocity = Vector2.zero;
            anim.SetBool("Shoot", false);
            shootOn = false;
        }
    }
   
    private IEnumerator PrepairJump()
    {
        if (!fallOn && !prepairOn)
        {
            prepairOn = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("PrepairJump", true);
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            anim.SetBool("PrepairJump", false);
            _prepairJump = prepairOn = false;
            rb.AddForce(new Vector2(0f, m_JumpForce));
        }
    }
    private IEnumerator Fall()
    {
        if (!fallOn)
        {
            fallOn = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("Fall", true);
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            anim.SetBool("Fall", false);
            _fall = fallOn = false;
        }
    }

    void CheckColision()
    {
        _hole = !Physics2D.Raycast(hasGround.position, Vector2.down, 0.1f) ? true : false;
        _wall = Physics2D.Raycast(hasWall.position, new Vector2(move, 0), 0.1f) ? true : false;
    }

    bool IsGround()
    {
        if (Physics2D.Raycast(FootOne.position, Vector2.down, 0.1f) || Physics2D.Raycast(FootTwo.position, Vector2.down, 0.1f))
            return true;
        else
        {
            _fall = true;
            return false;
        }
    }

    void Flip()
    {        
        move *= -1;
        Vector3 temp = transform.localScale;

        temp.x = move > 0 ? Mathf.Abs(temp.x) : -Mathf.Abs(temp.x);
        transform.localScale = temp;
    }

    void Chase()
    {
        _chaseChange = _fastChase;
        _fastChase = _distToPlayer > ViewRange ? true : false;
        if(_chaseChange != _fastChase && changeChase > 0)
        {
            _canChaseChange = _fastChase;
            changeChase -= Time.deltaTime;
        }
        if (changeChase < .2f && changeChase > 0f)
            changeChase -= Time.deltaTime;
        else if (changeChase <= 0f)
            changeChase = .2f;


        if ((_wall || _obstacleHole) && IsGround() && !_prepairJump)
        {
            if (JumpTime == 1)
            {
                _prepairJump = true;
                JumpTime -= Time.deltaTime;
            }
        }
        if(JumpTime < 1 && JumpTime > 0)
            JumpTime -= Time.deltaTime;
        else if(JumpTime <= 0)
            JumpTime = 1f;


        if (_canChaseChange)
        {
            ChaseTime = 10f;
            if (FastChaseTime > 0)
                FastChaseTime -= Time.deltaTime;
            else
            {
                FastChaseTime = 15f;
                mustPatrol = true;
            }
            if(!_wall && !prepairOn && !fallOn && !shootOn && !lookOn)
                rb.velocity = new Vector2(_velMove * 2 * move, rb.velocity.y);
            else if(_wall && !prepairOn && !fallOn && !shootOn && !lookOn)
                rb.velocity = new Vector2(_velMove * move, rb.velocity.y);
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
            if(_hole && !prepairOn && !fallOn && !shootOn && !lookOn)
                rb.velocity = new Vector2(_velMove * 2 * move, rb.velocity.y);
            else if(!prepairOn && !fallOn && !shootOn && !lookOn)
                rb.velocity = new Vector2(_velMove / 2 * move, rb.velocity.y);
            else if(_wall && !prepairOn && !fallOn && !shootOn && !lookOn)
                rb.velocity = new Vector2(_velMove * move, rb.velocity.y);
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
