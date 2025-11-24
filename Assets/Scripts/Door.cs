using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //public ClosedDoorManager closedDoorManager;
    [SerializeField] private GameObject doorAccess1;
    [SerializeField] private GameObject doorAccess2;

    private bool doorClosed = false;
    private bool beginning = true;
    private bool beginning2 = true;

    private void Start()
    {
        //closedDoorManager = FindAnyObjectByType<ClosedDoorManager>();
    }

    public bool IsDoorClosed()
    {
        if (doorClosed)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OpenDoor()
    {
        if (!doorClosed)
        {
            if (!beginning)
            {
                doorAccess1.SetActive(true);
                doorAccess2.SetActive(true);
            }

            beginning = false;
            doorClosed = true;
        }
        else if (doorClosed)
        {
            if (!beginning2)
            {
                doorAccess1.SetActive(false);
                doorAccess2.SetActive(false);
            }

            beginning2 = false;
            doorClosed = false;
        }
    }
}
