using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour
{
    public int maxSlots;
    public GameObject slotPrefab;
    public GameObject slotsContainer;
    public StackGroup inventoryStackGroup;

    private Dictionary<string, int> inventory;

    void Start()
    {
        this.AddResourcesToInventory
        (
            "Inventory/Resources/res_cloth",
            "Inventory/Resources/res_cloth",
            "Inventory/Resources/res_cotton",
            "Inventory/Resources/res_cotton",
            "Inventory/Resources/res_cotton"
        );

        /*inventoryItems.Add("Inventory/Resources/res_cloth", 2);
        inventoryItems.Add("Inventory/Resources/res_cotton", 3);
        inventoryItems.Add("Inventory/Recipes/rec_shirt", 1);
        inventoryItems.Add("Inventory/Wearables/wea_shirt", 1);*/

        // Crea el inventario con el número de casillas especificado
        for (int i = 1; i <= maxSlots; i++)
        {
            Instantiate(slotPrefab, slotsContainer.transform).name = "Slot " + i;
        }

        // Llena el inventario de objetos
        foreach (KeyValuePair<string, int> entry in inventory)
        {

            // Aquí se debe comprobar el número máximo de recursos agrupables.
            // Si por ejemplo hay 78 unidades de tela y se pueden agrupar máximo en grupos de 30,
            // se deberán agrupar en 3 casillas diferentes así: tela x30, tela x30 y tela x18

            Debug.Log("Load " + entry.Value + " " + entry.Key);
            GameObject gameObject = Resources.Load<GameObject>(entry.Key);
            int a = inventoryStackGroup.AddItemFromPrefab(gameObject.GetComponent<StackItem>(), entry.Value);
            Debug.Log(a);
        }

        // Init existing items's internal state on scene start
        foreach (StackItem stackItem in AccessUtility.FindObjectsOfType<StackItem>())
        {
            stackItem.Init();
        }
    }

    // Función a la que le mandas una lista de
    // recursos y los agrupa en el diccionario.
    public void AddResourcesToInventory(params string[] keys)
    {
        if (this.inventory == null) // Comprueba si el diccionario está inicializado
        { 
            this.inventory = new Dictionary<string, int>(); 
        }
        
        foreach (var k in keys)
        {
            if (!this.inventory.ContainsKey(k)) // Comprueba si ya está registrado el recurso en el diccionario
            {
                this.inventory.Add(k, 0);
            }
            this.inventory[k]++; // Suma un recurso a la casilla de dicho recurso, "agrupándolos"
        }
    }
}
