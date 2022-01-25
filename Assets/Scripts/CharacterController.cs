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
	[SerializeField] private Transform m_GroundCheck;                           // Uma marcação de posição onde verifica se o jogador está no chão.
	[SerializeField] private Transform m_CeilingCheck;                          // Uma marcação de posição onde verificar os tetos
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // Um colisor que será desativado ao agachar

	[SerializeField] private float _dashSpeed = 15f;
	[SerializeField] private float _dashLength = .2f;
	private Vector2 _dashDir;
	[SerializeField] private float _dashCount = 1f;
	private float _dashTemp;
	[SerializeField] private float _dashInputTime = 1f;
	private bool _isDashing = false;
	private bool _hasDashed;
	private bool _canDash = true;
	private bool _getDashKey = false;


	public float runSpeed = 20f;
	float horizontalMove = 0f;
	bool _isJump = false, _isCrouch = false;
	private float _horizontalDirection;
	private float _verticalDirection;

	const float k_GroundedRadius = .2f; // Constante de um raio do círculo de sobreposição para determinar se é chão
	[SerializeField] private bool m_Grounded;            // Se o jogador está ou não no chão
	const float k_CeilingRadius = .2f; // Constante de um raio do círculo de sobreposição para determinar se o jogador pode ficar de pé
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // Para determinar para que lado o jogador está olhando.
	private Vector3 m_Velocity = Vector3.zero;
	private float vel;
	private Animator animator;
	float auxGrav, auxDrag;

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
		//animator = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		auxGrav = m_Rigidbody2D.gravityScale;
		auxDrag = m_Rigidbody2D.drag;

	}

    void Update()
    {
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

		//animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
		//animator.SetFloat("yVelocity", m_Rigidbody2D.velocity.y);

		

		if (Input.GetButtonDown("Jump"))
			_isJump = true;

		if (Input.GetButtonDown("Crouch"))
		{
			//animator.SetBool("IsCrouching", true);
			_isCrouch = true;
		}
		else if (Input.GetButtonUp("Crouch"))
		{
			//animator.SetBool("IsCrouching", false);
			_isCrouch = false;
		}
		if (Input.GetButtonDown("Dash") && _dashTemp <= 0)
		{
			_dashTemp = _dashCount;
			_isDashing = true;

			StartCoroutine(GetDashKey());
			while (!_getDashKey)
			{
				_horizontalDirection = GetInput().x;
				_verticalDirection = GetInput().y;
				if (_horizontalDirection != 0 || _verticalDirection != 0) break;
			}
			StopCoroutine(GetDashKey());
			_getDashKey = false;
			m_Rigidbody2D.velocity = Vector2.zero;
		}
		else
		{
			//if (_dashBufferCounter > 0) _dashBufferCounter -= Time.deltaTime;
			if (_dashTemp > 0) _dashTemp -= Time.deltaTime;
		}
	}

    private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// O jogador está no chão se um circlecast para a posição de groundcheck atingir qualquer coisa designada como "ground"
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
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

		//if (_canDash) StartCoroutine(Dash(_horizontalDirection, _verticalDirection));

		//Mover o personagem
		Move(horizontalMove * Time.deltaTime, _isCrouch, _isJump, _isDashing);
		_isJump = false;
		//animator.SetBool("IsJumping", !m_Grounded);
	}

	private Vector2 GetInput()
	{
		return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
	}

	private IEnumerator GetDashKey()
	{
		yield return new WaitForSeconds(_dashInputTime);
		_getDashKey = true;
	}
	private IEnumerator StopDashing()
    {
		yield return new WaitForSeconds(_dashLength);

		m_Rigidbody2D.gravityScale = auxGrav;
		m_Rigidbody2D.drag = auxDrag;
		_isDashing = false;
    }
	/*private IEnumerator Dash(float x, float y)
    {
		float dashStartTime = Time.time;
		
		_hasDashed = true;
		_isDashing = true;
		_isJump = false;
		m_Rigidbody2D.velocity = Vector2.zero;
		m_Rigidbody2D.gravityScale = 0f;
		m_Rigidbody2D.drag = 0f;

		Vector2 dir;
		if (x != 0f || y != 0f) dir = new Vector2(x, y);
        else
        {
			if (m_FacingRight) dir = new Vector2(1f, 0f);
			else dir = new Vector2(-1f, 0f);
        }

		while (Time.time < dashStartTime + _dashLength)
        {
			m_Rigidbody2D.velocity = dir.normalized * _dashSpeed;
			yield return null;
		}
		m_Rigidbody2D.gravityScale = auxGrav;
		m_Rigidbody2D.drag = auxDrag;
		_isDashing = false; 
	}*/

	public void Move(float move, bool crouch, bool jump, bool dash)
	{
		// Se estiver agachado, verifique se o personagem consegue se levantar
		/*if (!crouch)
		{
			// Se o personagem tiver um teto impedindo-o de ficar de pé, mantenha-o agachado
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}
		*/

		// só controle o player se estiver no chão ou airControl estiver ativado
		if (m_Grounded || m_AirControl)
		{
			if (dash)
			{
				if (_canDash)
				{
					_canDash = false;

					StartCoroutine(StopDashing());
				}
				if (_isDashing)
				{
					_dashDir = new Vector2(_horizontalDirection, _verticalDirection);
					if (_dashDir == Vector2.zero) _dashDir = new Vector2(transform.localScale.x, 0);
					m_Rigidbody2D.gravityScale = 0f;
					m_Rigidbody2D.drag = 0f;
					m_Rigidbody2D.velocity = _dashDir.normalized * _dashSpeed;
					return;
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
					// Tira a velocidade
					vel = 0;
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
}