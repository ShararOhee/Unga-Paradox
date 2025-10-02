using UnityEngine;

// this script is to be placed on the parent gameObject of the enemy, to relay all events to the child scripts, and manage them in here.
public class General_Enemy_Mechanics_Parent : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private General_Enemy_PathFinding pathFinder;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var childDetection = GetComponentInChildren<General_Enemy_Detection>();
        //var childWandering = GetComponentInChildren<General_Enemy_Wandering>();

        if (childDetection != null && childDetection.hasLineOfSight)
        {
            // proceed to chase the mf
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        var childWander = GetComponentInChildren<General_Enemy_Wandering>();
        if (childWander != null)
            childWander.OnParentCollision(collision);
    }
}
