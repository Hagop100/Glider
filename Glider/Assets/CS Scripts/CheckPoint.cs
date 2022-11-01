using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    private DeathZone deathZone;
    private Vector2 checkPointPosition;
    private SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        deathZone = FindObjectOfType<DeathZone>();
        mySpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer.color = Color.red;
        checkPointPosition = this.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mySpriteRenderer.color = Color.white;
        deathZone.ResetPlayerPosition(checkPointPosition);
    }
}
