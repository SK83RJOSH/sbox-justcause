namespace JustCause.RenderBlockModel.Common;

using JustCause.RenderBlockModel.Utilities;
using System.IO;

public struct Vector3<T> : IBinaryFormat where T : struct
{
	public T X;
	public T Y;
	public T Z;

	public Vector3(T Value)
	{
		X = Value;
		Y = Value;
		Z = Value;
	}

	public Vector3(T X, T Y, T Z)
	{
		this.X = X;
		this.Y = Y;
		this.Z = Z;
	}

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		X = reader.Read<T>(endian);
		Y = reader.Read<T>(endian);
		Z = reader.Read<T>(endian);
	}
}
