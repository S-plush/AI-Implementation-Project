using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoorInteraction : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject button;
    [SerializeField] private float timer;

    private DoorInteraction doorInteraction;
    private Door doorStatus;

    private bool enemyInside = false;

    private float enemyLastInteraction;

    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        doorStatus = door.GetComponent<Door>();
        doorInteraction = button.GetComponent<DoorInteraction>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyInside && doorStatus.IsDoorClosed())
        {
            Debug.Log("am i at least inside");

            if (Time.time - enemyLastInteraction < timer)
            {
                return;
            }

            counter++;
            Debug.Log(counter);
            enemyLastInteraction = Time.time;

            if (counter >= 2)
            {
                counter = 0;
                StartCoroutine(doorInteraction.OpenDoor());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemyInside = true;
            //Debug.Log("does this turn true");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemyInside = false;
            counter = 0;
        }
    }
}
