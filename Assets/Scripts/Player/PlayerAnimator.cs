using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    [SerializeField]
    private GameObject modelHolder;
    private Animator animator;

    private const string MovementXAnimationConditionName = "HorizontalBlend";
    private const string MovementYAnimationConditionName = "VerticalBlend";
    private const string GroundedAnimationConditionName = "IsGrounded";

    void Awake ()
    {
        animator = GetComponent<Animator>();
    }

    public void FlipModelY (bool _isFlipped)
    {
        int flipValue = (_isFlipped) ? -1 : 1;
        modelHolder.transform.localScale = new Vector3(modelHolder.transform.localScale.x, flipValue, 1);
    }

    public void SetFacingDirection (float _moveDirectionX)
    {
        int faceDirection = 0;

        faceDirection = (_moveDirectionX <= 0) ? -1 : 1;
        modelHolder.transform.localScale = new Vector3(faceDirection, modelHolder.transform.localScale.y, 1);
    }

    public void AnimateByMoveDirection (bool _isFlipped, Vector2 _moveDirection)
    {
        int flipMovementValueY = 0;

        flipMovementValueY = (_moveDirection.y <= 0) ? -1 : 1;
        animator.SetFloat(MovementYAnimationConditionName, _moveDirection.y * flipMovementValueY);
        animator.SetFloat(MovementXAnimationConditionName, Mathf.Abs(_moveDirection.x));
    }

    public void SetGroundedState (bool _isGrounded)
    {
        animator.SetBool(GroundedAnimationConditionName, _isGrounded);
    }

}
