namespace JustCause.FileFormats.PropertyContainer;

using JustCause.FileFormats.Utilities;
using System;
using System.Collections.Generic;

public static class PropertyContainerExtensions
{
	public static IEnumerator<PropertyContainer<KeyType>>
		GetEnumerator<KeyType>(this IEnumerator<PropertyContainer<KeyType>> enumerator)
		where KeyType : struct, IConvertible => enumerator;

	public static IEnumerator<PropertyVariant>
		GetEnumerator(this IEnumerator<PropertyVariant> enumerator)
		=> enumerator;
}

public class PropertyContainer<KeyType> where KeyType : struct, IConvertible
{
	protected Dictionary<KeyType, PropertyNode<KeyType>> KeyToNode = new();

	public static KeyType AsKey(string key)
		=> key.HashJenkins<KeyType>();

	public bool Delete(string key)
		=> Delete(AsKey(key));

	public bool Set(string key, PropertyNode<KeyType> value)
		=> Set(AsKey(key), value);

	public bool Get(string key, out PropertyNode<KeyType> value)
		=> Get(AsKey(key), out value);

	public void SetContainer(string key, PropertyContainer<KeyType> container)
		=> SetContainer(AsKey(key), container);

	public PropertyContainer<KeyType> CreateContainer(string key)
		=> CreateContainer(AsKey(key));

	public bool GetContainer(string key, out PropertyContainer<KeyType> container)
		=> GetContainer(AsKey(key), out container);

	public PropertyVariant CreateVariant(string key)
		=> CreateVariant(AsKey(key));

	public void SetVariant(string key, PropertyVariant variant)
		=> SetVariant(AsKey(key), variant);

	public bool GetVariant(string key, out PropertyVariant variant)
		=> GetVariant(AsKey(key), out variant);

	public bool GetValue<ValueType>(string key, out ValueType value)
		=> GetValue(AsKey(key), out value);

	public bool AssignValue<ValueType>(string key, ValueType value)
		=> AssignValue(AsKey(key), value);

	public IEnumerator<PropertyContainer<KeyType>> GetContainers()
	{
		foreach (PropertyNode<KeyType> node in KeyToNode.Values)
		{
			if (node.GetValue(out PropertyContainer<KeyType> container))
			{
				yield return container;
			}
		}
	}

	public IEnumerator<PropertyVariant> GetVariants()
	{
		foreach (PropertyNode<KeyType> node in KeyToNode.Values)
		{
			if (node.GetValue(out PropertyVariant variant))
			{
				yield return variant;
			}
		}
	}

	public bool Delete(KeyType key)
	{
		return KeyToNode.Remove(key);
	}

	public bool Set(KeyType key, PropertyNode<KeyType> value)
	{
		if (value != null)
		{
			KeyToNode[key] = value;
			return true;
		}

		return false;
	}

	public bool Get(KeyType key, out PropertyNode<KeyType> value)
	{
		if (KeyToNode.ContainsKey(key))
		{
			value = KeyToNode[key];
			return true;
		}

		value = default;
		return false;
	}

	public void SetContainer(KeyType key, PropertyContainer<KeyType> container)
	{
		KeyToNode[key] = new(container);
	}

	public PropertyContainer<KeyType> CreateContainer(KeyType key)
	{
		PropertyContainer<KeyType> container = new();
		SetContainer(key, container);
		return container;
	}

	public bool GetContainer(KeyType key, out PropertyContainer<KeyType> container)
	{
		if (KeyToNode.ContainsKey(key))
		{
			return KeyToNode[key].GetValue(out container);
		}

		container = default;
		return false;
	}

	public PropertyVariant CreateVariant(KeyType key)
	{
		PropertyVariant variant = new();
		SetVariant(key, variant);
		return variant;
	}

	public void SetVariant(KeyType key, PropertyVariant variant)
	{
		KeyToNode[key] = new(variant);
	}

	public bool GetVariant(KeyType key, out PropertyVariant variant)
	{
		if (KeyToNode.ContainsKey(key))
		{
			return KeyToNode[key].GetValue(out variant);
		}

		variant = default;
		return false;
	}

	public bool GetValue<ValueType>(KeyType key, out ValueType value)
	{
		if (GetVariant(key, out PropertyVariant variant))
		{
			return variant.GetValue(out value);
		}

		value = default;
		return false;
	}


	public bool AssignValue<ValueType>(KeyType key, ValueType value)
	{
		if (GetVariant(key, out PropertyVariant variant))
		{
			return variant.AssignValue(value);
		}

		return false;
	}
}
