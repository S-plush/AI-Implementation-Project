using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraViewChange : MonoBehaviour
{
    [SerializeField] private GameObject mCamera;
    [SerializeField] private GameObject tpCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TPView")
        {
            tpCamera.SetActive(true);
            mCamera.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        tpCamera.SetActive(false);
        mCamera.SetActive(true);
    }
}
