using System.Collections.Generic;
using UnityEngine;

public class InventoryControl : MonoBehaviour
{
    [Tooltip("Maximum stack space")]
    public int maximumSpace;
    [Tooltip("Cell container")]
    public GameObject cellContainer;
    [Tooltip("Inventory cells sheet")]
    public GameObject inventory;
    [Tooltip("Inventory stack group")]
    public StackGroup inventoryStackGroup;
    [Tooltip("Item inside inventory")]
    public Dictionary<string, int> inventoryItems = new Dictionary<string, int>();

    void Start()
    {
        inventoryItems.Add("Inventory/Resources/res_cloth", 2);
        inventoryItems.Add("Inventory/Resources/res_cotton", 3);
        inventoryItems.Add("Inventory/Recipes/rec_shirt", 1);
        inventoryItems.Add("Inventory/Wearables/wea_shirt", 1);

        // Filling inventory with empty cells
        for (int i = 0; i < maximumSpace; i++)
        {
            Instantiate(Resources.Load("Inventory/cell"), cellContainer.transform);
        }
        // Filling inventory with items
        foreach (KeyValuePair<string, int> entry in inventoryItems)
        {
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
}
