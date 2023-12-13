using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable, IMoveable
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

    public float speed = 0.01f;

    private Transform player;

    private Rigidbody rb;

    public float distanceToPlayer;

    public float attackRange = 15f;
    public float attackCooldown = 1f;
    public Transform orientation;
    public AnimationControl animationControl;

    public float lastAttackTime;

    public float MaxHealth { get; set; }
    public float CurrentHealth { get ; set; }
    Rigidbody2D IMoveable.rb { get; set; }
    public bool isFacingRight { get; set; }

    #region State Machine Var
    public EnemyStateMachine stateMachine {  get; set; }
    public EnemyPatrol PatrolState { get; set; }
    public EnemyChase ChaseState { get; set; }
    public EnemyAttack AttackState { get; set; }
    #endregion

    #region Idle Var
    public float Rand_MoveRange = 5f;
    public float Rand_Move_Speed = 8f;
    public float _changeDirectionTimer;
    public float _currentTimer;
    #endregion

    private void Awake()
    {
        stateMachine = new EnemyStateMachine();

        PatrolState = new EnemyPatrol(this, stateMachine);
        ChaseState = new EnemyChase(this, stateMachine);
        AttackState = new EnemyAttack(this, stateMachine);
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;
        rb = GetComponent<Rigidbody>();

        stateMachine.Initialize(PatrolState);

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
        stateMachine.CurrentEnemyState.FrameUpdate();

        if (transform.position.y < -50)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();
        ApplyAdditionalGravity();
        stateMachine.CurrentEnemyState.PhysicsUpdate();
        
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
    #region Health/Die
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        
    }
    #endregion
    #region Movement
    public void MoveEnemy(Vector3 velocity)
    {
        rb.velocity = velocity;
        
    }

    #endregion
    #region Animation Triggers
    public enum AnimationTriggerType
    {
        Idle,
        Walk,
        Run,
        Attack
    }
    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        stateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }
    #endregion
}
