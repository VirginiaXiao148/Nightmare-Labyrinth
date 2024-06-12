using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;

public class AricControllerMovementTests
{
    private GameObject gameObject;
    private AricController aricController;
    private Camera camera;

    [SetUp]
    public void SetUp()
    {
        // Crear un nuevo GameObject y añadir el componente AricController
        gameObject = new GameObject();
        aricController = gameObject.AddComponent<AricController>();

        // Crear una cámara
        camera = new GameObject().AddComponent<Camera>();
        aricController.mainCamera = camera;

        // Inicializar el CharacterController
        gameObject.AddComponent<CharacterController>();
        aricController.Start(); // Inicializar valores
    }

    [TearDown]
    public void TearDown()
    {
        // Limpiar objetos de prueba
        Object.Destroy(gameObject);
        Object.Destroy(camera.gameObject);
    }

    [UnityTest]
    public IEnumerator HandleMovement_MovesCharacter()
    {
        float initialPositionX = gameObject.transform.position.x;

        // Simular la entrada del usuario
        Input.GetAxis("Vertical");
        aricController.Update();

        yield return null;

        // Comprobar si el objeto se ha movido
        Assert.AreNotEqual(initialPositionX, gameObject.transform.position.x);
    }

    [UnityTest]
    public IEnumerator HandleRotation_RotatesCharacter()
    {
        Quaternion initialRotation = gameObject.transform.rotation;

        // Simular la entrada del usuario
        aricController.HandleRotation();
        yield return null;

        // Comprobar si el objeto se ha rotado
        Assert.AreNotEqual(initialRotation, gameObject.transform.rotation);
    }
}