using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private Vector3 closedDoorPosition;
    [SerializeField] private Vector3 openDoorPosition;

    private Door doorStatus;

    private bool playerInside = false;

    private void Start()
    {
        doorStatus = door.GetComponent<Door>();
    }

    // Update is called once per frame
    void Update()
    {
        if (doorStatus.IsDoorClosed() && playerInside && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(OpenDoor());
        }
        else if (!doorStatus.IsDoorClosed() && playerInside && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(CloseDoor());
        }
    }

    public IEnumerator CloseDoor()
    {
        doorStatus.OpenDoor();
        Vector3 targetPosition = door.transform.position + closedDoorPosition;
        Vector3 startingPosition = door.transform.position;
        float movingTime = .5f;
        float elapsedTime = 0f;

        while (elapsedTime <= movingTime)
        {
            float openingProgress = elapsedTime / movingTime;
            door.transform.position = Vector3.Lerp(startingPosition, targetPosition, openingProgress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(5f);
    }

    public IEnumerator OpenDoor()
    {
        doorStatus.OpenDoor();
        Vector3 targetPosition = door.transform.position + openDoorPosition;
        Vector3 startingPosition = door.transform.position;
        float movingTime = .5f;
        float elapsedTime = 0f;

        while(elapsedTime <= movingTime)
        {
            float openingProgress = elapsedTime / movingTime;
            door.transform.position = Vector3.Lerp(startingPosition, targetPosition, openingProgress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
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
}
