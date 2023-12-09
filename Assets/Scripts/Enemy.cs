using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField] private float additionalGravity = 20f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Ground Check")]
    [SerializeField] private float height = 2f;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    [Header("Patrolling")]
    [SerializeField] private float patrolMoveTime = 3f; // Time the enemy moves in one direction
    [SerializeField] private float patrolCooldown = 2f; // Time between movements
    private float patrolTimer;
    private float cooldownTimer;
    private Vector3 patrolDirection;
    public bool isPatrolMoving;

    public Transform Model;


    public float speed = 0.01f;
    float timeCount = 0.0f;

    private Transform player;

    private Rigidbody rb;

    public enum EnemyState { Patrolling, Chasing }
    public EnemyState currentState;

    private void Start()
    {
        currentState = EnemyState.Patrolling;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        GameObject playerGameObject = GameObject.FindWithTag("Player");
        if (playerGameObject != null)
        {
            player = playerGameObject.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure your player is tagged correctly.");
        }
    }

    private void Update()
    {
        GroundCheck();
        ApplyAdditionalGravity();

        switch (currentState)
        {
            case EnemyState.Patrolling:
                PatrollingBehavior();
                break;
            case EnemyState.Chasing:
                ChasingBehavior();
                break;
        }
    }

    private void PatrollingBehavior()
    {
        if (isPatrolMoving)
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0)
            {
                isPatrolMoving = false;
                cooldownTimer = patrolCooldown;
            }
            else
            {
                MoveInDirection(patrolDirection);
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isPatrolMoving = true;
                patrolTimer = patrolMoveTime;
                ChooseNewPatrolDirection();
            }
        }

        // Transition to chasing if player is detected
        if (IsPlayerDetected())
        {
            ChangeState(EnemyState.Chasing);
        }
    }

    private void ChooseNewPatrolDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        patrolDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
        patrolDirection.Normalize();
    }

    private void MoveInDirection(Vector3 direction)
    {
        rb.velocity = direction * 1.4f;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 7f);
        //transform.rotation = lookRotation;
    }

    private void ChasingBehavior()
    {
        MoveTowardsPlayer();

        // Transition back to patrolling if player is lost
        if (!IsPlayerDetected())
        {
            ChangeState(EnemyState.Patrolling);
        }
    }

    private bool IsPlayerDetected()
    {
        // Simple distance-based detection for example
        return Vector3.Distance(transform.position, player.position) < 10f;
    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        // Handle any setup or cleanup on state change
    }

    private void MoveTowardsPlayer()
    {
        if (player != null && grounded)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, timeCount * speed);
            timeCount = timeCount + Time.deltaTime;
            //transform.rotation = lookRotation;
            rb.velocity = direction * moveSpeed;
            Debug.DrawLine(transform.position, transform.position + direction * 5, Color.red, 0.1f);
        }
    }

    private void GroundCheck()
    {
        RaycastHit hit;
        grounded = Physics.SphereCast(transform.position + Vector3.up, 0.125f, Vector3.down, out hit, height / 2, whatIsGround);
    }

    private void ApplyAdditionalGravity()
    {
        if (!grounded)
        {
            rb.AddForce(Vector3.down * additionalGravity, ForceMode.Force);
        }
    }
}
