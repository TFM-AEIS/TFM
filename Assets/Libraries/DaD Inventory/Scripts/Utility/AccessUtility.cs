using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.SceneManagement;

/// <summary>
/// Allows to GetComponent on inactive gameobjects.
/// </summary>
public static class AccessUtility
{
	/// <summary>
	/// Gets component in parent (including inactive state).
	/// </summary>
	/// <returns>The component in parent.</returns>
	/// <param name="child">Child.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T GetComponentInParent<T>(Transform child) where T : Component
	{
		T res = null;
		Transform transform = child;
		while (true)
		{
			if (transform == null)
			{
				break;
			}
			res = transform.GetComponent<T>();
			if (res != null)
			{
				break;
			}
			transform = transform.parent;
		}
		return res;
	}

	/// <summary>
	/// Gets the components in parent (including inactive state).
	/// </summary>
	/// <returns>The components in parent.</returns>
	/// <param name="child">Child.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static List<T> GetComponentsInParent<T>(Transform child) where T : Component
	{
		List<T> res = new List<T>();
		Transform transform = child;
		while (true)
		{
			if (transform == null)
			{
				break;
			}
			foreach (T comp in transform.GetComponents<T>())
			{
				res.Add(comp);
			}
			transform = transform.parent;
		}
		return res;
	}

	/// <summary>
	/// Sends the message (including inactive state).
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <param name="methodName">Method name.</param>
	/// <param name="value">Value.</param>
	public static void SendMessage(Transform obj, string methodName, object value)
	{
		foreach (Component component in obj.GetComponents<Component>())
		{
			MethodInfo methodInfo = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (methodInfo != null)
			{
				methodInfo.Invoke(component, new object[] {value});
			}
		}
	}

	/// <summary>
	/// Sends the message (including inactive state).
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <param name="methodName">Method name.</param>
	public static void SendMessage(Transform obj, string methodName)
	{
		foreach (Component component in obj.GetComponents<Component>())
		{
			MethodInfo methodInfo = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (methodInfo != null)
			{
				methodInfo.Invoke(component, new object[] {});
			}
		}
	}

	/// <summary>
	/// Sends the message upwards (including inactive gameobjects).
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <param name="methodName">Method name.</param>
	/// <param name="value">Value.</param>
	public static void SendMessageUpwards(Transform child, string methodName, object value)
	{
		foreach (Component component in GetComponentsInParent<Component>(child))
		{
			MethodInfo methodInfo = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (methodInfo != null)
			{
				methodInfo.Invoke(component, new object[] {value});
			}
		}
	}

	/// <summary>
	/// Sends the message upwards (including inactive gameobjects).
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <param name="methodName">Method name.</param>
	public static void SendMessageUpwards(Transform child, string methodName)
	{
		foreach (Component component in GetComponentsInParent<Component>(child))
		{
			MethodInfo methodInfo = component.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			if (methodInfo != null)
			{
				methodInfo.Invoke(component, new object[] {});
			}
		}
	}

	/// <summary>
	/// Finds the component of type T including inactive gameobjects on scene.
	/// </summary>
	/// <returns>The object of type T.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T FindObjectOfType<T>() where T : Component
	{
		T res = null;
		foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			res = go.GetComponentInChildren<T>(true);
			if (res != null)
			{
				break;
			}
		}
		return res;
	}

	/// <summary>
	/// Finds the components of type T including inactive gameobjects on scene.
	/// </summary>
	/// <returns>The objects of type.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static List<T> FindObjectsOfType<T>() where T : Component
	{
		List<T> res = new List<T>(); 
		foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			res.AddRange(go.GetComponentsInChildren<T>(true));
		}
		return res;
	}
}
