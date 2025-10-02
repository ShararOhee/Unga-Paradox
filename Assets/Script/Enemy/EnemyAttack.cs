using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            var healthController = collision.gameObject.GetComponent<Health>();
            
            healthController.TakeDamage(damage);
        }
    }
}
