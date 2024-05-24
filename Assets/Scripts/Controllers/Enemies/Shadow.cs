using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public Transform player;
    public Light mainLight;
    public float followSpeed = 2.0f;
    public float attackDelay = 5.0f;

    private float shadowTimer = 0.0f;
    public float attackDamage = 5;
    public float shadowDistance = 2.0f;

    private MeshRenderer shadowRenderer;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        shadowRenderer = GetComponent<MeshRenderer>();
        shadowRenderer.enabled = false;
    }

    void Update()
    {
        if (IsPlayerInShadow())
        {
            PlaceShadowBehindPlayer();
            shadowRenderer.enabled = true;

            shadowTimer += Time.deltaTime;

            if (shadowTimer >= attackDelay)
            {
                AttackPlayer();
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
                return true;
            }
        }

        return false;
    }

    void PlaceShadowBehindPlayer()
    {
        // Calcular la posición detrás del jugador
        Vector3 directionBehindPlayer = -player.forward;
        Vector3 targetPosition = player.position + directionBehindPlayer * shadowDistance;

        // Mover la sombra suavemente a la posición calculada
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

        // Realizar el ataque
        Debug.Log("Sombra atacando al jugador");

        // Implementar lógica de ataque, por ejemplo, aplicar daño al jugador
        AricController playerController = player.GetComponent<AricController>();
        if (playerController != null)
        {
            playerController.TakeMentalDamage(attackDamage);
        }
    }
}
