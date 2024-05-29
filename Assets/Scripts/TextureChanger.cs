using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureChanger : MonoBehaviour
{
    public Texture[] wallTextures;  // The textures for the walls

    // Start is called before the first frame update
    void Start()
    {
        // Obt�n el Renderer del prefab
        Renderer renderer = GetComponent<Renderer>();

        // Change the texture
        renderer.material.mainTexture = wallTextures[Random.Range(0, wallTextures.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}