using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

/// <summary>
/// DaD Inventory editor menu.
/// </summary>
public class DaDI : EditorWindow
{
	[MenuItem("Window/DaDI/UI", false, 0)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void UI()
	{
		// Create full UI from prefab
		string prefabName = "Demo/UiCanvas";
		GameObject prefab = Resources.Load<GameObject>(prefabName);
		if (prefab != null)
		{
			GameObject go = Instantiate(prefab);
			go.name = prefab.name;
			go.transform.SetAsLastSibling();

			// Crete EventSystem
			if (FindObjectOfType<EventSystem>() == null)
			{
				GameObject eventSystemGO = new GameObject();
				eventSystemGO.name = "EventSystem";
				eventSystemGO.AddComponent<EventSystem>();
				eventSystemGO.AddComponent<StandaloneInputModule>();
			}

			// Add AudioSource to main camera
			if (Camera.main != null && Camera.main.GetComponent<AudioSource>() == null)
			{
				Camera.main.gameObject.AddComponent<AudioSource>().playOnAwake = false;
			}
		}
		else
		{
			Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
		}
	}

	[MenuItem("Window/DaDI/Sheets/Equipment", false, 11)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void Equipment()
	{
		bool res = false;
		if (Selection.activeGameObject != null)
		{
			if (Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
			{
				res = true;
			}
		}

		if (res == true)
		{
			// Create sheet from prefab
			string prefabName = "Demo/Equipments";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, Selection.activeGameObject.transform);
				go.name = prefab.name;
				go.transform.SetAsLastSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any canvas on scene before add this element");
		}
	}

	[MenuItem("Window/DaDI/Sheets/Inventory", false, 11)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void Inventory()
	{
		bool res = false;
		if (Selection.activeGameObject != null)
		{
			if (Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
			{
				res = true;
			}
		}

		if (res == true)
		{
			// Create sheet from prefab
			string prefabName = "Demo/InventorySheet";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, Selection.activeGameObject.transform);
				go.name = prefab.name;
				go.transform.SetAsLastSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any canvas on scene before add this element");
		}
	}

	[MenuItem("Window/DaDI/Sheets/Skills", false, 11)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void Skills()
	{
		bool res = false;
		if (Selection.activeGameObject != null)
		{
			if (Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
			{
				res = true;
			}
		}

		if (res == true)
		{
			// Create sheet from prefab
			string prefabName = "Demo/Skills";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, Selection.activeGameObject.transform);
				go.name = prefab.name;
				go.transform.SetAsLastSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any canvas on scene before add this element");
		}
	}


	[MenuItem("Window/DaDI/Sheets/QickSlots", false, 11)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void QickSlots()
	{
		bool res = false;
		if (Selection.activeGameObject != null)
		{
			if (Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
			{
				res = true;
			}
		}

		if (res == true)
		{
			// Create sheet from prefab
			string prefabName = "Demo/QuickSlots";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, Selection.activeGameObject.transform);
				go.name = prefab.name;
				go.transform.SetAsLastSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any canvas on scene before add this element");
		}
	}

	[MenuItem("Window/DaDI/Sheets/Vendor", false, 11)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void Vendor()
	{
		bool res = false;
		if (Selection.activeGameObject != null)
		{
			if (Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
			{
				res = true;
			}
		}

		if (res == true)
		{
			// Create sheet from prefab
			string prefabName = "Demo/Vendor";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, Selection.activeGameObject.transform);
				go.name = prefab.name;
				go.transform.SetAsLastSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any canvas on scene before add this element");
		}
	}

	[MenuItem("Window/DaDI/Item", false, 22)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void Item()
	{
		StackCell res = null;
		if (Selection.activeGameObject != null)
		{
			res = AccessUtility.GetComponentInParent<StackCell>(Selection.activeGameObject.transform);
		}

		if (res != null)
		{
			// Create item from prefab
			string prefabName = "Demo/Item";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, res.transform);
				go.name = prefab.name;
				go.transform.SetAsFirstSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any stack cell on scene before add this element");
		}
	}

	[MenuItem("Window/DaDI/Skill", false, 33)]
	/// <summary>
	/// On menu item select.
	/// </summary>
	static void Skill()
	{
		DadCell res = null;
		if (Selection.activeGameObject != null)
		{
			res = AccessUtility.GetComponentInParent<DadCell>(Selection.activeGameObject.transform);
		}

		if (res != null)
		{
			// Create skill from prefab
			string prefabName = "Demo/Skill";
			GameObject prefab = Resources.Load<GameObject>(prefabName);
			if (prefab != null)
			{
				GameObject go = Instantiate(prefab, res.transform);
				go.name = prefab.name;
				go.transform.SetAsFirstSibling();
			}
			else
			{
				Debug.Log("Can't find needed prefab in Resources folder: " + prefabName);
			}
		}
		else
		{
			Debug.Log("Please choose any DaD cell on scene before add this element");
		}
	}
}
