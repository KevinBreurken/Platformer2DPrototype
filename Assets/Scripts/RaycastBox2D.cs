using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects hits by casting rays from an objects boundaries
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class RaycastBox2D: MonoBehaviour
{
    [System.Serializable]
    public struct CollisionInfo
    {
        public bool above, below, left, right;

        //Slopes
        public bool isClimbingSlope; 
        public float slopeAngle;
        public float slopeAngleOld;

        public void Reset ()
        {
            above = false;
            below = false;
            left = false;
            right = false;

            isClimbingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }

        public void ResetHorizontal ()
        {
            left = false;
            right = false;
        }

        public void ResetVertical ()
        {
            below = false;
            above = false;
        }
    }

    public CollisionInfo movementCollisionInfo;

    public float SkinWidth
    {
        get { return skinWidth; }
    }

    [Header("Raycast Controller")]
    [SerializeField]
    private RaycastBoundaries raycastOrigins = new RaycastBoundaries();
    [SerializeField]
    private LayerMask collisionMask;
    [SerializeField]
    private LayerMask triggerCollisionMask;

    [Header("Slopes")]
    [SerializeField]
    private bool movesOnSlopes;
    [SerializeField]
    private float maxSlopeAngle;
    private const float skinWidth = .015f;

    void Awake ()
    {
        raycastOrigins.SetBox(GetComponent<BoxCollider2D>());
    }

    public void ClosestCollisionVertical (ref Vector2 _moveDirection)
    {
        float directionY = Mathf.Sign(_moveDirection.y);
        float rayLength = Mathf.Abs(_moveDirection.y) + skinWidth;

        List<RaycastHit2D> rayHits = new List<RaycastHit2D>();
        RaycastBoundaries.RayCastSide sideToDetect = (directionY == -1) ? RaycastBoundaries.RayCastSide.Below : RaycastBoundaries.RayCastSide.Above;
        rayHits = CastInDirection(sideToDetect, false, rayLength);
        float closestHit = float.MaxValue;
        if (rayHits.Count > 0)
        {
            
            movementCollisionInfo.below = directionY == -1;
            movementCollisionInfo.above = directionY == 1;

            for (int i = 0; i < rayHits.Count; i++)
            {
                if (rayHits[i].distance < closestHit)
                {
                    closestHit = rayHits[i].distance;
                }
            }

          
            _moveDirection.y = (closestHit - skinWidth) * directionY;
            if (movementCollisionInfo.isClimbingSlope)
            {
                _moveDirection.x = _moveDirection.y / Mathf.Tan(movementCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(_moveDirection.x);
            }
        }

    }

    public void ClimbSlope (ref Vector2 _moveDirection, float _slopeAngle)
    {
        float moveDistance = Mathf.Abs(_moveDirection.x);
        float climbDirectionY = Mathf.Sin(_slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (_moveDirection.y <= climbDirectionY)
        {
            _moveDirection.y = climbDirectionY;
            _moveDirection.x = Mathf.Cos(_slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(_moveDirection.x);
            movementCollisionInfo.below = true;
            Debug.Log("Call");
            movementCollisionInfo.isClimbingSlope = true;
            movementCollisionInfo.slopeAngle = _slopeAngle;
        }
    }

    public void ClosestCollisionHorizontal (ref Vector2 _moveDirection)
    {
        float directionX = Mathf.Sign(_moveDirection.x);
        float rayLength = Mathf.Abs(_moveDirection.x) + skinWidth;

        List<RaycastHit2D> rayHits = new List<RaycastHit2D>();
        RaycastBoundaries.RayCastSide sideToDetect = (directionX == -1) ? RaycastBoundaries.RayCastSide.Left : RaycastBoundaries.RayCastSide.Right;
        rayHits = CastInDirection(sideToDetect, false, rayLength);

        float closestHit = float.MaxValue;
        if (rayHits.Count > 0)
        {
            float slopeAngle = 0;
            for (int i = 0; i < rayHits.Count; i++)
            {
                if (movesOnSlopes)
                {
                    slopeAngle = Vector2.Angle(rayHits[i].normal, Vector2.up);

                    if (i == 0 && slopeAngle <= maxSlopeAngle)
                    {
                        float distanceToSlopeStart = 0;
                        if(slopeAngle != movementCollisionInfo.slopeAngleOld)
                        {
                            distanceToSlopeStart = rayHits[0].distance - skinWidth;
                            _moveDirection.x -= distanceToSlopeStart * directionX;
                        }
                        ClimbSlope(ref _moveDirection, slopeAngle);
                        _moveDirection.x += distanceToSlopeStart * directionX;
                        
                    }
                }

                if (rayHits[i].distance < closestHit)
                {
                    closestHit = rayHits[i].distance;
                }

                if (movesOnSlopes)
                {
                    if (!movementCollisionInfo.isClimbingSlope || slopeAngle > maxSlopeAngle)
                    {
                        _moveDirection.x = (closestHit - skinWidth) * directionX;
                        if (movementCollisionInfo.isClimbingSlope)
                        {
                            _moveDirection.y = Mathf.Tan(movementCollisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(_moveDirection.x);
                        }
                        movementCollisionInfo.left = directionX == -1;
                        movementCollisionInfo.right = directionX == 1;
                    }
                }
            }

      
        }

    }

    /// <summary>
    /// Casts ray's from a specific direction
    /// </summary>
    public List<RaycastHit2D> CastInDirection (RaycastBoundaries.RayCastSide _raycastSide, bool usesTriggerMask, float _rayDistance)
    {
        raycastOrigins.UpdateRaycastOrigins();

        RaycastHit2D rayHit = new RaycastHit2D();
        List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();
        _rayDistance = Mathf.Abs(_rayDistance) + skinWidth;

        for (int i = 0; i < raycastOrigins.GetRayAmountFromRaycastSide(_raycastSide); i++)
        {
            //Get starting point
            Vector2 rayStart = raycastOrigins.GetOriginFromRaycastSide(_raycastSide);
            //Space out ray's on x or y Axis.
            rayStart += raycastOrigins.GetRayStartDirectionFromRaycastSide(_raycastSide) * (raycastOrigins.GetRayStartSpacingFromRaycastSide(_raycastSide) * i);
            //cast ray
            LayerMask maskToCheck = usesTriggerMask ? triggerCollisionMask : collisionMask;
            rayHit = Physics2D.Raycast(rayStart, raycastOrigins.GetRayCastDirectionFromRaycastSide(_raycastSide), _rayDistance, maskToCheck);

            if (rayHit)
            {
                raycastHits.Add(rayHit);
            }
        }
        return raycastHits;

    }

    [System.Serializable]
    public class RaycastBoundaries
    {
        public enum RayCastSide
        {
            Above,
            Below,
            Left,
            Right
        }

        [SerializeField]
        [Range(1, 10)]
        private int horizontalRayCount;
        [SerializeField]
        [Range(1, 10)]
        private int verticalRayCount;

        //TODO: topRight isn't used, commented out to stop the warning.
        private Vector2 topLeft, /*topRight,*/ bottomLeft, bottomRight;
        private BoxCollider2D objectCollider;
        private float horizontalRaySpacing;
        private float verticalRaySpacing;

        public void SetBox (BoxCollider2D _boxCollider2D)
        {
            objectCollider = _boxCollider2D;
            CalculateRaySpacing();
        }

        public void UpdateRaycastOrigins ()
        {
            Bounds bounds = objectCollider.bounds;
            bounds.Expand(skinWidth * -2);

            bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            topLeft = new Vector2(bounds.min.x, bounds.max.y);
            /*topRight = new Vector2(bounds.max.x, bounds.max.y);*/
        }

        public Vector2 GetOriginFromRaycastSide (RayCastSide _raycastSide)
        {
            switch (_raycastSide)
            {
                default:
                return topLeft;
                case RayCastSide.Above:
                return topLeft;
                case RayCastSide.Below:
                return bottomLeft;
                case RayCastSide.Left:
                return bottomLeft;
                case RayCastSide.Right:
                return bottomRight;
            }
        }

        public Vector2 GetRayStartDirectionFromRaycastSide (RayCastSide _raycastSide)
        {
            switch (_raycastSide)
            {
                default:
                return Vector2.up;
                case RayCastSide.Above:
                return Vector2.right;
                case RayCastSide.Below:
                return Vector2.right;
                case RayCastSide.Left:
                return Vector2.up;
                case RayCastSide.Right:
                return Vector2.up;
            }
        }

        public Vector2 GetRayCastDirectionFromRaycastSide (RayCastSide _raycastSide)
        {
            switch (_raycastSide)
            {
                default:
                return Vector2.up;
                case RayCastSide.Above:
                return Vector2.up;
                case RayCastSide.Below:
                return Vector2.down;
                case RayCastSide.Left:
                return Vector2.left;
                case RayCastSide.Right:
                return Vector2.right;
            }
        }

        public float GetRayStartSpacingFromRaycastSide (RayCastSide _raycastSide)
        {
            switch (_raycastSide)
            {
                default:
                return horizontalRaySpacing;
                case RayCastSide.Above:
                return verticalRaySpacing;
                case RayCastSide.Below:
                return verticalRaySpacing;
                case RayCastSide.Left:
                return horizontalRaySpacing;
                case RayCastSide.Right:
                return horizontalRaySpacing;
            }
        }

        public int GetRayAmountFromRaycastSide (RayCastSide _raycastSide)
        {
            switch (_raycastSide)
            {
                default:
                return horizontalRayCount;
                case RayCastSide.Above:
                return verticalRayCount;
                case RayCastSide.Below:
                return verticalRayCount;
                case RayCastSide.Left:
                return horizontalRayCount;
                case RayCastSide.Right:
                return horizontalRayCount;
            }
        }

        private void CalculateRaySpacing ()
        {
            Bounds bounds = objectCollider.bounds;
            bounds.Expand(skinWidth * -2);

            horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }
    }
}
