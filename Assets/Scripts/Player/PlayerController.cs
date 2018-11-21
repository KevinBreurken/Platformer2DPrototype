using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public bool StandingOnPlayer
    {
        get { return standingOnPlayer; }
        private set { standingOnPlayer = value; }
    }

    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    public bool ReadyToJump
    {
        get { return readyToJump; }
        set { readyToJump = value; }
    }

    public float HorizontalMovementValue
    {
        get { return horizontalMovementValue; }
        set { horizontalMovementValue = value; }
    }

    public bool IsGravityFlipped
    {
        get { return gravityFlipped; }
        set { gravityFlipped = value;
            CalculateJumpPhysics();
        }
    }

    public Vector2 MoveDirection
    {
        get { return moveDirection; }
        set
        {
            moveDirection = value;
        }
    }

    public float GetMaxMovementSpeed()
    {
        return maxHorizontalMovementSpeed;
    }

    [Header("Jump")]
    [SerializeField]
    private float maxJumpHeight;
    [SerializeField]
    private float minJumpHeight;
    [SerializeField]
    private float timeToJumpApex;
    [SerializeField]
    private float pushDecreaseFactor;
    [Header("Movement")]
    [SerializeField]
    private float maxHorizontalMovementSpeed;

    private float currentPlayerGravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float horizontalMovementValue;
    private bool standingOnPlayer;
    private bool readyToJump;
    private bool gravityFlipped;
    private bool isGrounded;
    private Vector2 moveDirection;
    private bool isInJumpingState;

    private RaycastBox2D hitBox;

    void Awake ()
    {
        CalculateJumpPhysics();
        hitBox = GetComponent<RaycastBox2D>();
    }

    void Update ()
    {
        IsGrounded = (GetFloorCollisionInfoFromGravityDirection(IsGravityFlipped) || StandingOnPlayer) ? true : false;

        if (hitBox.movementCollisionInfo.below || hitBox.movementCollisionInfo.above)
            moveDirection.y = 0;
        if (StandingOnPlayer)
            moveDirection.y = 0;

        if (ReadyToJump == true)
            if (IsGrounded)
                    moveDirection.y = maxJumpVelocity;


        //TODO: Implement Jump cancel for both gravity directions.
        /*
        This method break gravity switching
        if (ReadyToJump == false)
        {
            
            if (!IsGravityFlipped)
                if (moveDirection.y > minJumpVelocity)
                    moveDirection.y = minJumpVelocity;

            
            if (IsGravityFlipped)
                if (moveDirection.y < minJumpVelocity)
                    moveDirection.y = minJumpVelocity;
            
        }
        */

        moveDirection.x = HorizontalMovementValue * maxHorizontalMovementSpeed;
        moveDirection.y += currentPlayerGravity * Time.deltaTime;

        Move(moveDirection * Time.deltaTime);
    }

    public void Move (Vector2 _moveDirection)
    {
        hitBox.movementCollisionInfo.Reset();
        if (_moveDirection.x != 0)
        {
            hitBox.ClosestCollisionHorizontal(ref _moveDirection);
            CheckForPlayerWhileMovingHorizontal(ref _moveDirection);
        }

        if (MoveDirection.y != 0)
        {
            hitBox.ClosestCollisionVertical(ref _moveDirection);
            CheckForPlayerWhileMovingVertical(ref _moveDirection);
        }

        transform.Translate(_moveDirection);
    }

    public void MoveX (Vector2 _moveDirection)
    {
        hitBox.movementCollisionInfo.ResetHorizontal();
        if (_moveDirection.x != 0)
            hitBox.ClosestCollisionHorizontal(ref _moveDirection);

        transform.Translate(_moveDirection.x, 0, 0);
    }

    public void MoveY (Vector2 _moveDirection)
    {
        hitBox.movementCollisionInfo.ResetVertical();
        if (_moveDirection.y != 0)
            hitBox.ClosestCollisionVertical(ref _moveDirection);

        transform.Translate(0, _moveDirection.y, 0);
    }

    private void CheckForPlayerWhileMovingVertical (ref Vector2 _moveDirection)
    {

        PlayerController foundPlayerController = null;
        RaycastBox2D.RaycastBoundaries.RayCastSide castSide = GetCeilingSideFromGravityDirection();
        List<RaycastHit2D> cast = hitBox.CastInDirection(castSide, true, Mathf.Abs(_moveDirection.y));

        for (int i = 0; i < cast.Count; i++)
        {
            foundPlayerController = cast[i].collider.gameObject.GetComponent<PlayerController>();
            if (foundPlayerController)
                break;
        }

        if (foundPlayerController)
        {
            if (foundPlayerController.GetFloorCollisionInfoFromGravityDirection(foundPlayerController.IsGravityFlipped))
                moveDirection.y = 0;
            if (foundPlayerController.IsGravityFlipped == IsGravityFlipped)
            {
                foundPlayerController.MoveY(_moveDirection);
                foundPlayerController.StandingOnPlayer = true;
            }
            else
            {
                moveDirection.y = 0;
            }

        }
        
        //falling
        StandingOnPlayer = false;
        if (isFalling())
        {
            int hitIndex = -1;
            foundPlayerController = null;
            castSide = GetFloorSideFromGravityDirection();
            cast = hitBox.CastInDirection(castSide, true, Mathf.Abs(_moveDirection.y) + hitBox.SkinWidth);
            for (int i = 0; i < cast.Count; i++)
            {
                foundPlayerController = cast[i].collider.gameObject.GetComponent<PlayerController>();
                if (foundPlayerController)
                {
                    hitIndex = i;
                    break;
                }
            }

            if (foundPlayerController)
            {
                if (IsGravityFlipped)
                {
                    _moveDirection.y = (cast[hitIndex].distance - hitBox.SkinWidth);
                }
                else
                {
                    _moveDirection.y = -(cast[hitIndex].distance - hitBox.SkinWidth);
                }
                if (foundPlayerController.IsGravityFlipped == IsGravityFlipped)
                {
                    StandingOnPlayer = true;
                }
                else
                {
                   moveDirection.y = 0;
                }
            }
        }

    }

    private void CheckForPlayerWhileMovingHorizontal (ref Vector2 _moveDirection)
    {
        //Pushing
        PlayerController foundPlayerController = null;
        RaycastBox2D.RaycastBoundaries.RayCastSide castSide = (_moveDirection.x < 0) ? RaycastBox2D.RaycastBoundaries.RayCastSide.Left : RaycastBox2D.RaycastBoundaries.RayCastSide.Right;
        List<RaycastHit2D> cast = hitBox.CastInDirection(castSide, true, Mathf.Abs(_moveDirection.x) + hitBox.SkinWidth);
        for (int i = 0; i < cast.Count; i++)
        {
            foundPlayerController = cast[i].collider.gameObject.GetComponent<PlayerController>();
            if (foundPlayerController)
            {
                foundPlayerController = cast[i].collider.gameObject.GetComponent<PlayerController>();
                break;
            }
        }

        if (foundPlayerController)
        {
            _moveDirection.x = _moveDirection.x / (1.0f / pushDecreaseFactor);
            foundPlayerController.MoveX(_moveDirection);

            if (foundPlayerController.hitBox.movementCollisionInfo.right || foundPlayerController.hitBox.movementCollisionInfo.left)
                _moveDirection.x = 0;
        }

        foundPlayerController = null;
        cast = hitBox.CastInDirection(GetCeilingSideFromGravityDirection(), true, hitBox.SkinWidth + hitBox.SkinWidth);
        for (int i = 0; i < cast.Count; i++)
        {
            foundPlayerController = cast[i].collider.gameObject.GetComponent<PlayerController>();
            if (foundPlayerController)
            {
                foundPlayerController = cast[i].collider.gameObject.GetComponent<PlayerController>();
                break;
            }
        }
        if (foundPlayerController)
        {
            foundPlayerController.MoveX(_moveDirection);
        }

    }

    private void CalculateJumpPhysics ()
    {
        currentPlayerGravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(currentPlayerGravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(currentPlayerGravity) * minJumpHeight);

        if (IsGravityFlipped)
        {
            currentPlayerGravity = -currentPlayerGravity;
            maxJumpVelocity = -maxJumpVelocity;
            minJumpVelocity = -minJumpVelocity;
        }
    }

    private bool GetFloorCollisionInfoFromGravityDirection(bool _isGravityFlipped)
    {
        if (_isGravityFlipped)
            return hitBox.movementCollisionInfo.above;
        else
            return hitBox.movementCollisionInfo.below;
    }

    private RaycastBox2D.RaycastBoundaries.RayCastSide GetFloorSideFromGravityDirection ()
    {
        if (IsGravityFlipped)
            return RaycastBox2D.RaycastBoundaries.RayCastSide.Above;
        else
            return RaycastBox2D.RaycastBoundaries.RayCastSide.Below;
    }

    private RaycastBox2D.RaycastBoundaries.RayCastSide GetCeilingSideFromGravityDirection ()
    {
        if (IsGravityFlipped)
            return RaycastBox2D.RaycastBoundaries.RayCastSide.Below;
        else
            return RaycastBox2D.RaycastBoundaries.RayCastSide.Above;
    }

    private bool isFalling ()
    {
        if (IsGravityFlipped)
        {
            if (moveDirection.y < 0)
                return false;
            else
                return true;
        }
        else
        {
            if (moveDirection.y < 0)
                return true;
            else
                return false;
        }        
    }

}

