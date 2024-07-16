using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Dev settings bools.
    public bool AllowMovement { get; set; } = true;
    public bool AllowRotation { get; set; } = true;

    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float rotateSpeed = 8f;

    private float currentHorizontalMovement;
    private float currentVerticalMovement;

    void Update()
    {
        if (AllowMovement)
            Move();
        
        if (AllowRotation)
            RotateToMouse();
    }

    private void Move()
    {
        // Get input from keyboard
        currentHorizontalMovement = Input.GetAxisRaw("Horizontal");
        currentVerticalMovement = Input.GetAxisRaw("Vertical");

        // Calculate movement vector
        Vector3 movement = new Vector3(currentHorizontalMovement, 0.0f, currentVerticalMovement);

        // Apply movement to the Rigidbody
        rb.velocity = movement * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    private void RotateToMouse()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;

        Plane playerPlane = new Plane(Vector3.up, transform.position);

        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);

        if (playerPlane.Raycast(ray, out float hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);

            Vector3 lookDir = targetPoint - transform.position;
            lookDir.y = 0; 

            Quaternion targetRotation = Quaternion.LookRotation(lookDir);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }
    }
}
