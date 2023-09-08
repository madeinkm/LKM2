using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eHitType
{
    WallCheck,
    ItemCheck,
}
public class HitBox : MonoBehaviour
{
    private Player player;
    [SerializeField] private eHitType hitType;

    void Start()
    {
        player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.Log("<color=red>Error</color> = player was Null");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.TriggerEnter(hitType, collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player.TriggerExit(hitType, collision);
    }
}
