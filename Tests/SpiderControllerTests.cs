using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class SpiderControllerTests
{
    private GameObject spider;
    private SpiderController spiderController;
    private GameObject player;
    private HealthBar healthBar;

    [SetUp]
    public void SetUp()
    {
        // Configurar el jugador
        player = new GameObject();
        player.tag = "Player";
        player.AddComponent<AricController>();
        player.transform.position = Vector3.zero;

        // Configurar la araña
        spider = new GameObject();
        spiderController = spider.AddComponent<SpiderController>();
        spiderController.player = player.transform;

        // Configurar la barra de salud
        GameObject healthBarObject = new GameObject();
        healthBar = healthBarObject.AddComponent<HealthBar>();
        spiderController.healthBar = healthBar;

        // Configurar el Rigidbody y Animation
        spider.AddComponent<Rigidbody>();
        spider.AddComponent<Animation>();
        
        spiderController.maxHealthSpider = 30;
        spiderController.Start(); // Inicializar valores
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(spider);
        Object.Destroy(player);
    }

    [Test]
    public void TakeDamage_ReducesHealth()
    {
        float initialHealth = spiderController.maxHealthSpider;
        float damage = 10f;
        
        spiderController.TakeDamage(damage);
        
        Assert.AreEqual(initialHealth - damage, spiderController.currentHealth);
    }

    [UnityTest]
    public IEnumerator TakeDamage_TriggersDeathAnimation()
    {
        spiderController.TakeDamage(30f);
        
        yield return new WaitForSeconds(0.1f); // Espera para asegurar que la animación inicie
        
        Assert.IsTrue(spider.GetComponent<Animation>().IsPlaying("Death"));
    }

    [UnityTest]
    public IEnumerator TakeDamage_DisablesGameObjectAfterDeath()
    {
        spiderController.TakeDamage(30f);
        
        yield return new WaitForSeconds(spider.GetComponent<Animation>()["Death"].length);
        
        Assert.IsFalse(spider.activeSelf);
    }
}