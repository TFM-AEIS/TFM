using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This items may unite into stack.
/// </summary>
public class StackItem : MonoBehaviour
{
	[Tooltip("current stack of this item")]
	public int stack = 1;
	[Tooltip("Max stack amount for this item")]
	public int maxStack = 10;
	[Tooltip("Game object with stack text field")]
	public GameObject stackObject;
	[Tooltip("Stack text field")]
	public Text stackText;
	[Tooltip("SFX when stack operations")]
	public AudioClip sound;

	private bool inited = false;												// Was internal state inited?

	/// <summary>
	/// Init this instance.
	/// </summary>
	public void Init()
	{
		if (inited == false)
		{
			// Set stack amount
			SetStack(stack);
			CooldownItem cooldownItem = GetComponent<CooldownItem>();
			if (cooldownItem != null)
			{
				// Init cooldown item
				cooldownItem.Init();
			}
			inited = true;
		}
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Debug.Assert(stackObject && stackText, "Wrong settings");
		Init();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		UpdateCondition();
	}

	/// <summary>
	/// Updates stack's condition.
	/// </summary>
	private void UpdateCondition()
	{
		int stack = GetStack();
		if (stack > 1)
		{
			ShowStack();
		}
		else if (stack == 1)
		{
			// Hide stack text if stack == 0
			HideStack();
		}
		else
		{
			// Stack <= 0
			DadCell dadCell = AccessUtility.GetComponentInParent<DadCell>(transform);
			if (dadCell != null)
			{
				dadCell.RemoveItem();
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	/// <summary>
	/// Gets the stack cell of this item.
	/// </summary>
	/// <returns>The stack cell.</returns>
	public StackCell GetStackCell()
	{
		return AccessUtility.GetComponentInParent<StackCell>(transform);
	}

	/// <summary>
	/// Gets the stack of this item.
	/// </summary>
	/// <returns>The stack.</returns>
	public int GetStack()
	{
		return stack;
	}

	/// <summary>
	/// Sets the item's stack.
	/// </summary>
	/// <param name="stack">Stack.</param>
	public void SetStack(int stack)
	{
		this.stack = stack;
		if (stack == int.MaxValue)
		{
			stackText.text = "∞";
		}
		else
		{
			stackText.text = this.stack.ToString();
		}
		UpdateCondition();
	}

	/// <summary>
	/// Adds the stack.
	/// </summary>
	/// <param name="stack">Stack.</param>
	public void AddStack(int stack)
	{
		if (this.stack != int.MaxValue)
		{
			SetStack(GetStack() + stack);
		}
	}

	/// <summary>
	/// Reduces the stack.
	/// </summary>
	/// <param name="stack">Stack.</param>
	public void ReduceStack(int stack)
	{
		if (this.stack != int.MaxValue)
		{
			SetStack(GetStack() - stack);
		}
	}

	/// <summary>
	/// Shows the stack.
	/// </summary>
	public void ShowStack()
	{
		stackObject.SetActive(true);
	}

	/// <summary>
	/// Hides the stack.
	/// </summary>
	public void HideStack()
	{
		stackObject.SetActive(false);
	}
}
