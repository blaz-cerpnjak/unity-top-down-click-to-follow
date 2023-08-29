using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Update the speed parameter of the animator for the blend tree
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);

        if (Input.mousePresent)
        {
            HandleMouseClick();
        }
        else if (Input.touchSupported)
        {
            HandleTouch();
        }
    }

    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MovePlayerToPosition(Input.mousePosition);
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                MovePlayerToPosition(touch.position);
            }
        }
    }

    private void MovePlayerToPosition(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the touched point is on the ground
            // Add "Ground" Tag to your ground object
            if (hit.collider.CompareTag("Ground"))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }

}
