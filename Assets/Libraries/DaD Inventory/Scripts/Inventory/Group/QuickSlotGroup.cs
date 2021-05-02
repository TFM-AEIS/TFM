using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls the behavior of quick slot items.
/// </summary>
public class QuickSlotGroup : MonoBehaviour
{
	[Tooltip("Only items and skills from this groups will be permitted to use in quick slots")]
	public List<GameObject> permittedSources = new List<GameObject>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		foreach (GameObject source in permittedSources)
		{
			StackGroup stackGroup = AccessUtility.GetComponentInParent<StackGroup>(source.transform);
			if (stackGroup == null)
			{
				stackGroup = source.GetComponentInChildren<StackGroup>(true);
			}
			if (stackGroup != null && stackGroup.eventAdditionalReceivers.Contains(gameObject) == false)
			{
				// Add quick slot group as stack events receiver
				stackGroup.eventAdditionalReceivers.Add(gameObject);
			}
		}
	}

	/// <summary>
	/// Raises the DaD group event.
	/// </summary>
	/// <param name="desc">Desc.</param>
	public void OnDadGroupEvent(DadCell.DadEventDescriptor desc)
	{
		switch (desc.triggerType)
		{
		case DadCell.TriggerType.DropEnd:
			// Remove unresolved items after any DaD event
			RemoveUnresolvedItems();
			break;
		}
	}

	/// <summary>
	/// Raises the stack group event.
	/// </summary>
	public void OnStackGroupEvent(StackGroup.StackGroupEventDescriptor desc)
	{
		// Remove unresolved items after any stack group event
		RemoveUnresolvedItems();
	}

	/// <summary>
	/// Removes quick slot items if they have no source or source is not in permitted groups
	/// </summary>
	public void RemoveUnresolvedItems()
	{
		foreach (QuickItem quickItem in GetComponentsInChildren<QuickItem>(true))
		{
			bool hit = false;
			// Chek if item source is child of this GO
			foreach (Transform parentTransform in AccessUtility.GetComponentsInParent<Transform>(quickItem.itemSource.transform))
			{
				if (permittedSources.Contains(parentTransform.gameObject) == true)
				{
					hit = true;
					break;
				}
			}
			if (hit == false)
			{
				// If item source was moved outside or deleted - remove quick item
				quickItem.Remove();
			}
		}
	}
}
