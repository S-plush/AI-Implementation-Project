using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;

    private bool movingPressed;
    private bool isJogging;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movingPressed = Input.GetKey(KeyCode.W);

        isJogging = animator.GetBool("isJogging");

        //when player starts moving starts jogging animation
        if(!isJogging && movingPressed)
        {
            animator.SetBool("isJogging", true);
        }

        //when player stops moving jogging animation stops
        if(isJogging && !movingPressed)
        {
            animator.SetBool("isJogging", false);
        }
    }
}
