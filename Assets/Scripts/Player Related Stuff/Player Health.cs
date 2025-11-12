using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Slider healthSlider;

    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider = FindObjectOfType<Slider>();
        healthSlider.value = currentHealth;
    }

    public void TakeDamage()
    {
        //if(currentHealth > 0)
        //{
        //    currentHealth --;
        //    UpdateHealthBar();
        //}
        if (currentHealth == 0)
        {
            Debug.Log("you're dead");
        }

        currentHealth--;
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthSlider.value = currentHealth;
    }
}
