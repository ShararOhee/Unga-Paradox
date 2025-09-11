using UnityEngine;

/// <summary>
/// Simple enemy script that chases after the player
/// </summary>
public class EnemyChase : MonoBehaviour
{
    [Header("Chase Settings")]
    [Tooltip("The speed at which the enemy moves")]
    public float moveSpeed = 3.0f;
    [Tooltip("The distance at which the enemy stops chasing")]
    public float stopDistance = 1.0f;

    [Header("References")]
    [Tooltip("The player's transform to chase after")]
    public Transform playerTarget;

    [Header("Sprite Settings")]
    [Tooltip("The sprite renderer to flip based on direction")]
    public SpriteRenderer enemySprite;

    void Start()
    {
        // Try to find the player if not assigned
        if (playerTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTarget = player.transform;
            }
        }
    }

    void Update()
    {
        // Check if we have a target and it's active
        if (playerTarget == null || !playerTarget.gameObject.activeInHierarchy)
        {
            return;
        }

        ChasePlayer();
    }

    /// <summary>
    /// Moves the enemy towards the player
    /// </summary>
    private void ChasePlayer()
    {
        // Calculate direction to player
        Vector3 direction = playerTarget.position - transform.position;
        float distance = direction.magnitude;

        // Stop if we're close enough
        if (distance <= stopDistance)
        {
            return;
        }

        // Normalize direction and move
        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;
        if (enemySprite != null)
    {
        // Flip sprite based on X direction
        if (direction.x > 0)
        {
            enemySprite.flipX = true; // Face right
        }
        else if (direction.x < 0)
        {
            enemySprite.flipX = false; // Face left
        }
    }
    }

    /// <summary>
    /// Draw gizmos in the editor to visualize chase range
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw stop distance circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        
        // Draw line to target if assigned
        if (playerTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, playerTarget.position);
        }
    }
}