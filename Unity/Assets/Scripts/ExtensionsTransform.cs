using System;
using UnityEngine;

public static class ExtensionsTransform
{
	public static T SearchComponent<T>(this Component source, Predicate<T> predicate) where T : Component
	{
		// BFS

		predicate = predicate ?? (_ => true);

		if(source.gameObject.FilterComponent(predicate, out var result))
		{
			return result;
		}

		for(var index = 0; index < source.transform.childCount; index++)
		{
			if(source.transform.GetChild(index).gameObject.FilterComponent(predicate, out result))
			{
				return result;
			}
		}

		for(var index = 0; index < source.transform.childCount; index++)
		{
			result = source.transform.GetChild(index).GetComponent<RectTransform>().SearchComponent(predicate);
			if(result)
			{
				return result;
			}
		}

		return null;
	}

	public static bool FilterComponent<T>(this GameObject source, Predicate<T> predicate, out T component) where T : Component
	{
		var components = source.GetComponents<Component>();
		for(var index = 0; index < components.Length; index++)
		{
			component = components[index] as T;
			if(component is null || !predicate(component))
			{
				continue;
			}

			return true;
		}

		component = null;
		return false;
	}
}
