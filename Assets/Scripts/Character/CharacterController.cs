using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    private enum States { IDLE, WALK, HIT, COMBO }
    private States m_CurrentState;

    private Animator m_Animator;
    private SpriteRenderer sr;

    private float m_StateDeltaTime;

    //[SerializeField] private GameManager gameManager;

    public delegate void Attack(int quantity);
    public static Attack attack;
    public delegate void HealthText(int quantity);
    public static HealthText health_txt;

    [SerializeField] private CharacterStats m_stats;
    private int health;
    private PlayerActions m_playerActions;
    private Rigidbody2D m_rigidBody;

    private bool hitRecive = false;

    private void Awake()
    {
        m_playerActions = new PlayerActions();
    }

    private void OnEnable()
    {
        m_playerActions.Player_map.Enable();
        m_playerActions.Player_map.Attack.performed += AttackAction;
    }

    private void OnDisable()
    {
        m_playerActions.Player_map.Disable();
    }

    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        m_rigidBody = gameObject.GetComponent<Rigidbody2D>();

        health = m_stats.health;
        Enemy1Controller.attack += onDamageRecive;
        Enemy2Controller.attack += onDamageRecive;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState(m_CurrentState);
    }
    private void ChangeState(States newState)
    {
        if (newState == m_CurrentState)
            return;

        ExitState(m_CurrentState);
        InitState(newState);
    }

    private void InitState(States initState)
    {
        m_CurrentState = initState;
        m_StateDeltaTime = 0;

        switch (m_CurrentState)
        {
            case States.IDLE:
                m_Animator.Play("Idle");
                break;
            case States.WALK:
                if (m_rigidBody.velocity.x > 0)
                {
                    sr.flipX = false;
                }
                else
                {
                    sr.flipX = true;
                }

                m_Animator.Play("Run");
                break;
            case States.HIT:
                m_Animator.Play("Attack1");
                attack.Invoke(m_stats.attack);
                break;
            case States.COMBO:
                m_Animator.Play("Attack2");
                attack.Invoke(m_stats.combo);
                break;
            default:
                break;
        }
    }


    private void UpdateState(States updateState)
    {
        
        m_StateDeltaTime += Time.deltaTime;
        m_rigidBody.velocity = m_playerActions.Player_map.Movement.ReadValue<Vector2>() * m_stats.velocity;

        switch (updateState)
        {
            case States.IDLE:
                if (m_playerActions.Player_map.Movement.ReadValue<Vector2>() != Vector2.zero)
                    ChangeState(States.WALK);
                break;

            case States.WALK:

                if (m_StateDeltaTime >= 0.5f && m_playerActions.Player_map.Movement.ReadValue<Vector2>() == Vector2.zero)
                    ChangeState(States.IDLE);

                break;

            case States.HIT:

                if (m_StateDeltaTime >= 0.5f)
                {
                    ChangeState(States.IDLE);
                }
                else if(m_playerActions.Player_map.Combo.IsPressed() && m_StateDeltaTime < 0.5f)
                {
                    ChangeState(States.COMBO);
                }

                break;
            case States.COMBO:

                if (m_StateDeltaTime >= 0.5f)
                {
                    ChangeState(States.IDLE);
                }

                break;
            default:
                break;
        }
    }

    private void ExitState(States exitState)
    {
        switch (exitState)
        {
            case States.IDLE:
                break;
            case States.WALK:
                break;
            case States.HIT:
                break;
            case States.COMBO:
                break;
            default:
                break;
        }
    }

    private void AttackAction(InputAction.CallbackContext callbackContext)
    {
        ChangeState(States.HIT);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9 || collision.gameObject.layer == 10)
        {
            hitRecive = true;
        }
    }

    private void onDamageRecive(int damage)
    {
        if (hitRecive)
        {
            if (health - damage > 0)
            {
                health -= damage;
                health_txt.Invoke(health);
            }
            else
            {
                Enemy1Controller.attack -= onDamageRecive;
                Enemy2Controller.attack -= onDamageRecive;
                SceneManager.LoadScene("GameOver");
            }
        }
    }
}
