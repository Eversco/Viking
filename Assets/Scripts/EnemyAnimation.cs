using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator mAnimator;
    private Enemy enemyMovement;
    private bool attackTriggered;

    void Start()
    {
        enemyMovement = GetComponent<Enemy>();
        attackTriggered = false;
    }

    void Update()
    {
        if (mAnimator != null)
        {
            MyInput();
        }
    }

    private void MyInput()
    {
        if (enemyMovement.currentState == Enemy.EnemyState.Chasing)
        {
            attackTriggered = false;
            mAnimator.SetBool("Run", enemyMovement.distanceToPlayer > 1.5f);
            mAnimator.SetBool("Walk", false);
        }
        else if (enemyMovement.currentState == Enemy.EnemyState.Patrolling && enemyMovement.isPatrolMoving)
        {
            attackTriggered = false;
            mAnimator.SetBool("Walk", true);
            mAnimator.SetBool("Run", false);
        }
        else if (enemyMovement.currentState == Enemy.EnemyState.Attacking && !attackTriggered)
        {
            mAnimator.SetTrigger("Slash");
            attackTriggered = true;
        }
        else if (enemyMovement.currentState != Enemy.EnemyState.Attacking)
        {
            attackTriggered = false;
            mAnimator.SetBool("Run", false);
            mAnimator.SetBool("Walk", false);
        }
    }
}
