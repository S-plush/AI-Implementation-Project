using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private Animator winScreen;
    [SerializeField] private GameObject victoryCamera;

    private bool hasExited = false;

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
        if(other.gameObject.tag == "Player")
        {
            hasExited = true;
            StartCoroutine(VictoryAnimation());
        }
    }

    public IEnumerator VictoryAnimation()
    {
        winScreen.Play("Victory");
        victoryCamera.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0f;
    }

    public bool HasExited()
    {
        return hasExited;
    }
}
