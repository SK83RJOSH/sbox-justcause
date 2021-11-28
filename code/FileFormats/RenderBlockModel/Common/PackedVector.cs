namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using System;

// Another use case for static abstract methods in C#11 preview
public abstract class PackingModel<T>
{
	public abstract T Unpack(float value);

	protected float Frac(float value)
	{
		return value - MathF.Floor(value);
	}
}

public sealed class NormalPackingModel : PackingModel<Vector3<float>>
{
	public override Vector3<float> Unpack(float value)
	{
		float abs_value = MathF.Abs(value);

		return new(
			Frac(abs_value) * 2.0f - 1.0f,
			Frac(abs_value / 256.0f) * 2.0f - 1.0f,
			Frac(abs_value / 65536.0f) * 2.0f - 1.0f
		);
	}
}

public sealed class ColorPackingModelRGB : PackingModel<Vector3<float>>
{
	public override Vector3<float> Unpack(float value)
	{
		return new(
			Frac(value),
			Frac(value / 64.0f),
			Frac(value / 4096.0f)
		);
	}
}

public sealed class ColorPackingModelRGBA : PackingModel<Vector4<float>>
{
	public override Vector4<float> Unpack(float value)
	{
		return new(
			Frac(value),
			Frac(value / 64.0f),
			Frac(value / 4096.0f),
			Frac(value / 262144.0f)
		);
	}
}

public partial struct PackedVector3<T> where T : PackingModel<Vector3<float>>, new()
{
	public Vector3<float> Value;
	public float Sign;

	public PackedVector3(Vector3<float> value, float sign)
	{
		Value = value;
		Sign = sign;
	}
}

public partial struct PackedVector4<T> where T : PackingModel<Vector4<float>>, new()
{
	public Vector4<float> Value;
	public float Sign;

	public PackedVector4(Vector4<float> value, float sign)
	{
		Value = value;
		Sign = sign;
	}
}
