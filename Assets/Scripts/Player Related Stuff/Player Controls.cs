using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private float movementSpd;
    [SerializeField] private GameObject holdEPopup;
    [SerializeField] private GameObject pressEPopup;

    private CharacterController charController;
    private PlayerHealth playerHealth;
    private ExitTrigger exitTrigger;
    private float ySpeed;

    public float rotateSpeed;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        playerHealth = GetComponent<PlayerHealth>();
        exitTrigger = FindAnyObjectByType<ExitTrigger>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth.GetCurrentHealth() != 0 && !exitTrigger.HasExited()) 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Title Screen");
            }

            Movement();
        }
    }

    public void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * verticalInput;

        if (horizontalInput > 0 || horizontalInput < 0)
        {
            this.gameObject.transform.Rotate(new Vector3(0, horizontalInput * rotateSpeed * Time.deltaTime, 0));
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

    public void ChangeMovementSpd(float newSpd)
    {
        movementSpd = newSpd;
    }

    public void ShowHoldE()
    {
        holdEPopup.SetActive(true);
    }

    public void HideHoldE()
    {
        holdEPopup.SetActive(false);
    }

    public void ShowPressE()
    {
        pressEPopup.SetActive(true);
    }

    public void HidePressE()
    {
        pressEPopup.SetActive(false);
    }
}
