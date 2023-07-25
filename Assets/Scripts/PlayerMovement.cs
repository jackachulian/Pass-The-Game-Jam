using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] Rigidbody rb;
    [SerializeField] private LayerMask terrainLayerMask; 

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 right = mainCamera.transform.right;
        Vector3 forward = mainCamera.transform.forward;

        Vector3 movement = right * horizontal + forward * vertical;
        movement = new Vector3(movement.x, 0, movement.z);
        movement = movement.normalized * moveSpeed;

        Vector3 velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        rb.velocity = velocity;


        // Send a raycast from above the player to right above their head.
        // If it hits terrain then player is below the terrain,
        // probably because they placed a mountain on themself.

        if (Physics.Raycast(rb.position + Vector3.up * 22.5f, -transform.up, out RaycastHit hit, 20f, terrainLayerMask))
        {
            // send to point plus half of capsule collider's height (5) because transform is in center of player
            rb.position = hit.point + Vector3.up * 2.5f;
        }
    }
}