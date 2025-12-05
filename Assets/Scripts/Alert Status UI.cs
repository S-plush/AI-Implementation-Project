using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlertStatusUI : MonoBehaviour
{
    [SerializeField] private GameObject statusColorObject;
    [SerializeField] private Image statusColor;
    [SerializeField] private TextMeshProUGUI alertText;

    // Start is called before the first frame update
    void Start()
    {
        statusColor = statusColorObject.GetComponent<Image>();
        statusColor.color = Color.green;
        alertText.text = "Hidden";
    }

    public void HiddenStatus()
    {
        statusColor.color = Color.green;
        alertText.text = "Hidden";
    }

    public void AlertStatus()
    {
        Debug.Log("heres");
        statusColor.color = Color.red;
        alertText.text = "Found";
    }

    public void SearchingStatus()
    {
        statusColor.color = Color.yellow;
        alertText.text = "Searching";
    }
}
