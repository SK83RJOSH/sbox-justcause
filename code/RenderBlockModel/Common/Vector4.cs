namespace JustCause.RenderBlockModel.Common;

using JustCause.RenderBlockModel.Utilities;
using System.IO;

public struct Vector4<T> : IBinaryFormat where T : struct
{
	public T X;
	public T Y;
	public T Z;
	public T W;

	public Vector4(T Value)
	{
		X = Value;
		Y = Value;
		Z = Value;
		W = Value;
	}

	public Vector4(T X, T Y, T Z, T W)
	{
		this.X = X;
		this.Y = Y;
		this.Z = Z;
		this.W = W;
	}

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		X = reader.Read<T>(endian);
		Y = reader.Read<T>(endian);
		Z = reader.Read<T>(endian);
		W = reader.Read<T>(endian);
	}
}
