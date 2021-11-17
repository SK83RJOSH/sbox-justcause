namespace JustCause.RenderBlockModel.Common;

using JustCause.RenderBlockModel.Utilities;
using System;
using System.IO;

public enum Packing
{
	XZY,
	ZXY,
	Normal,
	Color
}

public struct PackedVector3 : IBinaryFormat
{
	public Packing Packing { get; private set; }
	public float X;
	public float Y;
	public float Z;

	public PackedVector3(Packing packing) : this()
	{
		Packing = packing;
	}

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		float packed = reader.ReadSingle(endian);

		switch (Packing)
		{
			case Packing.XZY:
				X = packed;
				Y = packed / 65536.0f;
				Z = packed / 256.0f;
				break;
			case Packing.ZXY:
				X = packed / 256.0f;
				Y = packed / 65536.0f;
				Z = packed;
				break;
			case Packing.Normal:
				X = packed;
				Y = packed / 256.0f;
				Z = packed / 65536.0f;
				break;
			case Packing.Color:
				X = packed;
				Y = packed / 64;
				Z = packed / 4096;
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
