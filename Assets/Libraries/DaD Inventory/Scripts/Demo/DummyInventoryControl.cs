using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Dummy inventory control for demo scene.
/// </summary>
public class DummyInventoryControl : MonoBehaviour
{
	[Tooltip("Equipments cells sheet")]
	public GameObject equipment;
	[Tooltip("Inventory cells sheet")]
	public GameObject inventory;
	[Tooltip("Skills cells sheet")]
	public GameObject skills;
	[Tooltip("Vendor cells sheet")]
	public GameObject vendor;
	[Tooltip("Inventory stack group")]
	public StackGroup inventoryStackGroup;
	[Tooltip("Equipment stack group")]
	public StackGroup equipmentStackGroup;
	[Tooltip("Vendor stack group")]
	public StackGroup vendorStackGroup;
	[Tooltip("Inventory's click to equip group")]
	public ClickEquipGroup clickEquipInventory;

	private PriceGroup priceGroup;											// Player's price group

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		// Init existing items's internal state on scene start
		foreach (StackItem stackItem in AccessUtility.FindObjectsOfType<StackItem>())
		{
			stackItem.Init();
		}

		priceGroup = GetComponent<PriceGroup>();
		Debug.Assert(equipment && inventory && vendor && skills && inventoryStackGroup && equipmentStackGroup && vendorStackGroup && clickEquipInventory && priceGroup, "Wrong settings");
		priceGroup.ShowPrices(vendor.activeSelf);
	}

	/// <summary>
	/// Show/Hide the equipments.
	/// </summary>
	public void ToggleEquipment(Toggle toggle)
	{
		CloseAllSheets();
		if (toggle.isOn == true)
		{
			equipment.SetActive(true);
			inventory.SetActive(true);
		}
	}

	/// <summary>
	/// Show/Hide the skills.
	/// </summary>
	public void ToggleSkills(Toggle toggle)
	{
		CloseAllSheets();
		if (toggle.isOn == true)
		{
			skills.SetActive(true);
		}
	}

	/// <summary>
	/// Show/Hide the vendor.
	/// </summary>
	public void ToggleVendor(Toggle toggle)
	{
		CloseAllSheets();
		if (toggle.isOn == true)
		{
			inventory.SetActive(true);
			vendor.SetActive(true);
			priceGroup.ShowPrices(true);
			// Sell items on double click
			clickEquipInventory.destinationGroup = vendorStackGroup;
		}
		else
		{
			// Equip items on double click
			clickEquipInventory.destinationGroup = equipmentStackGroup;
		}
	}

	/// <summary>
	/// Closes all sheets.
	/// </summary>
	private void CloseAllSheets()
	{
		equipment.SetActive(false);
		inventory.SetActive(false);
		skills.SetActive(false);
		vendor.SetActive(false);
		priceGroup.ShowPrices(false);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		// On click
		if (Input.GetMouseButtonDown(0) == true)
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			pointerEventData.position = Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerEventData, results);
			// If clicked not on UI
			if (results.Count <= 0)
			{
				DummyItemPickUp dummyItemPickUp = null;
				// Raycast to colliders2d
				RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward);
				if (hit2D.collider != null)
				{
					dummyItemPickUp = hit2D.collider.gameObject.GetComponent<DummyItemPickUp>();
				}
				if (dummyItemPickUp == null)
				{
					// Raycast to colliders3d
					RaycastHit[] hits3D = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition));
					if (hits3D.Length > 0)
					{
						foreach (RaycastHit hit in hits3D)
						{
							dummyItemPickUp = hit.collider.gameObject.GetComponent<DummyItemPickUp>();
							if (dummyItemPickUp != null)
							{
								break;
							}
						}
					}
				}
				if (dummyItemPickUp != null)
				{
					// Hitted on DummyItemPickUp item
					if (dummyItemPickUp.itemPrefab != null)
					{
						// Check allowed space in inventory for this item
						int pickUpAmount = inventoryStackGroup.GetAllowedSpace(dummyItemPickUp.itemPrefab);
						pickUpAmount = Mathf.Min(pickUpAmount, dummyItemPickUp.stack);
						// Try to place item into inventory
						dummyItemPickUp.stack -= inventoryStackGroup.AddItemFromPrefab(dummyItemPickUp.itemPrefab, pickUpAmount);
						if (dummyItemPickUp.stack <= 0)
						{
							Destroy(dummyItemPickUp.gameObject);
						}
						// Set item's price using current sell modifier
						priceGroup.UpdatePrices();
						// Show item price if vendor is active
						priceGroup.ShowPrices(vendor.activeSelf);
					}
				}
			}
		}
	}
}
