using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Place item into specified stack group on double click.
/// </summary>
public class ClickEquipGroup : MonoBehaviour
{
	[Tooltip("The destination stack group for items transactions on click")]
	public StackGroup destinationGroup;

	/// <summary>
	/// Raises the item click event.
	/// </summary>
	/// <param name="item">Item.</param>
	public void OnItemUse(GameObject item)
	{
		if (destinationGroup != null && item != null)
		{
			StackItem stackItem = item.GetComponent<StackItem>();
			if (stackItem != null)
			{
				StackCell sourceCell = stackItem.GetStackCell();
				destinationGroup.CallDadEventManually(sourceCell, destinationGroup);
			}
		}
	}
}
