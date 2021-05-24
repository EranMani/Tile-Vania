using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        FindObjectOfType<GameSession>().AddScore(20);
        AudioSource.PlayClipAtPoint(coinSound, Camera.main.transform.position);
        Destroy(this.gameObject);
    }
}
