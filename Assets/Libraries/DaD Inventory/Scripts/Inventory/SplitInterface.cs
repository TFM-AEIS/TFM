using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface for items splitting.
/// </summary>
public class SplitInterface : MonoBehaviour
{
	[Tooltip("Stack amount of left item")]
	public Text leftAmount;
	[Tooltip("Stack amount of right item")]
	public InputField rightAmount;
	[Tooltip("Price game object of left item")]
	public GameObject leftPrice;
	[Tooltip("Price game object of right item")]
	public GameObject rightPrice;

	private Text leftPriceText;												// Text field of left item's price
	private Text rightPriceText;											// Text field of right item's price
	private StackItem stackItem;											// Current stack item
	private PriceItem priceItem;											// Current price item

	/// <summary>
	/// Show/Hide the prices GO.
	/// </summary>
	/// <param name="condition">If set to <c>true</c> condition.</param>
	private void ShowPrices(bool condition)
	{
		leftPrice.SetActive(condition);
		rightPrice.SetActive(condition);
	}

	/// <summary>
	/// Updates the prices using modifier.
	/// </summary>
	private void UpdatePrices()
	{
		if (priceItem != null)
		{
			int leftAmount = GetLeftAmount();
			leftPriceText.text = leftAmount == int.MaxValue ? "∞" : ((long)leftAmount * priceItem.GetPrice()).ToString();
			int rightAmount = GetRightAmount();
			rightPriceText.text = rightAmount == int.MaxValue ? "∞" : ((long)rightAmount * priceItem.GetPrice()).ToString();
		}
	}

	/// <summary>
	/// Sets the right stack amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	private void SetRightAmount(int amount)
	{
		int maxAmount = stackItem.GetStack();
		if (amount > maxAmount)
		{
			amount = maxAmount;
		}
		rightAmount.text = amount.ToString();
		// Update left stack amount
		leftAmount.text = maxAmount == int.MaxValue ? "∞" : (maxAmount - amount).ToString();
		UpdatePrices();
	}

	/// <summary>
	/// Gets the left stack amount.
	/// </summary>
	/// <returns>The left amount.</returns>
	public int GetLeftAmount()
	{
		int amount;
		if (int.TryParse(leftAmount.text, out amount) == false)
		{
			amount = int.MaxValue;
		}
		return amount;
	}

	/// <summary>
	/// Gets the right stack amount.
	/// </summary>
	/// <returns>The right amount.</returns>
	public int GetRightAmount()
	{
		int amount;
		if (int.TryParse(rightAmount.text, out amount) == false)
		{
			amount = int.MaxValue;
		}
		return amount;
	}

	/// <summary>
	/// Increases right stack amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void IncreaseAmount(int amount)
	{
		int currentAmount = GetRightAmount();
		int maxAmount = stackItem.GetStack();
		if (currentAmount < int.MaxValue - 1)
		{
			currentAmount += amount;
		}
		if (currentAmount > maxAmount)
		{
			currentAmount = maxAmount;
		}
		SetRightAmount(currentAmount);
	}

	/// <summary>
	/// Decreases right stack amount.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void DecreaseAmount(int amount)
	{
		int currentAmount = GetRightAmount();
		currentAmount -= amount;
		if (currentAmount < 0)
		{
			currentAmount = 0;
		}
		SetRightAmount(currentAmount);
	}

	/// <summary>
	/// Sets max right stack amount.
	/// </summary>
	public void SetMaxAmount()
	{
		if (stackItem.GetStack() != int.MaxValue)
		{
			SetRightAmount(stackItem.GetStack());
		}
	}

	/// <summary>
	/// Sets min right stack amount.
	/// </summary>
	public void SetMinAmount()
	{
		SetRightAmount(0);
	}

	/// <summary>
	/// Raises the right amount update event.
	/// </summary>
	public void OnRightAmountUpdate()
	{
		int amount = GetRightAmount();
		SetRightAmount(amount == int.MaxValue ? 0 : amount);
	}

	/// <summary>
	/// Shows split interface.
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="price">Price.</param>
	public void ShowSplitter(StackItem item, PriceItem price)
	{
		if (item != null)
		{
			// Set interface active
			gameObject.SetActive(true);
			Debug.Assert(leftAmount && rightAmount && leftPrice && rightPrice, "Wrong settings");
			// Get prices textfield
			leftPriceText = leftPrice.GetComponentInChildren<Text>(true);
			rightPriceText = rightPrice.GetComponentInChildren<Text>(true);
			Debug.Assert(leftPriceText && rightPriceText, "Wrong settings");
			stackItem = item;
			priceItem = price;
			// Hide prices if no different price groups
			ShowPrices(priceItem != null);
			// By default set min stack amount to split
			SetRightAmount(1);
		}
	}

	/// <summary>
	/// Splits the complete.
	/// </summary>
	public void SplitComplete()
	{
		gameObject.SetActive(false);
	}
}
