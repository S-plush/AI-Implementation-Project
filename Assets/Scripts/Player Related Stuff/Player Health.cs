using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthSliderFill;
    [SerializeField] private Animator gameOver;

    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider = FindObjectOfType<Slider>();
        healthSliderFill = GameObject.FindGameObjectWithTag("Health Bar").GetComponent<Image>();
        healthSlider.value = currentHealth;
    }

    public void TakeDamage()
    {
        currentHealth--;
        UpdateHealthBar();

        if (currentHealth == 0)
        {
            StartCoroutine(GameOver());
        }
    }

    public void UpdateHealthBar()
    {
        healthSlider.value = currentHealth;

        if(healthSlider.value == 3)
        {
            healthSliderFill.color = Color.yellow;
        }
        else if(healthSlider.value == 1)
        {
            healthSliderFill.color = Color.red;
        }
    }

    public IEnumerator GameOver()
    {
        gameOver.Play("Fade In");
        yield return new WaitForSeconds(1.2f);
        Time.timeScale = 0.0f;
    }
}
