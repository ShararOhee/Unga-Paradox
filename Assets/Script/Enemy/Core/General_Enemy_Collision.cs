using UnityEngine;

public class General_Enemy_Collision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private General_Enemy_Wandering wanderingScript;
    private General_Enemy_Movement movementScript;

    void Awake()
    {   // this transform.parent is needed to call the sibling script in the same parent gameObject
        wanderingScript = transform.parent.GetComponentInChildren<General_Enemy_Wandering>(); // go to the parent, then actually get the component in the parent's children lmao
        movementScript = transform.parent.GetComponentInChildren<General_Enemy_Movement>();

        if (wanderingScript == null) 
            throw new MissingComponentException("General_Enemy_Detection missing!");
        if (movementScript == null) 
            throw new MissingComponentException("General_Enemy_Movement missing!");

    }

    public void OnParentCollision(Collision2D collision) // needs to update collision detection
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(HelperFuncs.GetOwnerName(transform) + " Collided With Character! Stopping and wandering away.");
            StopAllCoroutines(); // Stop any current wandering coroutine
            movementScript.Stop();                  // 
            StartCoroutine(wanderingScript.IdleAndSetNewWanderPoint());
        }


    }
}
