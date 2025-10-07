using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private ICollectable _collectable;

    private void Awake()
    {
        _collectable = GetComponent<ICollectable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerController>();

        if (player != null)
        {
            _collectable.OnCollected(player.gameObject);
            Destroy(gameObject);
        }
    }
}
