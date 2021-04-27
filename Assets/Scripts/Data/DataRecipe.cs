using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Data/Recipe")]
public class DataRecipe : ScriptableObject
{
    [System.Serializable]
    struct Requisite
    {
        public DataResource type; // tipos de recursos necesarios para la receta
        public int quantity; // cantidad necesaria de cada tipo de recursos
    }

    [SerializeField] private string displayName; // nombre de la receta que se va a ver en el juego
    [SerializeField] private Requisite[] requisites;
    [SerializeField] private DataWearable reward; // recompensa por completar la receta

    //-----------------------------------------------------------//
    // Properties
    //-----------------------------------------------------------//

    public string DisplayName { get { return displayName; } }

    public Dictionary<DataResource, int> Requisites
    {
        get 
        {
            Dictionary<DataResource, int> requisites = new Dictionary<DataResource, int>();

            for (int i = 0; i < this.requisites.Length; i++)
            {
                requisites.Add(this.requisites[i].type, this.requisites[i].quantity);
            }

            return requisites;
        }
    }

    public DataWearable Reward { get { return reward; } }
}
