using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickButtons : MonoBehaviour
{
	void Update()
	{
		if (Input.anyKeyDown == true)
		{
			int key = -1;

			for (int i = 1; i <= 9; ++i)
			{
				if (Input.GetKeyDown(i.ToString()) == true)
				{
					key = i;
					break;
				}
			}
			if (key != -1)
			{
				QuickCell[] quickCells = GetComponentsInChildren<QuickCell>();
				if (quickCells.Length >= key - 1)
				{
					QuickItem quickItem = quickCells[key - 1].GetQuickItem();
					if (quickItem != null)
					{
						quickItem.UseQuickItem();
					}
				}
			}
		}
	}
}
