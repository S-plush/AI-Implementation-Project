using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    [SerializeField] GameObject exitDoor;

    private ExitDoor updateExitDoor;

    private bool playerInside = false;

    // Start is called before the first frame update
    void Start()
    {
        updateExitDoor = exitDoor.GetComponent<ExitDoor>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInside && Input.GetKey(KeyCode.E))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInside = false;
        }
    }

    private void OnDestroy()
    {
        if (updateExitDoor != null)
        {
            updateExitDoor.KeyCollected(this.gameObject);
        }
    }
}
