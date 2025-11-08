using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private List<GameObject> keys = new List<GameObject>();
    [SerializeField] private GameObject exitDoor;

    private int keyCount;

    // Start is called before the first frame update
    void Start()
    {
        keyCount = keys.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if(keyCount == 0)
        {
            Debug.Log("exit door is now open");
        }
    }

    public void KeyCollected(GameObject key)
    {
        keys.Remove(key);
        keyCount--;
    }
}
