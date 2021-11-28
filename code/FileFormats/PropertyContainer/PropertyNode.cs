namespace JustCause.FileFormats.PropertyContainer;

using System;

public enum NodeType
{
	Invalid,
	Container,
	Variant,
}

public class PropertyNode<KeyType>
	where KeyType : unmanaged, IComparable
{
	protected NodeType Type = NodeType.Invalid;
	protected object Value;

	protected PropertyNode(NodeType type, object value)
	{
		Type = type;
		Value = value;
	}

	public PropertyNode(PropertyContainer<KeyType> value) : this(NodeType.Container, value) { }
	public PropertyNode(PropertyVariant value) : this(NodeType.Variant, value) { }

	public bool Contains<ValueType>()
	{
		if (typeof(ValueType) == typeof(PropertyVariant))
		{
			return Type == NodeType.Variant;
		}
		else if (typeof(ValueType) == typeof(PropertyContainer<KeyType>))
		{
			return Type == NodeType.Container;
		}

		return false;
	}

	protected bool GetValue<ValueType>(NodeType type, out ValueType value) where ValueType : class
	{
		if (Type == type)
		{
			value = Value as ValueType;
			return true;
		}

		value = default;
		return false;
	}

	public bool GetValue(out PropertyContainer<KeyType> value)
	{
		return GetValue(NodeType.Container, out value);
	}

	public bool GetValue(out PropertyVariant value)
	{
		return GetValue(NodeType.Variant, out value);
	}
}
