using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private EnemyAI enemy;
    private PlayerHealth player;

    // Start is called before the first frame update
    void Start()
    {
        enemy = FindAnyObjectByType<EnemyAI>();
        player = FindAnyObjectByType<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.TakeDamage();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
