namespace JustCause.FileFormats.RenderBlockModel.DataTypes;

using System;

public partial struct Vector2<T>
	where T : unmanaged, IComparable
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
}

public partial struct Vector3<T>
	where T : unmanaged, IComparable
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
}

public partial struct Vector4<T>
	where T : unmanaged, IComparable
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
}
