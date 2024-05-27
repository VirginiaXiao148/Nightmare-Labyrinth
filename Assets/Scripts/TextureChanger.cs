using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public Texture newTexture; // Asigna esta variable en el inspector con la nueva textura

    //public Texture[] wallTextures;  // The textures for the walls

    // Start is called before the first frame update
    void Start()
    {
        // Obtén el Renderer del prefab
        Renderer renderer = GetComponent<Renderer>();

        // Cambia la textura
        renderer.material.mainTexture = newTexture;
        //renderer.material.mainTexture = wallTextures[Random.Range(0, wallTextures.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}