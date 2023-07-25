using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] Rigidbody rb;

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
    }
}