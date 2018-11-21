using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;
            }
            return instance;
        }
    }

    [SerializeField]
    private Player[] players;

    [Header("Death Sequence")]
    [SerializeField]
    private float delayBetweenPlayerDeath;
    [SerializeField]
    private float delayBeforeRespawningPlayers;

    private static PlayerManager instance = null;

    void Awake ()
    {
        for (int i = 0; i < players.Length; i++)
            players[i].OnPlayerDeath += PlayerManager_OnPlayerDeath;
    }

    private void PlayerManager_OnPlayerDeath (Player _player)
    {
        _player.gameObject.SetActive(false);
        StartCoroutine("PlayDeathSequence");
    }

    private IEnumerator PlayDeathSequence ()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].gameObject.activeSelf)
                players[i].gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(delayBeforeRespawningPlayers);
        RespawnPlayers();
    }

    private void RespawnPlayers ()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].RespawnPlayer();
        }
    }

}
