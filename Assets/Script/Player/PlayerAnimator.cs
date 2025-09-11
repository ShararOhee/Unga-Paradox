using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles interactions with the animator component of the player
/// It reads the player's state from the controller and animates accordingly
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The player controller script to read state information from")]
    public PlayerController playerController;
    [Tooltip("The animator component that controls the player's animations")]
    public Animator animator;

    void Start()
    {
        ReadPlayerStateAndAnimate();
    }

    void Update()
    {
        ReadPlayerStateAndAnimate();
    }

    void ReadPlayerStateAndAnimate()
    {
        if (animator == null || playerController == null)
        {
            return;
        }
        animator.SetBool("isIdle", playerController.state == PlayerController.PlayerState.Idle);
        animator.SetBool("isRunning", playerController.state == PlayerController.PlayerState.Walk);
        animator.SetBool("isDead", playerController.state == PlayerController.PlayerState.Dead);
        
    }
}