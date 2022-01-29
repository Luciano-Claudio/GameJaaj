using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Quantidade de força adicionada quando o jogador pula.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Quantidade de maxSpeed aplicada ao movimento de agachamento. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // Quanto para suavizar o movimento
    [SerializeField] private bool m_AirControl = false;                         // Se um jogador pode ou não se controlar enquanto pula
    [SerializeField] private LayerMask m_WhatIsGround;                          // Uma máscara determinando o que é um chão para o personagem
    [SerializeField] private Transform m_GroundCheckOne;                           // Uma marcação de posição onde verifica se o jogador está no chão.
    [SerializeField] private Transform m_GroundCheckTwo;                           // Uma marcação de posição onde verifica se o jogador está no chão.
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // Um colisor que será desativado ao agachar
    [SerializeField] private int Life = 10;
    private int _lifeAux = 10;
    private bool _dead = false;


    [SerializeField] private GameObject Fade;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform Gun;
    [SerializeField] private Sprite Fire1;
    [SerializeField] private Sprite Fire2;
    private SpriteRenderer Gun_Explosion;

    private float _timeFadeSpawns;
    private float _startTimeFadeSpawns = .05f;
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashLength = .1f;
    private Vector2 _dashDir;
    [SerializeField] private float _dashCount = 1f;
    private float _dashTemp;
    private float _dashInputTime = .1f;
    private bool _isDashing = false;
    private bool _hasDashed = false;
    private bool _canDash = true;



    public float runSpeed = 40f;
    float horizontalMove = 0f;


    private bool _isJump = false;
    private bool _isJumping = false;
    private bool prepairOn = false;
    private bool _fall = false;
    private bool fallOn = false;
    [SerializeField] private float _jumpCount = .5f;
    private float _jumpTemp;

    private float _fireTemp;
    [SerializeField] private float _fireCount = .5f;
    private bool fireOn = false;


    public bool _isCrouch = false;
    private float _horizontalDirection = 0;
    private float _verticalDirection = 0;
    public float _horizontal, _vertical;

    const float k_GroundedRadius = .2f; // Constante de um raio do círculo de sobreposição para determinar se é chão
    [SerializeField] private bool m_Grounded;            // Se o jogador está ou não no chão
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // Para determinar para que lado o jogador está olhando.
    private Vector3 m_Velocity = Vector3.zero;
    private float vel;
    private Animator animator;
    float auxGrav, auxDrag;

    public bool hide { get; private set; }


    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;


    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Gun_Explosion = Gun.GetComponent<SpriteRenderer>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        auxGrav = m_Rigidbody2D.gravityScale;
        auxDrag = m_Rigidbody2D.drag;

        hide = false;
    }

    void Update()
    {
        if (Life <= 0) _dead = true;
        if (!_dead)
        {
                _horizontal = GetInput().x;
                _vertical = GetInput().y;
                horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

                animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
                animator.SetBool("IsJumping", !m_Grounded);
                animator.SetBool("IsDashing", _isDashing);
                if (!_isDashing) animator.SetFloat("yVelocity", m_Rigidbody2D.velocity.y);



                if (Input.GetButtonDown("Jump") && _jumpTemp <= 0 && !_isCrouch)
                {
                    _jumpTemp = _jumpCount;
                    _isJump = _isJumping = true;

                }
                else if (_jumpTemp > 0) _jumpTemp -= Time.deltaTime;

                if (Input.GetButtonDown("Crouch"))
                {
                    _isCrouch = true;
                }
                else if (Input.GetButtonUp("Crouch"))
                {
                    _isCrouch = false;
                    animator.SetBool("xInput", false);
                    animator.SetBool("yInput", false);
                }
                if (Input.GetButtonDown("Dash") && _dashTemp <= 0)
                {
                    _dashTemp = _dashCount;
                    _isDashing = true;
                    StartCoroutine(InputDashing());
                    m_Rigidbody2D.velocity = Vector2.zero;
                }
                else if (_dashTemp > 0) _dashTemp -= Time.deltaTime;

                if (Input.GetButton("Fire1") && _fireTemp <= 0 && !_isDashing && m_Grounded && !fallOn && !fireOn)
                {
                    _fireTemp = _fireCount;
                    StartCoroutine(Fire());
                }
                else if (_fireTemp > 0) _fireTemp -= Time.deltaTime;

                if (_isDashing)
                    _dashInputTime -= Time.deltaTime;
            
        }
        else
        {
            this.enabled = false;
            animator.SetBool("IsDead", true);
        }
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        Collider2D[] colliders;
        // O jogador está no chão se um circlecast para a posição de groundcheck atingir qualquer coisa designada como "ground"
        colliders = Physics2D.OverlapCircleAll(m_GroundCheckOne.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
        colliders = Physics2D.OverlapCircleAll(m_GroundCheckTwo.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }

        }




        if (m_Grounded) _canDash = true;
        else _fall = true;

        if (_fall && m_Grounded && !fallOn) StartCoroutine(Fall());
        //Mover o personagem
        if (!prepairOn && !fallOn)
        {
            Move(horizontalMove * Time.deltaTime, _isCrouch, _isJump, _isDashing);
            _isJump = false;
        }
        if (_timeFadeSpawns > 0) _timeFadeSpawns -= Time.deltaTime;

    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
    private IEnumerator Fire()
    {
        fireOn = true;
        if (m_Rigidbody2D.velocity != Vector2.zero)
        {
            Gun_Explosion.sprite = Fire1;
            yield return new WaitForSeconds(.05f);
            Gun_Explosion.sprite = Fire2;
            yield return new WaitForSeconds(.05f);
            Gun_Explosion.sprite = null;
        }
        animator.SetBool("Fire", true);
        GameObject bullet = Instantiate(Bullet);
        bullet.transform.position = Gun.position;
        yield return new WaitForSeconds(.2f);
        animator.SetBool("Fire", false);
        fireOn = false;
    }
    private IEnumerator Fall()
    {
        if (!fallOn)
        {
            fallOn = true;
            m_Rigidbody2D.velocity = Vector2.zero;
            animator.SetBool("Fall", true);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / 2);
            animator.SetBool("Fall", false);
            _fall = fallOn = _isJumping = false;
        }
    }

    private IEnumerator InputDashing()
    {
        float dashStartTime = Time.time;
        float x, y;


        while (true)
        {
            x = GetInput().x;
            y = GetInput().y;
            if (x != 0 || y != 0)
            {
                _horizontalDirection = x;
                _verticalDirection = y;
            }
            yield return null;
            if (_dashInputTime <= 0)
            {
                _hasDashed = true;
                break;
            }
        }
    }
    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(_dashLength);

        m_Rigidbody2D.gravityScale = auxGrav;
        m_Rigidbody2D.drag = auxDrag;
        _isDashing = false;
        _hasDashed = false;
        _dashInputTime = .1f;
        _horizontalDirection = _verticalDirection = 0;
    }
    public void LessLife()
    {
        Life--;
        animator.SetTrigger("Damaged");
    }
    void Move(float move, bool crouch, bool jump, bool dash)
    {

        // só controle o player se estiver no chão ou airControl estiver ativado
        if (m_Grounded || m_AirControl)
        {
            if (dash)
            {
                if (_hasDashed)
                {
                    if (_canDash)
                    {
                        _canDash = false;

                        StartCoroutine(StopDashing());
                    }
                    if (_isDashing)
                    {
                        if (_timeFadeSpawns <= 0)
                        {
                            Instantiate(Fade, transform.position, Quaternion.identity);
                            _timeFadeSpawns = _startTimeFadeSpawns;
                        }
                        _dashDir = new Vector2(_horizontalDirection, _verticalDirection);
                        if (_dashDir == Vector2.zero) _dashDir = new Vector2(transform.localScale.x, 0);
                        m_Rigidbody2D.gravityScale = 0f;
                        m_Rigidbody2D.drag = 0f;
                        m_Rigidbody2D.velocity = _dashDir.normalized * _dashSpeed;
                        return;
                    }
                }
            }
            else
            {
                // Se agachado
                if (crouch)
                {

                    if (!m_wasCrouching)
                    {
                        m_wasCrouching = true;
                        OnCrouchEvent.Invoke(true);
                    }
                    if(_horizontal != 0) animator.SetBool("xInput", true);
                    else animator.SetBool("xInput", false);
                    if (_vertical > 0) animator.SetBool("yInput", true);
                    else animator.SetBool("yInput", false);
                    // Tira a velocidade
                    vel = 0;
                    animator.SetBool("IsCrouching", true);
                    // Reduza a velocidade pelo multiplicador crouchSpeed
                    move *= m_CrouchSpeed;

                    // Desative um dos colisores ao agachar
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = false;
                }
                else
                {
                    // Coloca a velocidade
                    vel = 1;
                    animator.SetBool("IsCrouching", false);
                    // Ative o colisor quando não estiver agachado
                    if (m_CrouchDisableCollider != null)
                        m_CrouchDisableCollider.enabled = true;

                    if (m_wasCrouching)
                    {
                        m_wasCrouching = false;
                        OnCrouchEvent.Invoke(false);
                    }
                }

                // Mova o personagem usando -1 ou 1 que são axis horizontais enviada pelo parametro
                Vector3 targetVelocity = new Vector2(move * 10f * vel, m_Rigidbody2D.velocity.y);
                // E depois aplicando suavidade no movimento
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);


            }
            // Se o input estiver movendo o player para a direita e o player estiver virado para a esquerda..
            if (move > 0 && !m_FacingRight)
            {
                // ... flip o player.
                Flip();
            }
            // Caso contrário, se o input estiver movendo o player para a esquerda e o player estiver virado para a direita...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Se o jogador pular...
            if (m_Grounded && jump)
            {
                // Adicione uma força vertical ao jogador.
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
        }
    }


    private void Flip()
    {
        // Muda o valor Booleano, se tiver true vira false e vice-versa
        m_FacingRight = !m_FacingRight;

        // Multiplica o player x local scale por -1 para inverter. 
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void ChangeHide(bool valor)
    {
        hide = valor;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            LessLife();
        }
    }
}