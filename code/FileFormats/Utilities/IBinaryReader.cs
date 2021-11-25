namespace JustCause.FileFormats.Utilities;

using System;
using System.IO;

public interface IBinaryReader<T>
{
	// This could be static abstract in C#11/C#10 preview which would reduce a lot of boilerplate
	public static bool Read(BinaryReader reader, out T value, Endian endian = Endian.Little)
		=> throw new NotImplementedException();
}
