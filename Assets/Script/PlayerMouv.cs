using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public Rigidbody2D rb;
    public float Vitesse = 5f;
    Vector2 mouvement;
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if(!MenuInGame.isPaused)
        {
            mouvement.x = Input.GetAxisRaw("Horizontal");
            mouvement.y = Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", mouvement.x);
            animator.SetFloat("Vertical", mouvement.y);
            animator.SetFloat("Speed", mouvement.magnitude);
            mouvement.Normalize();
            rb.linearVelocity = mouvement * Vitesse;

           //  rb.MovePosition(rb.position + mouvement * Vitess * Time.deltaTime);
        }
    }
}