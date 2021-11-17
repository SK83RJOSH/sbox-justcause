namespace JustCause.RenderBlockModel;

using JustCause.RenderBlockModel.Common;
using JustCause.RenderBlockModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public struct ModelFile
{
	public Version Version;
	public Vector3<float> Min;
	public Vector3<float> Max;
	public readonly List<IRenderBlock> Blocks = new();

	public void Deserialize(BinaryReader reader)
	{
		uint magicLength = reader.ReadUInt32(Endian.Little);

		if (magicLength != 5 && magicLength != 83886080)
		{
			throw new FormatException("invalid magic length");
		}

		Endian endian = magicLength == 5 ? Endian.Little : Endian.Big;
		string magic = reader.ReadString(5, Encoding.ASCII);

		if (magic != "RBMDL")
		{
			throw new FormatException("invalid magic");
		}

		int versionMajor = reader.ReadInt32(endian);
		int versionMinor = reader.ReadInt32(endian);
		int versionRevision = reader.ReadInt32(endian);

		if (versionMajor != 1 || versionMinor != 13)
		{
			throw new FormatException("unsupported RBMDL version");
		}

		Version = new Version(versionMajor, versionMinor, 0, versionRevision);
		Min.Deserialize(reader, endian);
		Max.Deserialize(reader, endian);

		Blocks.Clear();
		Blocks.Capacity = reader.ReadInt32(endian);

		for (int i = 0; i < Blocks.Capacity; i++)
		{
			uint typeHash = reader.ReadUInt32(endian);
			IRenderBlock block = BlockTypeFactory.Create(typeHash);

			if (block == null)
			{
				string typeName = BlockTypeFactory.GetName(typeHash);

				if (string.IsNullOrEmpty(typeName) == false)
				{
					throw new NotSupportedException("unhandled block type " + typeName + " (0x" + typeHash.ToString("X8") + ")");
				}

				throw new NotSupportedException("unknown block type 0x" + typeHash.ToString("X8"));
			}

			block.Deserialize(reader, endian);

			if (reader.ReadUInt32(endian) != 0x89ABCDEF)
			{
				throw new FormatException("invalid block footer (data corrupt or misread)");
			}

			Blocks.Add(block);
		}
	}
}
