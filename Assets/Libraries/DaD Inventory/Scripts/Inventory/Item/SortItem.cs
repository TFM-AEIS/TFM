using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item has specified type (sort).
/// </summary>
public class SortItem : MonoBehaviour
{
	[Tooltip("Item's type")]
	public string itemType = "";

	/// <summary>
	/// Gets the sort cell of tjis item.
	/// </summary>
	/// <returns>The sort cell.</returns>
	public SortCell GetSortCell()
	{
		return AccessUtility.GetComponentInParent<SortCell>(transform);
	}
}
