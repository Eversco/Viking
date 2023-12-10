using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{

    public Animator mAnimator;
    private PlayerMovement mPlayerMovement;


    // Start is called before the first frame update
    void Start()
    {
        mPlayerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mAnimator != null)
        {
            MyInput();
        }
    }

    private void MyInput()
    {
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S)))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                mAnimator.SetBool("Run", true);
                mAnimator.SetBool("Walk", false);
            }
            else
            {
                mAnimator.SetBool("Walk", true);
                mAnimator.SetBool("Run", false);
            }
        }
        else
        {
            mAnimator.SetBool("Walk", false);
            mAnimator.SetBool("Run", false);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            mAnimator.SetTrigger("Jumping");
        }

        mAnimator.SetBool("grounded", mPlayerMovement.grounded);

        
    }

    public void TriggerAttack()
    {
        mAnimator.SetTrigger("Chop");
    }
}
