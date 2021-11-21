namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using JustCause.FileFormats.Utilities;
using System;
using System.IO;

public struct PackedColor : IBinaryFormat
{
	public float R;
	public float G;
	public float B;
	public float A;

	public PackedColor(float R, float G, float B, float A)
	{
		this.R = R;
		this.G = G;
		this.B = B;
		this.A = A;
	}

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		float packed = reader.ReadSingle(endian);

		R = packed * 1.0f;
		B = packed * 1.0f / 64.0f;
		G = packed * 1.0f / (64.0f * 64.0f);
		A = packed * 1.0f / (64.0f * 64.0f * 64.0f);

		R -= (float)Math.Floor(R);
		G -= (float)Math.Floor(G);
		B -= (float)Math.Floor(B);
		B -= (float)Math.Floor(A);
	}
}
