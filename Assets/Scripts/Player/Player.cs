using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController)), 
    RequireComponent(typeof(PlayerInputComponent)),
    RequireComponent(typeof(PlayerAnimator))]
public class Player : MonoBehaviour
{
    public delegate void PlayerEvent (Player _player);
    public event PlayerEvent OnPlayerDeath = delegate
    { };

    public PlayerController PlayerController
    {
        get { return playerController; }
        private set
        {
            playerController = value;
        }
    }

    private PlayerController playerController;
    private PlayerInputComponent playerInput;
    private PlayerAnimator playerAnimator;
    private Vector2 respawnPosition;
    private bool respawnGravityFlipped;

    void Awake ()
    {
        PlayerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInputComponent>();
        playerAnimator = GetComponent<PlayerAnimator>();

        respawnPosition = transform.position;
        respawnGravityFlipped = PlayerController.IsGravityFlipped;
    }

    void Update ()
    {
        playerController.HorizontalMovementValue = playerInput.GetHorizontalInputValue();
        playerController.ReadyToJump = playerInput.GetJumpState();

        playerAnimator.SetGroundedState(PlayerController.IsGrounded);
        playerAnimator.AnimateByMoveDirection(playerController.IsGravityFlipped,playerController.MoveDirection);
        if (playerController.MoveDirection.x != 0)
        playerAnimator.SetFacingDirection(playerController.MoveDirection.x);

    }

    public void ChangeGravity(bool _isflipped)
    {
        playerController.IsGravityFlipped = _isflipped;
        playerAnimator.FlipModelY(_isflipped);
    }

    public void KillPlayer ()
    {
        OnPlayerDeath(this);
    }

    public void RespawnPlayer ()
    {
        transform.gameObject.SetActive(true);
        transform.position = respawnPosition;
        ChangeGravity(respawnGravityFlipped);
        playerController.MoveDirection = Vector3.zero;
    }

}
