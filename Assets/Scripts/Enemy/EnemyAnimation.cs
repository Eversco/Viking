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
        
    }
}
