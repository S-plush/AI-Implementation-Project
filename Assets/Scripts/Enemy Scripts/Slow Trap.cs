using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlowTrap : MonoBehaviour
{
    [SerializeField] private Image fillCircle;

    private PlayerControls player;
    private SlowTrapManager slowTrapManager;

    private float holdDuration = 1.0f;
    private float holdTimer = 0f;

    private bool playerInside = false;
    private bool isHolding = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<PlayerControls>();
        fillCircle = GameObject.FindGameObjectWithTag("Trap Fill").GetComponent<Image>();
        slowTrapManager = FindAnyObjectByType<SlowTrapManager>();
        slowTrapManager.AddToList(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            isHolding = true;
            holdTimer = 0f;
        }
        else if(!playerInside || Input.GetKeyUp(KeyCode.E))
        {
            ResetHold();
        }

        if (isHolding)
        {
            player.HideHoldE();
            holdTimer += Time.deltaTime;
            fillCircle.fillAmount = holdTimer / holdDuration;

            if(holdTimer >= holdDuration)
            {
                ResetHold();
                slowTrapManager.RemoveFromList(gameObject);
                player.ChangeMovementSpd(5f);
                player.HideHoldE();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = true;
            player.ShowHoldE();
            player.ChangeMovementSpd(2.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = false;
            player.HideHoldE();
            player.ChangeMovementSpd(5f);
        }
    }

    public void ResetHold()
    {
        isHolding = false;

        if (playerInside)
        {
            player.ShowHoldE();
        }

        holdTimer = 0f;
        fillCircle.fillAmount = 0f;
    }
}
