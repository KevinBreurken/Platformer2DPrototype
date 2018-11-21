using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderDetectionComponent : MonoBehaviour
{
    public delegate void OnTriggerDetectionEvent (Collider2D _collider);
    public event OnTriggerDetectionEvent OnTriggerEnterDetect = delegate
    { };
    public event OnTriggerDetectionEvent OnTriggerExitDetect = delegate
    { };
    public event OnTriggerDetectionEvent OnTriggerStayDetect = delegate
    { };

    public delegate void OnCollisionDetectionEvent (Collision2D _collision);
    public event OnCollisionDetectionEvent OnCollisionEnterDetect = delegate
    { };
    public event OnCollisionDetectionEvent OnCollisionExitDetect = delegate
    { };
    public event OnCollisionDetectionEvent OnCollisionStayDetect = delegate
    { };

    void Awake ()
    {
        if (GetComponent<Collider2D>() == null)
            Debug.LogWarning("No Collider found on GameObject.", gameObject);        
    }

    private void OnTriggerEnter2D (Collider2D _collider)
    {
        OnTriggerEnterDetect(_collider);
    }

    private void OnTriggerExit2D (Collider2D _collider)
    {
        OnTriggerExitDetect(_collider);
    }

    public void OnTriggerStay2D (Collider2D _collider)
    {
        OnTriggerStayDetect(_collider);
    }

    private void OnCollisionEnter2D (Collision2D _collider)
    {
        OnCollisionEnterDetect(_collider);
    }

    private void OnCollisionExit2D (Collision2D _collider)
    {
        OnCollisionExitDetect(_collider);
    }

    private void OnCollisionStay2D (Collision2D _collider)
    {
        OnCollisionStayDetect(_collider);
    }
}
