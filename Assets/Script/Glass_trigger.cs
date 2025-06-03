using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


public class Glass_trigger : MonoBehaviour
{
    public string TagFilter = "Player";

    public void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag(TagFilter) && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
        }
        
       
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}