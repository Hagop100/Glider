using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private Vector2 startPlayerPosition;

    private void Start()
    {
        startPlayerPosition = player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.transform.position = startPlayerPosition;
    }

    public void ResetPlayerPosition(Vector2 checkPointPosition)
    {
        startPlayerPosition = checkPointPosition;
    }
}
