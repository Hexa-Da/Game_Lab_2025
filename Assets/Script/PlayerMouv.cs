using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    private Rigidbody2D rb;
    public float Vitesse = 5f;
    Vector2 mouvement;
    private Animator animator;
    private AudioSource _AudioSource;

    // Update is called once per frame
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _AudioSource = GetComponent<AudioSource>();
    }

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

           if(_AudioSource.isPlaying == false)
            {
                _AudioSource.Play();
            }
        }
    }
    
}