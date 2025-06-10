using System.Diagnostics;
using UnityEngine;

// RattachÃ© au player
// Script qui detecte la possibilitÃ© d'interaction avec les objets
public class CheckInteraction2D : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float playerReach = 10f; 
    [SerializeField] private Transform playerTransform;
    // les objets avec lesquels on peut interagir sont sur un autre layer
    [SerializeField] private LayerMask interactableLayer; 

    private Interaction currentInteractable;
    private NPCInteraction currentNPCInteractable;
    private Vector2 direction;

    void Update()
    {
        // si le jeu est en pause, on ne fait rien
        if(MenuInGame.isPaused) return;

        UpdateDirection(); 
        CheckInteraction();

        if ((currentNPCInteractable != null) && Input.GetKeyDown(KeyCode.F))
        {
            // si on appuie sur F, la fonction Interact() de la classe Interaction est appelÃ©e
            currentNPCInteractable.Interact();
        }

        else if((currentInteractable != null ) && Input.GetKeyDown(KeyCode.F))
        {   
            // si on appuie sur F, la fonction Interact() de la classe Interaction est appelÃ©e
            currentInteractable.Interact();
        }
    }

    void UpdateDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
  
        if (horizontal != 0 && vertical != 0)
        {
            direction = new Vector2(horizontal, vertical).normalized;
        }
        else if (horizontal != 0)
        {
            direction = new Vector2(horizontal, 0).normalized;
        }
        else if (vertical != 0)
        {
            direction = new Vector2(0, vertical).normalized;
        }
       
    }

    void CheckInteraction()
    {
        Vector2[] directions = 
        {
            RotateVector(direction, -50),
            RotateVector(direction, -40),
            RotateVector(direction, -30),
            RotateVector(direction, -20),
            RotateVector(direction, -10),
            direction, 
            RotateVector(direction, 10),
            RotateVector(direction, 20),
            RotateVector(direction, 30),
            RotateVector(direction, 40),
            RotateVector(direction, 50)
        };

        // on lance des raycast dans toutes les directions
        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(playerTransform.position, dir, playerReach, interactableLayer);

            if (hit.collider != null && hit.collider.CompareTag("Interactable"))
            {
                // on rÃ©cupÃ¨re le script Interaction de l'objet avec lequel on peut interagir
                Interaction newInteractable = hit.collider.GetComponent<Interaction>();

                // si on a dÃ©jÃ  un objet avec lequel on peut interagir et que c'est un autre objet, on le dÃ©sactive
                if(currentInteractable && newInteractable != currentInteractable)
                {
                    DisableCurrentInteractable();
                }

                // si l'objet avec lequel on peut interagir est actif, on l'active
                if (newInteractable.enabled)
                {
                    SetNewCurrentInteractable(newInteractable);
                }

                else
                {
                    DisableCurrentInteractable();
                }
            }

            else if (hit.collider != null && hit.collider.CompareTag("NPCInteractable"))
            {
                // on rÃ©cupÃ¨re le script NPCInteraction de l'objet avec lequel on peut interagir
                NPCInteraction newNPCInteractable = hit.collider.GetComponent<NPCInteraction>();

                // si on a dÃ©jÃ  un objet avec lequel on peut interagir et que c'est un autre objet, on le dÃ©sactive
                if (currentNPCInteractable && newNPCInteractable != currentNPCInteractable)
                {
                    DisableCurrentNPCInteractable();
                }

                // si l'objet avec lequel on peut interagir est actif, on l'active
                if (newNPCInteractable.enabled)
                {
                    SetNewCurrentNPCInteractable(newNPCInteractable);
                }
            }

            else
            {
                DisableCurrentNPCInteractable();
                DisableCurrentInteractable();
               
            }
        }
    }

    // pour visualiser les raycast dans la scÃ¨ne
    void OnDrawGizmos()
    {
        Vector2[] directions = 
        {
            RotateVector(direction, -50),
            RotateVector(direction, -40),
            RotateVector(direction, -30),
            RotateVector(direction, -20),
            RotateVector(direction, -10),
            direction, 
            RotateVector(direction, 10),
            RotateVector(direction, 20),
            RotateVector(direction, 30),
            RotateVector(direction, 40),
            RotateVector(direction, 50)
        };

        Gizmos.color = Color.red;
        foreach (Vector2 dir in directions)
        {
            Gizmos.DrawLine(playerTransform.position, playerTransform.position + (Vector3)dir * playerReach);
        }
    }

    Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
            v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
        );
    }

    void SetNewCurrentInteractable(Interaction newInteractable)
    {
        currentInteractable = newInteractable;
        currentInteractable.EnableOutline();

        // Afficher le message au-dessus de l'objet
        HUDController.instance.EnableInteraction(currentInteractable.transform.position, false, newInteractable.isTeleport); //false car jamais NPC
        
    }

    void SetNewCurrentNPCInteractable(NPCInteraction newNPCInteractable)
    {
        currentNPCInteractable = newNPCInteractable;
        currentNPCInteractable.EnableOutline();

        // Afficher le message au-dessus de l'objet
        HUDController.instance.EnableInteraction(currentNPCInteractable.transform.position, true, false); // true car tt le temps NPC et jamais teleport
            
    }
    void DisableCurrentInteractable()
    {
        HUDController.instance.DisableInteraction();
        if (currentInteractable)
        {
            currentInteractable.DisableOutline();
            // on resret la variable currentInteractable pour ne pas avoir d'objet avec lequel on peut interagir
            currentInteractable = null;
        }
    }
    void DisableCurrentNPCInteractable()
    {
        HUDController.instance.DisableInteraction();
        if (currentNPCInteractable)
        {
            currentNPCInteractable.DisableOutline();
            // on resret la variable currentInteractable pour ne pas avoir d'objet avec lequel on peut interagir
            currentNPCInteractable = null;
        }
    }

}
