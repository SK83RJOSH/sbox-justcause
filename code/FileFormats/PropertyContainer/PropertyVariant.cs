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
		{ typeof(uint), VariantType.Integer },
		{ typeof(short), VariantType.Integer },
		{ typeof(ushort), VariantType.Integer },
		{ typeof(long), VariantType.Integer },
		{ typeof(ulong), VariantType.Integer },
		{ typeof(sbyte), VariantType.Integer },
		{ typeof(byte), VariantType.Integer },
		{ typeof(bool), VariantType.Integer },
		{ typeof(float), VariantType.Float },
		{ typeof(double), VariantType.Float },
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
		if (typeof(ValueType).IsEnum)
		{
			Type = VariantType.Integer;
			Value = (int)(object)value;
			return true;
		}

		if (TypeToVariantType.TryGetValue(value.GetType(), out VariantType type))
		{
			Type = type;

			if (type == VariantType.Integer)
			{
				Value = Convert.ToInt32(value);
			}
			else if (type == VariantType.Float)
			{
				Value = Convert.ToSingle(value);
			}
			else
			{
				Value = value;
			}

			return true;
		}

		return false;
	}

	public bool GetValue<ValueType>(out ValueType value, ValueType default_value = default)
	{
		if (typeof(ValueType).IsEnum && Type == VariantType.Integer && typeof(ValueType).IsEnumDefined(Value))
		{
			value = (ValueType)Value;
			return true;
		}

		if (typeof(ValueType) == typeof(bool) && Type == VariantType.Integer)
		{
			value = (ValueType)(object)Convert.ToBoolean((int)Value);
			return true;
		}

		if (TypeToVariantType.TryGetValue(typeof(ValueType), out VariantType type) && Type == type)
		{
			Type value_type = typeof(ValueType);

			if (Type == VariantType.Integer)
			{
				if (value_type == typeof(int) || value_type == typeof(short) || value_type == typeof(long) || value_type == typeof(sbyte))
				{
					int int_value = (int)Value;
					value = (ValueType)(object)(true switch
					{
						true when value_type == typeof(short)  => Convert.ToInt16(int_value),
						true when value_type == typeof(int) => Convert.ToInt32(int_value),
						true when value_type == typeof(long) => Convert.ToInt64(int_value),
						true when value_type == typeof(sbyte) => Convert.ToSByte(int_value),
						_ => null
					});
					return true;
				}
				else if (value_type == typeof(uint) || value_type == typeof(ushort) || value_type == typeof(ulong) || value_type == typeof(byte))
				{
					uint uint_value = BitConverter.ToUInt32(BitConverter.GetBytes((int)Value));
					value = (ValueType)(object)(true switch
					{
						true when value_type == typeof(ushort) => Convert.ToUInt16(uint_value),
						true when value_type == typeof(uint) => Convert.ToUInt32(uint_value),
						true when value_type == typeof(ulong) => Convert.ToUInt64(uint_value),
						true when value_type == typeof(byte) => Convert.ToByte(uint_value),
						_ => null
					});
					return true;
				}
			}
			else if (Type == VariantType.Float)
			{
				float float_value = (float)Value;
				value = (ValueType)(object)(true switch
				{
					true when value_type == typeof(float) => Convert.ToSingle(float_value),
					true when value_type == typeof(double) => Convert.ToDouble(float_value),
					_ => null
				});
				return true;
			}
			else
			{
				value = (ValueType)Value;
				return true;
			}
		}

		value = default_value;
		return false;
	}
}
