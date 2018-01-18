using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimationHandler : MonoBehaviour {

    Animator anim;
    Health ourHealth;
    PlayerMovement movement;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        movement = gameObject.GetComponent<PlayerMovement>();
        ourHealth = GetComponent<Health>();
    }

    // Use this for initialization
    void Update()
    {



        //if (movement.grounded)
        //{
        //    anim.SetBool("Grounded", true);
        //}
        //else if (!movement.grounded)
        //{
        //    anim.SetBool("Grounded", false);
        //}

        //anim.SetFloat("Speed", Mathf.Abs(movement.horizontalSpeed));


        //if (!movement.jumping && anim.GetBool("Jumping"))
        //{
        //    anim.SetBool("Jumping", false);
        //}

        //if (movement.jumpInitiated)
        //{
        //    anim.SetTrigger("JumpInitiated");
        //    anim.SetBool("Jumping", true);
        //    movement.jumpInitiated = false;
        //}





    }

    private void OnEnable()
    {
        ourHealth.Died += this.PlayDeathAnimation;
     //   ourHealth.Respawned -= this.ResetBackToIdleAfterRespawn;
    }
    private void OnDisable()
    {
        ourHealth.Died -= this.PlayDeathAnimation;
       // unit.ourHealth.Respawned -= this.ResetBackToIdleAfterRespawn;
    }

    void ResetBackToIdleAfterRespawn()
    {
        anim.SetTrigger("Respawned");
    }

    void PlayDeathAnimation(GameObject go)
    {
        anim.SetTrigger("Dead");
    }
}
