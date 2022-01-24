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
	

	const float k_GroundedRadius = .2f; // Constante de um raio do círculo de sobreposição para determinar se é chão
	private bool m_Grounded;            // Se o jogador está ou não no chão
	const float k_CeilingRadius = .2f; // Constante de um raio do círculo de sobreposição para determinar se o jogador pode ficar de pé
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // Para determinar para que lado o jogador está olhando.
	private Vector3 m_Velocity = Vector3.zero;
	private float mass, mass_Crouch, vel;
	private Animator animator;
	private Vector3 cursorPos;

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
		mass = m_Rigidbody2D.mass;
		mass_Crouch = mass * 2;
		//animator = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
		
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
		//animator.SetBool("IsJumping", !m_Grounded);

		cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		if ((cursorPos.x < transform.position.x && m_FacingRight)
			|| (cursorPos.x > transform.position.x && !m_FacingRight))
        {
			Flip();
        }
	}


	public void Move(float move, bool crouch, bool jump)
	{
		// Se estiver agachado, verifique se o personagem consegue se levantar
		if (!crouch)
		{
			// Se o personagem tiver um teto impedindo-o de ficar de pé, mantenha-o agachado
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		// só controle o player se estiver no chão ou airControl estiver ativado
		if (m_Grounded || m_AirControl)
		{

			// Se agachado
			if (crouch)
			{

				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}
				// Aumenta seu peso
				m_Rigidbody2D.mass = mass_Crouch;
				// Reduza a velocidade pelo multiplicador crouchSpeed
				move *= m_CrouchSpeed;

				// Desative um dos colisores ao agachar
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Diminui seu peso
				m_Rigidbody2D.mass = mass;
				// Ative o colisor quando não estiver agachado
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}
			// Se o input estiver movendo o player para a direita e o player estiver virado para a esquerda..
			if (move > 0 && !m_FacingRight)
			{
				// ... flip o player.
				Flip();
				vel = 2;
			}
			// Caso contrário, se o input estiver movendo o player para a esquerda e o player estiver virado para a direita...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
				vel = 2;
			}
            else
            {
				vel = 1;
            }

			// Mova o personagem usando -1 ou 1 que são axis horizontais enviada pelo parametro
			Vector3 targetVelocity = new Vector2(move * 10f / vel, m_Rigidbody2D.velocity.y);
			// E depois aplicando suavidade no movimento
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			
		}
		// Se o jogador pular...
		if (m_Grounded && jump)
		{
			// Adicione uma força vertical ao jogador.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
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