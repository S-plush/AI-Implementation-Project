using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private List<GameObject> keys = new List<GameObject>();
    [SerializeField] private GameObject exitDoor;
    [SerializeField] private TextMeshProUGUI keysCollectedText;

    private int keyCount;
    private int maxKeyCount;

    // Start is called before the first frame update
    void Start()
    {
        maxKeyCount = keys.Count;
        keysCollectedText.text = "Keys \nCollected: " + keyCount + "/" + maxKeyCount;
    }

    // Update is called once per frame
    void Update()
    {
        if(keyCount == maxKeyCount)
        {
            Debug.Log("exit door is now open");
        }
    }

    public void KeyCollected(GameObject key)
    {
        keys.Remove(key);
        keyCount++;
        UpdateKeyCollectedCount();
    }

    public void UpdateKeyCollectedCount()
    {
        keysCollectedText.text = "Keys \nCollected: " + keyCount + "/" + maxKeyCount;
    }
}
