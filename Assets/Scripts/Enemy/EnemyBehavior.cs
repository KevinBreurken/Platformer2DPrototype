using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Enemy Behavior")]
    [SerializeField]
    private ColliderDetectionComponent lightHitDetector;
    [SerializeField]
    private ColliderDetectionComponent darkHitDetector;

    private Vector2 respawnPosition;

    protected virtual void Awake ()
    {
        respawnPosition = transform.position;
        darkHitDetector.OnTriggerEnterDetect += DarkHitDetector_OnTriggerEnterDetect;
        lightHitDetector.OnTriggerEnterDetect += LightHitDetector_OnTriggerEnterDetect;
    }

    public virtual void RespawnEnemy ()
    {
        transform.position = respawnPosition;
        transform.gameObject.SetActive(true);
    }

    public virtual void KillEnemy ()
    {
        transform.gameObject.SetActive(false);
    }

    protected virtual void LightHitDetector_OnTriggerEnterDetect (Collider2D _collider)
    {
        KillEnemy();
    }

    protected virtual void DarkHitDetector_OnTriggerEnterDetect (Collider2D _collider)
    {
        Player player = _collider.gameObject.GetComponent<Player>();
        if (player != null)
            player.KillPlayer();       
    }

}
