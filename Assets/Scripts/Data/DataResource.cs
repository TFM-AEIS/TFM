using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Data/Resource")]
public class DataResource : ScriptableObject
{
    [SerializeField] private string displayName; // nombre del recurso que se va a ver en el juego
}
