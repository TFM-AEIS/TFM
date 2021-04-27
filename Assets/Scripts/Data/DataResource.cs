using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Data/Resource")]
public class DataResource : ScriptableObject
{
    [SerializeField] private string displayName; // nombre del recurso que se va a ver en el juego
    [SerializeField] private Texture2D image;

    public string DisplayName { get { return displayName; } }
    public Texture2D Image { get { return image; } }
}
