using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private List<GameObject> keys = new List<GameObject>();

    [SerializeField] private GameObject exitDoor;
    [SerializeField] private GameObject doorCam;

    [SerializeField] private TextMeshProUGUI keysCollectedText;
    [SerializeField] private Animator doorAnimation;


    private int keyCount;
    private int maxKeyCount;

    // Start is called before the first frame update
    void Start()
    {
        maxKeyCount = keys.Count;
        keysCollectedText.text = "Keys \nCollected: " + keyCount + "/" + maxKeyCount;
        doorAnimation = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(keyCount == maxKeyCount)
        {
            Debug.Log("exit door is now open");
            StartCoroutine(OpenExitDoor());
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

    public IEnumerator OpenExitDoor()
    {
        Time.timeScale = 0.0f;
        doorCam.SetActive(true);
        doorAnimation.Play("Open Exit Door");
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1.0f;
        doorCam.SetActive(false);
    }
}
