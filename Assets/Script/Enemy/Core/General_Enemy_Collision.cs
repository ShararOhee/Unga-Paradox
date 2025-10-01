using UnityEngine;

public class General_Enemy_Collision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private General_Enemy_Wandering enemyWandering;

    private void Awake()
    {
        enemyWandering = GetComponent<General_Enemy_Wandering>();
    }

    public void OnParentCollision(Collision2D collision) // needs to update collision detection
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(HelperFuncs.GetOwnerName(transform) + " Collided With Character! Stopping and wandering away.");
            enemyWandering.StopAllCoroutines(); // Stop any current wandering coroutine  
            enemyWandering.StartCoroutine(enemyWandering.IdleAndSetNewWanderPoint());
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(HelperFuncs.GetOwnerName(transform) + " Collided With Player! Stopping and wandering away.");
           
        }
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log(HelperFuncs.GetOwnerName(transform) + " Collided With Wall!");
            StopAllCoroutines();// stop the current coroutine to avoid multiple coroutines running at the same time
            StartCoroutine(enemyWandering.IdleAndSetNewWanderPoint());
        }
    }
}
