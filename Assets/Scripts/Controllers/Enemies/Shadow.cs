using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shadow : MonoBehaviour
{
    public Transform player;
    public Light mainLight; // La fuente de luz principal del escenario
    public float followSpeed = 2.0f;
    public float attackDelay = 5.0f; // Tiempo en segundos antes de que la sombra ataque
    private bool isInShadow = false;
    private float shadowTimer = 0.0f;

    public float attackDamage = 5;

    private MeshRenderer shadowRenderer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shadowRenderer = GetComponent<MeshRenderer>(); 
        shadowRenderer.enabled = false; 
    }

    void Update()
    {
        if (IsPlayerInShadow())
        {
            FollowPlayer();
            shadowRenderer.enabled = true;

            shadowTimer += Time.deltaTime;

            if (shadowTimer >= attackDelay)
            {
                //AttackPlayer();
                shadowTimer = 0.0f;
            }
        }
        else
        {
            shadowRenderer.enabled = false;
            shadowTimer = 0.0f;
        }
    }

    bool IsPlayerInShadow()
    {
        Vector3 directionToLight = mainLight.transform.position - player.position;
        RaycastHit hit;

        // Realizar un raycast desde la posición del jugador hacia la luz
        if (Physics.Raycast(player.position, directionToLight, out hit))
        {
            // Si el raycast golpea una pared antes de alcanzar la luz, el jugador está en la sombra
            if (hit.collider.CompareTag("Wall"))
            {
                isInShadow = true;
                return true;
            }
        }

        isInShadow = false;
        return isInShadow;
    }

    void FollowPlayer()
    {
        Vector3 targetPosition = player.position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // Calcular la dirección hacia el jugador
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // Mantener el ataque en el mismo plano horizontal
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Rotar la sombra hacia el jugador
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);

        player.GetComponent<AricController>().TakeMentalDamage(attackDamage);
    }
}
