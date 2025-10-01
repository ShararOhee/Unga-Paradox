using UnityEngine;
using System.Collections;
using System;
using UnityEditor.Experimental.GraphView;
public class General_Enemy_Wandering : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Wandering Settings")]
    [Tooltip("Movement speed of the NPC.")]
    public float moveSpeed = 2f;

    [Tooltip("Radius for wandering area.")] 
    public float wanderRadius = 5f;

    [Tooltip("Minimum Rest Time between Patrol Points")]
    public float minRestTime = 0.5f;

    [Tooltip("Maximum Rest Time between Patrol Points")]
    public float maxRestTime = 2f;

    private Rigidbody2D rb;
    private Animator anim;
    private General_Enemy_Movement enemyMovement;

    private Vector2 homeLocation;
    private bool isResting;
    public Vector2 target;
    
    // for debugging purposes
    private Vector2 lastSelectedWanderPoint;

    private void Awake()
    {
        homeLocation = transform.parent != null ? (Vector2)transform.parent.position : (Vector2)transform.position;
        rb = GetComponentInParent<Rigidbody2D>(); // from parent gameObject
        anim = GetComponentInParent<Animator>(); // from parent gameObject
        enemyMovement = gameObject.AddComponent<General_Enemy_Movement>();
        enemyMovement.Initialized(rb, transform.parent);
        
    }
    private void OnEnable()
    {
        StartCoroutine(IdleAndSetNewWanderPoint());
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isResting)
        {
            enemyMovement.Stop();
            //anim.Play("Idle");
            return;
        }

        if (Vector2.Distance(transform.parent.position, target) < 0.1f)
        {
            enemyMovement.Stop();
            //anim.Play("Idle");
            StartCoroutine(IdleAndSetNewWanderPoint());
            return;
        }

        enemyMovement.MoveTowards(target, moveSpeed);
        anim.Play("Walk");
    }
    public IEnumerator IdleAndSetNewWanderPoint()
    {
        isResting = true;
        anim.Play("Idle");
        rb.linearVelocity = Vector2.zero;
        float restTime = UnityEngine.Random.Range(minRestTime, maxRestTime);
        yield return new WaitForSeconds(restTime);

        target = GetRandomWanderPoint();
        Debug.Log(HelperFuncs.GetOwnerName(transform) + " New Wander Point Set: " + target);
        
        isResting = false;
        anim.Play("Walk");
    }

    private Vector2 GetRandomWanderPoint() // the enemy will wander within the circle defined by wanderRadius started off with homeLocation
    {
        int maxAttempts = 4; // limit attempts to optimize, if no valid point, goes back to base
        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
            float radius = UnityEngine.Random.Range(0f, wanderRadius); // ensure they will never wander out of the circle
            Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            
            Vector2 potentialPoint = homeLocation + randomPoint;
            lastSelectedWanderPoint = potentialPoint; // for debugging purposes
            Collider2D hit = Physics2D.OverlapCircle(homeLocation + randomPoint, 0.2f, LayerMask.GetMask("Unwalkable")); // check if this dumbass chooses a wall to walk to again
            RaycastHit2D lineHit = Physics2D.Linecast(transform.parent.position, potentialPoint, LayerMask.GetMask("Unwalkable"));

            if (hit == null && lineHit == false)
            {
                return potentialPoint;
            }
            else
            {
                Debug.Log(HelperFuncs.GetOwnerName(transform) +
                    " Block Path: " + potentialPoint);
            }
        }
        return homeLocation; // if no valid point found, return to home location

    }
  
    private void OnDrawGizmosSelected()
    {
        //Debug.Log(gameObject.name + " spawnLocation: " + homeLocation);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(homeLocation, wanderRadius); // homeLocation will look weird before running the editor.

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(lastSelectedWanderPoint, 0.1f);
    }

    // misc shits
    
}
