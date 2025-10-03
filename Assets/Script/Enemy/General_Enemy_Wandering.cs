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

    private Vector2 homeLocation;
    private bool isResting;
    public Vector2 target;
    private void Awake()
    {
        homeLocation = transform.parent != null ? (Vector2)transform.parent.position : (Vector2)transform.position;
        rb = GetComponentInParent<Rigidbody2D>(); // from parent gameObject
        anim = GetComponentInParent<Animator>(); // from parent gameObject
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
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, target) < 0.1f )
        {
            StartCoroutine(IdleAndSetNewWanderPoint());
        }
        HandleMovements();
    }

    private void HandleMovements() // update for parent gameObject
    {
        Vector2 direction = (target - (Vector2)transform.parent.position).normalized;

        if (direction.x > 0 && transform.parent.localScale.x > 0)
        {
            transform.parent.localScale = new Vector3(-Mathf.Abs(transform.parent.localScale.x * -1), transform.parent.localScale.y, transform.parent.localScale.z);
        }
        else if (direction.x < 0  && transform.parent.localScale.x < 0)
        {
            transform.parent.localScale = new Vector3(Mathf.Abs(transform.parent.localScale.x), transform.parent.localScale.y, transform.parent.localScale.z);
        }

        rb.linearVelocity = direction * moveSpeed;
    }
    IEnumerator IdleAndSetNewWanderPoint()
    {
        isResting = true;
        anim.Play("Idle");
        rb.linearVelocity = Vector2.zero;
        float restTime = UnityEngine.Random.Range(minRestTime, maxRestTime);
        yield return new WaitForSeconds(restTime);

        target = GetRandomWanderPoint();
        isResting = false;
        anim.Play("Walk");
    }

    public void OnParentCollision(Collision2D collision) // needs to update collision detection
    {
        if (collision.gameObject.CompareTag("Player"))
           Debug.Log((transform.parent != null ? transform.parent.gameObject.name : gameObject.name) + " Collided With Player!");
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log((transform.parent != null ? transform.parent.gameObject.name : gameObject.name) + " Collided With Wall!");
            StopAllCoroutines();// stop the current coroutine to avoid multiple coroutines running at the same time
            StartCoroutine(IdleAndSetNewWanderPoint());
        }
    }
    private Vector2 GetRandomWanderPoint() // the enemy will wander within the circle defined by wanderRadius started off with homeLocation
    { 
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float radius = UnityEngine.Random.Range(0f, wanderRadius); // ensure they will never wander out of the circle
        Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        return homeLocation + randomPoint;

    }
  
    private void OnDrawGizmosSelected()
    {
        //Debug.Log(gameObject.name + " spawnLocation: " + homeLocation);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(homeLocation, wanderRadius); // homeLocation will look weird before running the editor.
    }
}
