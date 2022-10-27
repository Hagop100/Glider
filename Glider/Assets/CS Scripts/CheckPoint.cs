using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    private DeathZone deathZone;
    private Vector2 checkPointPosition;

    private void Awake()
    {
        deathZone = FindObjectOfType<DeathZone>();
    }

    // Start is called before the first frame update
    void Start()
    {
        checkPointPosition = this.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        deathZone.ResetPlayerPosition(checkPointPosition);
    }
}
