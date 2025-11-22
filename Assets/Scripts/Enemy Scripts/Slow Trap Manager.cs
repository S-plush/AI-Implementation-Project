using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTrapManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> slowTraps = new List<GameObject>();

    public void AddToList(GameObject slowTrap)
    {
        slowTraps.Add(slowTrap);
    }

    public void RemoveFromList(GameObject slowTrap)
    {
        slowTraps.Remove(slowTrap);
    }

    public int GetTrapsCount()
    {
        return slowTraps.Count;
    }
}
