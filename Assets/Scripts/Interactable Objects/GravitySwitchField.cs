using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColliderDetectionComponent))]
public class GravitySwitchField : MonoBehaviour
{
    [SerializeField]
    private LayerMask gravityOrientationCollisionMask;

    private enum FieldType
    {
        Horizontal,
        Vertical
    }
    private FieldType fieldType;
    private ColliderDetectionComponent colliderDetection;
    private bool leftIsFlipped;
    private bool rightIsFlipped;

    private List<Player> collidedPlayers = new List<Player>();

    void Awake ()
    {
        colliderDetection = GetComponent<ColliderDetectionComponent>();
        colliderDetection.OnTriggerEnterDetect += ColliderDetection_OnTriggerEnterDetect;
        colliderDetection.OnTriggerExitDetect += ColliderDetection_OnTriggerExitDetect;

        DetermineFieldType();
    }

    void Update ()
    {
        if (collidedPlayers.Count > 0)
            for (int i = 0; i < collidedPlayers.Count; i++)
            {
                if (fieldType == FieldType.Horizontal)
                    CheckPlayerHorizontalFieldType(collidedPlayers[i]);
                else
                    CheckPlayerVerticalFieldType(collidedPlayers[i]);
            }
    }

    private void ColliderDetection_OnTriggerExitDetect (Collider2D _collider)
    {
        Player player = _collider.GetComponent<Player>();
        if (player != null)
        {
            if (collidedPlayers.Contains(player) == true)
            {
                collidedPlayers.Remove(player);
            }
        }
    }

    private void ColliderDetection_OnTriggerEnterDetect (Collider2D _collider)
    {
        Player player = _collider.GetComponent<Player>();
        if (player != null)
        {
            if (collidedPlayers.Contains(player) == false)
            {
                collidedPlayers.Add(player);
            }
        }
    }

    private void DetermineFieldType ()
    {
        if (transform.localScale.x > transform.localScale.y)
            fieldType = FieldType.Horizontal;
        else
            fieldType = FieldType.Vertical;

        if (fieldType == FieldType.Vertical)
        {
            //Check sides for what gravity should be set to
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position + Vector3.left, Vector2.up, (transform.localScale.y / 2), gravityOrientationCollisionMask);
            if (rayHit)
            {
                leftIsFlipped = true;
                rightIsFlipped = false;
            }

            if (!rayHit)
            {
                rayHit = Physics2D.Raycast(transform.position + Vector3.right, Vector2.up, (transform.localScale.y / 2), gravityOrientationCollisionMask);
                if (rayHit)
                {
                    leftIsFlipped = false;
                    rightIsFlipped = true;
                }
                else
                {
                    leftIsFlipped = true;
                    rightIsFlipped = false;
                }
            }
        }

    }

    private void CheckPlayerVerticalFieldType (Player _player)
    {
        if (_player.transform.position.x < transform.position.x)
        {
            if (_player.PlayerController.IsGravityFlipped != leftIsFlipped)
                _player.ChangeGravity(leftIsFlipped);
        }
        else
        {
            if (_player.PlayerController.IsGravityFlipped != rightIsFlipped)
                _player.ChangeGravity(rightIsFlipped);
        }
    }

    private void CheckPlayerHorizontalFieldType (Player _player)
    {
        if (_player.transform.position.y < transform.position.y)
        {
            if (_player.PlayerController.IsGravityFlipped == false)
                _player.ChangeGravity(true);
        }
        else
        {
            if (_player.PlayerController.IsGravityFlipped == true)
                _player.ChangeGravity(false);
        }
    }

}
