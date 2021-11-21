namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public enum Packing
{
	XZY,
	ZXY,
	Normal,
	Color
}

public struct PackedVector3
{
	public Packing Packing { get; private set; }
	public float X;
	public float Y;
	public float Z;
	public float Packed;

	public PackedVector3(Packing packing) : this()
	{
		Packing = packing;
	}

	public void Deserialize(BinaryReader reader, Endian endian, bool abs = false)
	{
		Packed = reader.ReadSingle(endian);

		if (abs)
		{
			Packed = MathF.Abs(Packed);
		}

		switch (Packing)
		{
			case Packing.XZY:
				X = Packed;
				Y = Packed / 65536.0f;
				Z = Packed / 256.0f;
				break;
			case Packing.ZXY:
				X = Packed / 256.0f;
				Y = Packed / 65536.0f;
				Z = Packed;
				break;
			case Packing.Normal:
				X = Packed;
				Y = Packed / 256.0f;
				Z = Packed / 65536.0f;
				break;
			case Packing.Color:
				X = Packed;
				Y = Packed / 64;
				Z = Packed / 4096;
				break;
		}

		X -= (float)Math.Floor(X);
		Y -= (float)Math.Floor(Y);
		Z -= (float)Math.Floor(Z);

		if (Packing != Packing.Color)
		{
			X = X * 2 - 1;
			Y = Y * 2 - 1;
			Z = Z * 2 - 1;
		}
	}
}
