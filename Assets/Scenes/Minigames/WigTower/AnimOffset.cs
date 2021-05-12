using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimOffset : MonoBehaviour
{
    private MeshRenderer quadRenderer;

    public Vector2 scrollSpeed;

    void Start()
    {
        quadRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        quadRenderer.material.mainTextureOffset += scrollSpeed * Time.deltaTime;
    }
}
