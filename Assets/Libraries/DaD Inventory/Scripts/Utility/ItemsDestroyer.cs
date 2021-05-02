using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsDestroyer : MonoBehaviour
{
	private StackCell stackCell;

	void Start()
	{
		stackCell = GetComponent<StackCell>();
	}

	void Update()
	{
		if (stackCell != null)
		{
			stackCell.RemoveStackItem();
		}
	}
}
