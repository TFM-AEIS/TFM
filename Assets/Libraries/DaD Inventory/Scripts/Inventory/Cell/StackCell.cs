using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Items in this cell may unite in stack.
/// </summary>
public class StackCell : MonoBehaviour
{
	[Tooltip("Stack limit for this cell")]
	public int cellStackLimit = 100;

	/// <summary>
	/// Gets the stack item from this cell.
	/// </summary>
	/// <returns>The stack item.</returns>
	public StackItem GetStackItem()
	{
		StackItem res = null;
		GameObject item = GetComponent<DadCell>().GetItem();
		if (item != null)
		{
			res = item.GetComponent<StackItem>();
		}
		return res;
	}

	/// <summary>
	/// Removes the stack item from this cell.
	/// </summary>
	public void RemoveStackItem()
	{
		DadCell dadCell = GetComponent<DadCell>();
		if (dadCell != null)
		{
			dadCell.RemoveItem();
		}
	}

	/// <summary>
	/// Check if item has same name as item in this cell.
	/// </summary>
	/// <returns><c>true</c> if this instance has same item the specified stackItem; otherwise, <c>false</c>.</returns>
	/// <param name="stackItem">Stack item.</param>
	public bool HasSameItem(StackItem stackItem)
	{
		bool res = false;
		if (stackItem != null)
		{
			StackItem myStackItem = GetStackItem();
			if (myStackItem != null)
			{
				if (myStackItem.name == stackItem.name)
				{
					res = true;
				}
			}
		}
		return res;
	}

	/// <summary>
	/// Gets the allowed space in this cell.
	/// </summary>
	/// <returns>The allowed space.</returns>
	public int GetAllowedSpace()
	{
		int res = 0;
		StackItem myStackItem = GetStackItem();
		if (myStackItem != null)
		{
			int myStack = myStackItem.GetStack();

			Debug.Assert(cellStackLimit >= myStack && myStackItem.maxStack >= myStack, "Item's stack bigger than allowed stack");

			if (cellStackLimit == int.MaxValue && myStackItem.maxStack == int.MaxValue)
			{
				res = int.MaxValue;
			}
			else
			{
				int minimalStackLimit = Mathf.Min(cellStackLimit, myStackItem.maxStack);
				res = minimalStackLimit - myStack;
			}
		}
		else
		{
			res = cellStackLimit;
		}

		if (res < 0)
		{
			res = 0;
		}

		return res;
	}

	/// <summary>
	/// Crete item from prefab and place it in cell. Current item will be removed
	/// </summary>
	/// <returns>The item from prefab.</returns>
	/// <param name="itemPrefab">Item prefab.</param>
	/// <param name="stackAmount">Stack amount of created item.</param>
	public StackItem AddItemFromPrefab(StackItem itemPrefab, int stackAmount)
	{
		StackItem res = null;
		DadCell dadCell = GetComponent<DadCell>();
		if (dadCell != null)
		{
			// Create item
			res = Instantiate(itemPrefab);
			// Remove current stack item
			RemoveStackItem();
			// Place item into cell
			dadCell.AddItem(res.gameObject);
			res.name = itemPrefab.name;
			// Set stack amount for created item
			stackAmount = Mathf.Min(stackAmount, cellStackLimit, res.maxStack);
			res.SetStack(stackAmount);
		}
		return res;
	}

	/// <summary>
	/// Unites item in stack with this cell.
	/// </summary>
	/// <returns>The stack.</returns>
	/// <param name="stackItem">Stack item.</param>
	/// <param name="limit">Stack limit.</param>
	public int UniteStack(StackItem stackItem, int limit)
	{
		int res = 0;
		if (stackItem != null)
		{
			int allowedSpace = GetAllowedSpace();
			StackItem myStackItem = GetStackItem();
			if (myStackItem == null)														// Cell has no item
			{
				if (SortCell.IsSortAllowed(gameObject, stackItem.gameObject) == true) 		// Item type is allowed for this cell
				{
					if (stackItem.GetStack() == limit && allowedSpace >= limit)				// Cell has anough space for all item's stack
					{
						// Totaly place item in new cell
						DadCell sourceDadCell = AccessUtility.GetComponentInParent<DadCell>(stackItem.transform);
						if (sourceDadCell != null)
						{
							DadCell.SwapItems(gameObject, sourceDadCell.gameObject);
						}
						else
						{
							DadCell.AddItem(gameObject, stackItem.gameObject);
						}
						res = limit;
					}
					else 																	// Only part of item stack will be placed into new cell
					{
						// Create new stack item to put it into this cell
						StackItem newStackItem = Instantiate(stackItem);
						newStackItem.name = stackItem.name;
						DadCell.AddItem(gameObject, newStackItem.gameObject);
						// Check the correct amout of united item
						int stackDelta = Mathf.Min(stackItem.GetStack(), allowedSpace, limit);
						newStackItem.SetStack(stackDelta);
						stackItem.ReduceStack(stackDelta);
						res = stackDelta;
					}
				}
			}
			else if (HasSameItem(stackItem) == true)										// Cell has same item
			{
				int stackDelta = Mathf.Min(stackItem.GetStack(), allowedSpace, limit);
				myStackItem.AddStack(stackDelta);
				stackItem.ReduceStack(stackDelta);
				res = stackDelta;
			}
		}
		return res;
	}

	/// <summary>
	/// Swaps the stacks between cells.
	/// </summary>
	/// <returns><c>true</c>, if stacks was swaped, <c>false</c> otherwise.</returns>
	/// <param name="sourceStackCell">Source stack cell.</param>
	public bool SwapStacks(StackCell sourceStackCell)
	{
		bool res = false;
		if (sourceStackCell != null)
		{
			StackItem myStackItem = GetStackItem();
			StackItem theirStackItem = sourceStackCell.GetStackItem();
			if (myStackItem != null && theirStackItem != null)
			{
				if (	SortCell.IsSortAllowed(gameObject, theirStackItem.gameObject) == true
					&&	SortCell.IsSortAllowed(sourceStackCell.gameObject, myStackItem.gameObject) == true)
				{
					if (	cellStackLimit >= theirStackItem.GetStack()
						&& 	sourceStackCell.cellStackLimit >= myStackItem.GetStack())
					{
						DadCell.SwapItems(gameObject, sourceStackCell.gameObject);
						res = true;
					}
				}
			}
		}
		return res;
	}
}
