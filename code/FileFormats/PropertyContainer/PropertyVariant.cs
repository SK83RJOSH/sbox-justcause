namespace JustCause.FileFormats.PropertyContainer;

using System;
using System.Collections.Generic;

public enum VariantType : byte
{
	Unassigned,
	Integer,
	Float,
	String,
	Vec2,
	Vec3,
	Vec4,
	Mat3x3,
	Mat3x4,
	VecInt,
	VecFloat,
	SizeData,
}

public struct Vec2
{
	public float x;
	public float y;

	public Vec2(float[] values)
	{
		x = values[0];
		y = values[1];
	}
}

public struct Vec3
{
	public float x;
	public float y;
	public float z;

	public Vec3(float[] values)
	{
		x = values[0];
		y = values[1];
		z = values[2];
	}
}

public struct Vec4
{
	public float x;
	public float y;
	public float z;
	public float w;

	public Vec4(float[] values)
	{
		x = values[0];
		y = values[1];
		z = values[2];
		w = values[3];
	}
}

public struct Mat3x3
{
	public float[] data;

	public Mat3x3(float[] values)
	{
		data = values;
	}
}
public struct Mat3x4
{
	public float[] data;

	public Mat3x4(float[] values)
	{
		data = values;
	}
}

public class PropertyVariant
{
	static protected Dictionary<Type, VariantType> TypeToVariantType = new Dictionary<Type, VariantType>
	{
		{ typeof(int), VariantType.Integer },
		{ typeof(float), VariantType.Float },
		{ typeof(string), VariantType.String },
		{ typeof(Vec2), VariantType.Vec2 },
		{ typeof(Vec3), VariantType.Vec3 },
		{ typeof(Vec4), VariantType.Vec4 },
		{ typeof(Mat3x3), VariantType.Mat3x3 },
		{ typeof(Mat3x4), VariantType.Mat3x4 },
		{ typeof(int[]), VariantType.VecInt },
		{ typeof(float[]), VariantType.VecFloat },
		{ typeof(byte[]), VariantType.SizeData },
	};

	public VariantType GetVariantType() => Type;
	public object GetValue() => Value;

	protected VariantType Type;
	protected object Value;

	protected PropertyVariant(VariantType type, object value)
	{
		Type = type;
		Value = value;
	}

	public PropertyVariant() : this(VariantType.Unassigned, null) { }
	public PropertyVariant(int value) : this(VariantType.Integer, value) { }
	public PropertyVariant(float value) : this(VariantType.Float, value) { }
	public PropertyVariant(string value) : this(VariantType.String, value) { }
	public PropertyVariant(Vec2 value) : this(VariantType.Vec2, value) { }
	public PropertyVariant(Vec3 value) : this(VariantType.Vec3, value) { }
	public PropertyVariant(Vec4 value) : this(VariantType.Vec4, value) { }
	public PropertyVariant(Mat3x3 value) : this(VariantType.Mat3x3, value) { }
	public PropertyVariant(Mat3x4 value) : this(VariantType.Mat3x4, value) { }
	public PropertyVariant(int[] value) : this(VariantType.VecInt, value) { }
	public PropertyVariant(float[] value) : this(VariantType.VecFloat, value) { }
	public PropertyVariant(byte[] value) : this(VariantType.SizeData, value) { }

	public bool AssignValue<ValueType>(ValueType value)
	{
		if (TypeToVariantType.TryGetValue(value.GetType(), out VariantType type))
		{
			Type = type;
			Value = value;
			return true;
		}

		return false;
	}

	public bool GetValue<ValueType>(out ValueType value)
	{
		if (TypeToVariantType.TryGetValue(typeof(ValueType), out VariantType type) && Type == type)
		{
			value = (ValueType)Value;
			return true;
		}

		value = default;
		return false;
	}
}
