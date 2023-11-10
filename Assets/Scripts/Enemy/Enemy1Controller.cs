using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy1Controller : MonoBehaviour
{
    private enum States { IDLE, WALK, HIT}
    private States m_CurrentState;

    private Animator m_Animator;
    private SpriteRenderer sr;

    private float m_StateDeltaTime;

    [SerializeField] private EnemyStats m_stats;
    private int health;

    private bool areaDetector = false;
    private bool hittable = false;
    private bool hitRecive = false;

    public delegate void Attack(int quantity);
    public static Attack attack;

    void Start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        sr = gameObject.GetComponent<SpriteRenderer>();

        health = m_stats.health;
        CharacterController.attack += ondamageRecive;
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
                m_Animator.Play("Walk");
                break;
            case States.HIT:
                m_Animator.Play("Attack");
                attack.Invoke(m_stats.attack);
                break;
            default:
                break;
        }
    }


    private void UpdateState(States updateState)
    {

        m_StateDeltaTime += Time.deltaTime;

        switch (updateState)
        {
            case States.IDLE:

                if (areaDetector)
                {
                    ChangeState(States.WALK);
                }
                if (hittable)
                {
                    ChangeState(States.HIT);
                }
                break;

            case States.WALK:

                if (hittable)
                {
                    ChangeState(States.HIT);
                }
                if (m_StateDeltaTime >= 0.5f)
                    ChangeState(States.IDLE);

                break;

            case States.HIT:

                if (m_StateDeltaTime >= 0.7f)
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
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            hitRecive = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            
            areaDetector = true;

            float distancia = Vector3.Distance(collision.gameObject.transform.position, transform.position);

            Vector3 direccion = collision.gameObject.transform.position - transform.position;
            direccion.Normalize();

            if (direccion.x > 0)
            {
                sr.flipX = false;
            }
            else
            {
                sr.flipX = true;
            }

            if (distancia >= 2)
            {
                transform.position += direccion * m_stats.velocity * Time.deltaTime;
            }

            if (distancia < 2)
            {
                transform.position = transform.position;
                hittable = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            areaDetector = false;
        }

        if (collision.gameObject.layer == 8)
        {
            hitRecive = false;
        }
    }

    private void ondamageRecive(int damage)
    {
        if (hitRecive)
        {
            if (health - damage > 0)
            {
                health -= damage;
            }
            else
            {
                CharacterController.attack -= ondamageRecive;
                Destroy(gameObject);
            }
        }
    }
}
