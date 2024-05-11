using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthFill;  // Referencia al componente Image que muestra la vida actual
    public Text healthText;   // Referencia al componente Text que muestra el texto de la vida

    // Método para configurar la salud en la UI
    public void SetHealth(float currentHealth, float maxHealth)
    {
        healthFill.fillAmount = currentHealth / maxHealth;
        healthText.text = currentHealth.ToString("f0") + "/" + maxHealth.ToString("f0");
    }
}