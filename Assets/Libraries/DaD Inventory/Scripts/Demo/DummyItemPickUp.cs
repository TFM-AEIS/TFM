using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dummy example of item pick up on click.
/// </summary>
public class DummyItemPickUp : MonoBehaviour
{
	[Tooltip("Stack item prefab")]
	public StackItem itemPrefab;
	[Tooltip("Stack amount")]
	public int stack = 1;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		Debug.Assert(itemPrefab, "Wrong settings");
		Highlight highlight = GetComponent<Highlight>();
		if (highlight != null)
		{
			highlight.StartHighlight();
		}
	}
}
