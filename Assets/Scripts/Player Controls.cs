using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private float movementSpd;

    private CharacterController charController;
    private float ySpeed;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * verticalInput;

        if (horizontalInput > 0 || horizontalInput < 0)
        {
            this.gameObject.transform.Rotate(new Vector3(0, horizontalInput, 0));
        }

        float magnitude = Mathf.Clamp01(moveDirection.magnitude) * movementSpd;
        moveDirection.Normalize();
        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (charController.isGrounded)
        {
            ySpeed = 0f;
        }

        Vector3 velocity = moveDirection * magnitude;
        velocity = OnSlope(velocity);
        velocity.y += ySpeed;
        charController.Move(velocity * Time.deltaTime);
    }

    private Vector3 OnSlope(Vector3 velocity)
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if(Physics.Raycast(ray, out RaycastHit slopeHit, .4f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, slopeHit.normal);
            var adjustVelocity = slopeRotation * velocity;

            if(adjustVelocity.y < 0)
            {
                return adjustVelocity;
            }
        }

        return velocity;
    }
}
