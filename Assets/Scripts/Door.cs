using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool doorClosed = false;

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
            doorClosed = true;
        }
        else if (doorClosed)
        {
            doorClosed = false;
        }
    }
}
