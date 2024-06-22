using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [SerializeField] float shootingSpeed =10;
    [SerializeField] float arrowExistence = 1;
    
    Rigidbody2D arrowRigidbody;
    PlayerMovement player;
    float direction;
    
    void Start()
    {
        StartCoroutine(SelfDestruct());
        arrowRigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        direction = player.transform.localScale.x;
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSecondsRealtime(arrowExistence);
        Destroy(gameObject);
    }

    void Update()
    {
        arrowRigidbody.velocity = new Vector2(shootingSpeed * direction, 0f);
        transform.localScale = new Vector2(direction, transform.localScale.y);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }
}
