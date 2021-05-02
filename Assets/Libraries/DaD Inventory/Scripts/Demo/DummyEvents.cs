using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEvents : MonoBehaviour
{
	[Tooltip("Enable debug logs with stack events information")]
	public bool showStackEventsInfo = true;

	/// <summary>
	/// Raises the stack group event event.
	/// </summary>
	public void OnStackGroupEvent(StackGroup.StackGroupEventDescriptor desc)
	{
		if (showStackEventsInfo == true)
		{
			string log = " - Stack group event - \n";
			log += "Source group: " + desc.sourceGroup.name + "; ";
			log += "Source cell: " + desc.sourceCell.name + "; ";
			log += "Destination group: " + desc.destinationGroup.name + "; ";
			log += "Destination cells: ";
			foreach (StackCell cell in desc.destinationCells)
			{
				log += cell.name + " ";
			}
			Debug.Log(log);
		}
	}
}
