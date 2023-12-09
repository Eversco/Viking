using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{

    public Animator mAnimator;
    private Enemy enemyMovement;

    // Start is called before the first frame update
    void Start()
    {
        enemyMovement = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            MyInput();
        }
    }

    private void MyInput()
    {
        if(enemyMovement.currentState == Enemy.EnemyState.Chasing)
        {
            mAnimator.SetBool("Run", true);
            mAnimator.SetBool("Walk", false);
        }
        else if(enemyMovement.currentState == Enemy.EnemyState.Patrolling && enemyMovement.isPatrolMoving)
        {
            mAnimator.SetBool("Walk", true);
            mAnimator.SetBool("Run", false);
        }
        else
        {
            mAnimator.SetBool("Run", false);
            mAnimator.SetBool("Walk", false);
        }
    }
}
