﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip coinSound;
    [Range(0.0f, 5.0f)] [SerializeField] private float coinSoundVolume;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(coinSound, this.transform.position, coinSoundVolume);
            Destroy(this.gameObject);
        }
    }
}
