using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wearable", menuName = "Data/Wearable")]
public class DataWearable : ScriptableObject
{
    [SerializeField] private string displayName; // nombre del artículo llevable que se va a ver en el juego
    [SerializeField] private Texture2D image;

    public string DisplayName { get { return displayName; } }
    public Texture2D Image { get { return image; } }
}
