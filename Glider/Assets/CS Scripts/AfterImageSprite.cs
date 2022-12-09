using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageSprite : MonoBehaviour
{
    private Transform player;

    private SpriteRenderer mySR;
    private SpriteRenderer playerSR;

    private Color color;

    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    private float alphaSet = 0.8f;
    private float alphaMultiplier = 0.85f;

    private void OnEnable()
    {
        mySR = this.GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        mySR.sprite = playerSR.sprite;
        mySR.flipX = playerSR.flipX;
        this.transform.position = player.position;
        this.transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(0f, 1f, 1f, alpha);
        mySR.color = color;

        if(Time.time >= (timeActivated + activeTime))
        {
            AfterImageEffect.instance.AddToPool(this.gameObject);
            
        }
    }
}
