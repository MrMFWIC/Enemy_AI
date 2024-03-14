using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekingEnemy : MonoBehaviour
{
    public float wanderSpeed = 2.0f;
    public float seekSpeed = 4.0f;
    public float arrivalRadius = 1.0f;
    public float attackCooldownDuration = 2.0f;
    public float detectionRadius = 5.0f;
    public float attackDamage = 10.0f;
    public LayerMask playerLayer;

    private enum EnemyState
    {
        Idle,
        Attack,
        AttackCooldown
    }

    private EnemyState currentState;
    private Transform playerTransform;
    private Vector3 targetPosition;
    private float attackCooldownTimer;

    private void Start()
    {
        currentState = EnemyState.Idle;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        attackCooldownTimer = 0.0f;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Attack:
                AttackState();
                break;
            case EnemyState.AttackCooldown:
                AttackCooldownState();
                break;
        }

        if (currentState != EnemyState.AttackCooldown &&
            Physics2D.Raycast(transform.position, Vector2.down, 0.5f, playerLayer))
        {
            Destroy(gameObject);
        }
    }

    private void IdleState()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRadius)
        {
            currentState = EnemyState.Attack;
            return;
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetPosition = GetRandomPoint(transform.position, 5.0f);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, wanderSpeed * Time.deltaTime);
    }

    private void AttackState()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        direction.y = 0;

        transform.position += direction * seekSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, playerTransform.position) <= arrivalRadius)
        {
            AttackPlayer();

            currentState = EnemyState.AttackCooldown;
        }
    }

    private void AttackCooldownState()
    {
        attackCooldownTimer += Time.deltaTime;
        if (attackCooldownTimer >= attackCooldownDuration)
        {
            attackCooldownTimer = 0.0f;
            currentState = EnemyState.Idle;
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Player attacked for " + attackDamage + " damage.");
    }

    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized * radius;
        return center + new Vector3(randomDirection.x, 0, randomDirection.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, arrivalRadius);
    }
}
