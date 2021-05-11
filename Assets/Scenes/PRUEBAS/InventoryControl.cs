using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class InventoryControl : MonoBehaviour
{
    public int maxSlots;
    public GameObject slotPrefab;
    public GameObject slotsContainer;
    public StackGroup inventoryStackGroup;

    [SerializeField] private Text scoreText;
    private int score = 10000;
    private Dictionary<string, int> inventory;

    NumberFormatInfo numberFormat = new CultureInfo("es-ES", false).NumberFormat;

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
            Debug.Log(this.AddResource(entry.Key, entry.Value));
        }

        // Init existing items's internal state on scene start
        foreach (StackItem stackItem in AccessUtility.FindObjectsOfType<StackItem>())
        {
            stackItem.Init();
        }

        this.AddRecipe("Inventory/Recipes/rec_shirt");
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

    private int AddResource(string resource, int count)
    {
        Debug.Log("ADD resource: " + count + " " + resource);
        GameObject gameObject = Resources.Load<GameObject>(resource);
        return inventoryStackGroup.AddItemFromPrefab(gameObject.GetComponent<StackItem>(), count);
    }

    private int AddRecipe(string recipe)
    {
        Debug.Log("ADD recipe:" + recipe);
        GameObject gameObject = Resources.Load<GameObject>(recipe);
        return inventoryStackGroup.AddItemFromPrefab(gameObject.GetComponent<StackItem>(), 1);
    }

    private int AddWearable(string recipe)
    {
        Debug.Log("ADD wearable:" + recipe);
        GameObject gameObject = Resources.Load<GameObject>(recipe);
        gameObject.GetComponent<Wearable>().score = this.score;
        return inventoryStackGroup.AddItemFromPrefab(gameObject.GetComponent<StackItem>(), 1);
    }

    // Botón "Recursos" para testing
    public void CheatResources()
    {
        this.AddResourcesToInventory(
            "Inventory/Resources/res_cloth",
            "Inventory/Resources/res_cloth",
            "Inventory/Resources/res_cotton",
            "Inventory/Resources/res_cotton"
        );
        this.AddResource("Inventory/Resources/res_cloth", 2);
        this.AddResource("Inventory/Resources/res_cotton", 2);
    }

    // Botón "Play" para testing
    public void Play()
    {
        this.score = Random.Range(1, 50000);
        this.scoreText.text = this.score.ToString("N0", this.numberFormat);
    }

    public void OnItemUse(GameObject item)
    {
        Debug.Log("Item used:");
        Debug.Log(item.GetComponent<Recipe>().itemResult);
        Recipe recipe = item.GetComponent<Recipe>();
        string ingredient1Name = recipe.ingredient1Name;
        int ingredient1Count = recipe.ingredient1Count;
        string ingredient2Name = recipe.ingredient2Name;
        int ingredient2Count = recipe.ingredient2Count;

        this.inventory.TryGetValue(ingredient1Name, out int ingredient1Owned);
        Debug.Log("Ingredient1 owned: " + ingredient1Owned + ". Needed: " + ingredient1Count);
        if (ingredient1Owned < ingredient1Count)
        {
            return;
        }

        this.inventory.TryGetValue(ingredient2Name, out int ingredient2Owned);
        Debug.Log("Ingredient2 owned: " + ingredient2Owned + ". Needed: " + ingredient2Count);
        if (ingredient2Owned < ingredient2Count)
        {
            return;
        }
        this.AddWearable(item.GetComponent<Recipe>().itemResult);
    }
}
