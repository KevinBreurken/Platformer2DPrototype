using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBehavior : EnemyBehavior
{
    [Header("Crawler Behavior")]
    [SerializeField]
    private Transform floorTransform;
    [SerializeField]
    private bool isFacingRight;
    [SerializeField]
    private float movementSpeed;

    private float movementDirection;
    private float crawlerSize;
    private float floorMinX;
    private float floorMaxX;

    protected override void Awake ()
    {
        base.Awake();

        crawlerSize = transform.localScale.x / 2;

        //Determine a what position the left and right edges are  
        floorMinX = floorTransform.position.x - (floorTransform.localScale.x / 2) + crawlerSize;
        floorMaxX = floorTransform.position.x + (floorTransform.localScale.x / 2) - crawlerSize;

        SetDirection(isFacingRight);
    }

    protected override void DarkHitDetector_OnTriggerEnterDetect (Collider2D _collider)
    {
        base.DarkHitDetector_OnTriggerEnterDetect(_collider);

        if(_collider.transform.parent.GetComponent<CrawlerBehavior>())
        SetDirection(!isFacingRight);
    }

    void Update ()
    {
        Move();
    }

    private void Move ()
    {
        transform.Translate((transform.right * movementDirection * movementSpeed) * Time.deltaTime);
        CheckForFloorEdge();
    }

    private void CheckForFloorEdge ()
    {
        if (isFacingRight)
        {
            if (transform.position.x >= floorMaxX)
                SetDirection(false);
        }
        else
        {
            if (transform.position.x <= floorMinX)
                SetDirection(true);
        }
    }

    public void SetDirection (bool _facingRight)
    {
        isFacingRight = _facingRight;
        movementDirection = isFacingRight ? 1 : -1;
    }
}
