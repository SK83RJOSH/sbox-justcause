
namespace JustCause.RenderBlockModel.Utilities;

using JustCause.RenderBlockModel.Common;

public static class ConversionHelpers
{
	public readonly static float APEX_TO_SOURCE = 52.49344f;
	public readonly static float SOURCE_TO_APEX = 0.0190499994f;
	public readonly static float FLOAT_TO_BYTE = 255;
	public readonly static float BYTE_TO_FLOAT = 0.00392156862f;

	public static Vector4<float> AsApexPosition(this Vector4 input)
	{
		return new Vector4<float>(
			-input.y * SOURCE_TO_APEX,
			input.z * SOURCE_TO_APEX,
			-input.x * SOURCE_TO_APEX,
			input.w * SOURCE_TO_APEX
		);
	}

	public static Vector4 AsSourcePosition(this Vector4<float> input)
	{
		return new Vector4(
			-input.Z * APEX_TO_SOURCE,
			-input.X * APEX_TO_SOURCE,
			input.Y * APEX_TO_SOURCE,
			input.W * APEX_TO_SOURCE
		);
	}

	public static Vector4<float> AsApex(this Vector4 input)
	{
		return new Vector4<float>(
			input.x,
			input.y,
			input.z,
			input.w
		);
	}

	public static Vector4 AsSource(this Vector4<float> input)
	{
		return new Vector4(
			input.X,
			input.Y,
			input.Z,
			input.W
		);
	}

	public static Vector3<float> AsApexPosition(this Vector3 input)
	{
		return new Vector3<float>(
			-input.y * SOURCE_TO_APEX,
			input.z * SOURCE_TO_APEX,
			-input.x * SOURCE_TO_APEX
		);
	}

	public static Vector3 AsSourcePosition(this Vector3<float> input)
	{
		return new Vector3(
			-input.Z * APEX_TO_SOURCE,
			-input.X * APEX_TO_SOURCE,
			input.Y * APEX_TO_SOURCE
		);
	}

	public static Vector3<float> AsApex(this Vector3 input)
	{
		return new Vector3<float>(
			input.x,
			input.y,
			input.z
		);
	}

	public static Vector3 AsSource(this Vector3<float> input)
	{
		return new Vector3(
			input.X,
			input.Y,
			input.Z
		);
	}

	public static Vector2<float> AsApex(this Vector2 input)
	{
		return new Vector2<float>(
			input.x,
			input.y
		);
	}

	public static Vector2 AsSource(this Vector2<float> input)
	{
		return new Vector2(
			input.X,
			input.Y
		);
	}

	public static PackedVector3 AsApexPacked(this Vector3 input, Packing packing)
	{
		PackedVector3 result = new(packing);
		result.X = input.x;
		result.Y = input.y;
		result.Z = input.z;
		return result;
	}

	public static Vector3 AsSource(this PackedVector3 input)
	{
		return new Vector3(
			input.X,
			input.Y,
			input.Z
		);
	}

	public static PackedColor AsApexPacked(this Color32 input)
	{
		return new PackedColor(
			input.R * BYTE_TO_FLOAT,
			input.G * BYTE_TO_FLOAT,
			input.B * BYTE_TO_FLOAT,
			input.A * BYTE_TO_FLOAT
		);
	}

	public static Color32 AsSource(this PackedColor input)
	{
		return new Color32(
			(byte)(input.R * FLOAT_TO_BYTE),
			(byte)(input.G * FLOAT_TO_BYTE),
			(byte)(input.B * FLOAT_TO_BYTE),
			(byte)(input.A * FLOAT_TO_BYTE)
		);
	}
}
