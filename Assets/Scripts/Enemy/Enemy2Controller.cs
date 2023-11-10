using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy2Controller : MonoBehaviour
{
    private enum States { WALK, HIT}
    private States m_CurrentState;

    [SerializeField] private GameObject[] m_pathPoints;
    private int pathIndex = 0;
    private Animator m_Animator;
    private SpriteRenderer sr;

    private float m_StateDeltaTime;

    [SerializeField] private EnemyStats m_stats;
    private int health;

    private bool hittable = false;
    private bool hitRecive = false;

    public delegate void Attack2(int quantity);
    public static Attack2 attack;

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
            case States.WALK:
                m_Animator.Play("Walk");
                break;
            case States.HIT:
                StartCoroutine(ShootCoroutine());
                m_Animator.Play("Shot");
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
            case States.WALK:

                float distancia = Vector3.Distance(m_pathPoints[pathIndex].gameObject.transform.position, transform.position);

                if (distancia >= 2 && !hittable)
                {
                    Vector3 direccion = m_pathPoints[pathIndex].gameObject.transform.position - transform.position;
                    direccion.Normalize();

                    if (direccion.x > 0)
                    {
                        sr.flipX = false;
                    }
                    else
                    {
                        sr.flipX = true;
                    }

                    transform.position += direccion * m_stats.velocity * Time.deltaTime;
                }
                else if (distancia < 2 && !hittable)
                {
                    if (pathIndex == 0)
                    {
                        pathIndex = 1;
                    }
                    else
                    {
                        pathIndex = 0;
                    }
                }

                if (hittable)
                {
                    ChangeState(States.HIT);
                }

                break;

            case States.HIT:

                if (m_StateDeltaTime >= 1f && !hittable)
                {
                    ChangeState(States.WALK);
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

            hittable = true;

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

            transform.position = transform.position;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            hittable = false;
            StopCoroutine(ShootCoroutine());
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

    IEnumerator ShootCoroutine()
    {
        while (hittable)
        {
            attack.Invoke(m_stats.attack);
            Debug.Log("Disparo");
            yield return new WaitForSeconds(2f);
        }
    }
}
