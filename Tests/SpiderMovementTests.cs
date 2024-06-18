using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class SpiderMovementTests
{
    private GameObject spider;
    private SpiderController spiderController;
    private GameObject player;

    [SetUp]
    public void SetUp()
    {
        // Configurar el jugador
        player = new GameObject();
        player.tag = "Player";
        player.transform.position = new Vector3(0, 0, 10);

        // Configurar la araña
        spider = new GameObject();
        spiderController = spider.AddComponent<SpiderController>();
        spiderController.player = player.transform;

        // Configurar el Rigidbody y Animation
        spider.AddComponent<Rigidbody>();
        spider.AddComponent<Animation>();
        
        spiderController.Start(); // Inicializar valores
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(spider);
        Object.Destroy(player);
    }

    [UnityTest]
    public IEnumerator MoveTowardsPlayer_MovesCloserToPlayer()
    {
        Vector3 initialPosition = spider.transform.position;
        
        spiderController.MoveTowardsPlayer();
        
        yield return null; // Espera un frame
        
        Assert.Less(Vector3.Distance(spider.transform.position, player.transform.position), Vector3.Distance(initialPosition, player.transform.position));
    }

    [UnityTest]
    public IEnumerator AttackPlayer_WithinRangeAndCooldown_AttacksPlayer()
    {
        // Configurar al jugador dentro del rango de ataque
        player.transform.position = spider.transform.position + Vector3.forward * spiderController.attackRange;

        // Asegurar que el ataque esté disponible
        spiderController.lastAttackTime = Time.time - spiderController.attackCooldown;

        spiderController.CheckForPlayer();
        
        yield return null; // Espera un frame
        
        // Verificar si la animación de ataque se ha reproducido
        Assert.IsTrue(spider.GetComponent<Animation>().IsPlaying("Attack"));
    }
}