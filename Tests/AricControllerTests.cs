using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

public class AricControllerTests
{
    private GameObject gameObject;
    private AricController aricController;
    private GameObject healthBarObject;
    private Image healthBar;
    private Text healthText;

    [SetUp]
    public void SetUp()
    {
        // Crear un nuevo GameObject y añadir el componente AricController
        gameObject = new GameObject();
        aricController = gameObject.AddComponent<AricController>();
        
        // Configurar la barra de salud y el texto
        healthBarObject = new GameObject();
        healthBar = healthBarObject.AddComponent<Image>();
        aricController.healthBar = healthBar;

        GameObject healthTextObject = new GameObject();
        healthText = healthTextObject.AddComponent<Text>();
        aricController.healthText = healthText;

        aricController.maxHealth = 100;
        aricController.Start(); // Inicializar valores
    }

    [TearDown]
    public void TearDown()
    {
        // Limpiar objetos de prueba
        Object.Destroy(gameObject);
        Object.Destroy(healthBarObject);
        Object.Destroy(healthText.gameObject);
    }

    [Test]
    public void TakeDamage_ReducesHealthCorrectly()
    {
        aricController.TakeDamage(20);
        Assert.AreEqual(80, aricController.currentHealth);
        Assert.AreEqual("80", aricController.healthText.text);
    }

    [Test]
    public void TakeDamage_TriggersDieWhenHealthReachesZero()
    {
        aricController.TakeDamage(100);
        Assert.AreEqual(0, aricController.currentHealth);
        Assert.IsFalse(aricController.gameObject.activeSelf);
    }
}