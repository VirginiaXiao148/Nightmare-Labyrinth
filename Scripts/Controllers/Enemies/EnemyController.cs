using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public int enemyHealth = 0;
    int enemyCurrentHealth;

    // Start is called before the first frame update
    void Start()
    {
        enemyCurrentHealth = enemyHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageAmount)
    {
        // Reduce health by damage amount
        enemyCurrentHealth -= damageAmount;

        // Check if the player is dead
        if (enemyCurrentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle player death
    void Die()
    {
        // Add code here to handle player death, such as restarting the level or displaying a game over screen
        Debug.Log("Enemy died!");
    }

}
