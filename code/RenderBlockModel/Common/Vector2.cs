namespace JustCause.RenderBlockModel.Common;

using JustCause.RenderBlockModel.Utilities;
using System.IO;

public struct Vector2<T> : IBinaryFormat where T : struct
{
	public T X;
	public T Y;

	public Vector2(T Value)
	{
		X = Value;
		Y = Value;
	}

	public Vector2(T X, T Y)
	{
		this.X = X;
		this.Y = Y;
	}

	public void Deserialize(BinaryReader reader, Endian endian)
	{
		X = reader.Read<T>(endian);
		Y = reader.Read<T>(endian);
	}
}
