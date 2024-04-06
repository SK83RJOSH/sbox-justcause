namespace JustCause.FileFormats.Physics;

using JustCause.FileFormats.Physics.Havok;
using JustCause.FileFormats.Utilities;
using System.IO;

public struct LayoutRules
{
	public byte BytesInPointer;
	public bool LittleEndian;
	public bool ReusePaddingOptimization;
	public bool EmptyBaseClassOptimization;

	public static bool Read(BinaryReader reader, out LayoutRules rules)
	{
		rules = default;

		if (!reader.Read(out rules.BytesInPointer))
		{
			return false;
		}

		if (!reader.Read(out rules.LittleEndian))
		{
			return false;
		}

		if (!reader.Read(out rules.ReusePaddingOptimization))
		{
			return false;
		}

		if (!reader.Read(out rules.EmptyBaseClassOptimization))
		{
			return false;
		}

		return true;
	}
}

public struct PackfileHeader
{
	public int[] Magic;
	public int UserTag;
	public int FileVersion;
	public LayoutRules LayoutRules;
	public int NumSections;
	public int ContentsSectionIndex;
	public int ContentsSectionOffset;
	public int ContentsClassNameSectionIndex;
	public int ContentsClassNameSectionOffset;
	public string ContentsVersion;

	public bool IsMagicValid()
	{
		return Magic[0] == 0x57e0e057 && Magic[1] == 0x10c0c010;
	}

	public static bool Read(BinaryReader reader, out PackfileHeader header)
	{
		header = default;

		// Endian "agnostic magic" aka "could've been a string"
		if (!reader.Read(out header.Magic, 2) || !header.IsMagicValid())
		{
			return false;
		}

		if (!reader.Read(out header.UserTag))
		{
			return false;
		}

		if (!reader.Read(out header.FileVersion))
		{
			return false;
		}

		if (!LayoutRules.Read(reader, out header.LayoutRules))
		{
			return false;
		}

		Endian endian = header.LayoutRules.LittleEndian ? Endian.Little : Endian.Big;

		if (!endian.IsPlatformEndian())
		{
			header.UserTag = header.UserTag.Swap();
			header.FileVersion = header.FileVersion.Swap();
		}

		if (header.FileVersion != 5)
		{
			return false;
		}

		if (!reader.Read(out header.NumSections, endian))
		{
			return false;
		}

		if (!reader.Read(out header.ContentsSectionIndex, endian))
		{
			return false;
		}

		if (!reader.Read(out header.ContentsSectionOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out header.ContentsClassNameSectionIndex, endian))
		{
			return false;
		}

		if (!reader.Read(out header.ContentsClassNameSectionOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out header.ContentsVersion, 16))
		{
			return false;
		}

		header.ContentsVersion = header.ContentsVersion.Split('\0')[0];

		reader.Move(8);
		return true;
	}
}

public class PackfileSectionHeader
{
	public string SectionTag;
	public int GlobalDataStart;
	public int LocalFixupsOffset;
	public int GlobalFixupsOffset;
	public int VirtualFixupsOffset;
	public int ExportsOffset;
	public int ImportsOffset;
	public int EndOffset;

	public static bool Read(BinaryReader reader, out PackfileSectionHeader section, Endian endian)
	{
		section = new();

		if (!reader.Read(out section.SectionTag, 20))
		{
			return false;
		}

		section.SectionTag = section.SectionTag.Split('\0')[0];

		if (!reader.Read(out section.GlobalDataStart, endian))
		{
			return false;
		}

		if (!reader.Read(out section.LocalFixupsOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out section.GlobalFixupsOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out section.VirtualFixupsOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out section.ExportsOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out section.ImportsOffset, endian))
		{
			return false;
		}

		if (!reader.Read(out section.EndOffset, endian))
		{
			return false;
		}

		return true;
	}
}

public class PackfileSection
{
	public int BufferIndex;
	public int[] LocalFixups;
	public int[] GlobalFixups;
	public int[] VirtualFixups;
	public int[] Exports;
	public int[] Imports;

	public void ApplyLocalFixups(byte[] buffer, Endian endian)
	{
		// Apply local fix up offsets to the section data byte array
		for (int fixup_index = 0; fixup_index < LocalFixups.Length / 2; fixup_index++)
		{
			int source_offset = LocalFixups[(fixup_index * 2) + 0];
			int target_offset = LocalFixups[(fixup_index * 2) + 1];

			// TODO: Should *probably* throw an exception if not aligned
			if (source_offset != -1 && (source_offset % 4) == 0)
			{
				// Convert to absolute offset
				buffer.Set(source_offset + BufferIndex, target_offset + BufferIndex, endian);
			}
		}
	}

	public void ApplyGlobalFixups(byte[] buffer, PackfileSection[] sections, Endian endian)
	{
		ApplySectionFixups(buffer, sections, GlobalFixups, endian);
	}

	public void ApplyVirtualFixups(byte[] buffer, PackfileSection[] sections, Endian endian)
	{
		ApplySectionFixups(buffer, sections, VirtualFixups, endian);
	}

	private void ApplySectionFixups(byte[] buffer, PackfileSection[] sections, int[] fixups, Endian endian)
	{
		// Apply fix up offsets to the section data byte array
		for (int fixup_index = 0; fixup_index < fixups.Length / 3; fixup_index++)
		{
			int source_offset = fixups[(fixup_index * 3) + 0];
			int target_section = fixups[(fixup_index * 3) + 1];
			int target_offset = fixups[(fixup_index * 3) + 2];

			// TODO: Should *probably* throw an exception if not aligned
			if (source_offset != -1 && (source_offset % 4) == 0)
			{
				// Convert to absolute offset
				buffer.Set(source_offset + BufferIndex, target_offset + sections[target_section].BufferIndex, endian);
			}
		}
	}

	public static bool Read(BinaryReader reader, out PackfileSection section, PackfileSectionHeader header, byte[] buffer, int buffer_index, Endian endian)
	{
		section = new();

		// Set buffer index
		section.BufferIndex = buffer_index;

		// Read section data
		reader.Read(buffer, buffer_index, header.LocalFixupsOffset);

		// Read the local fixups
		int local_fixups_count = (header.GlobalFixupsOffset - header.LocalFixupsOffset) / sizeof(int);

		if (!reader.Read(out section.LocalFixups, local_fixups_count, endian))
		{
			return false;
		}

		// Read the global fixups
		int global_fixups_count = (header.VirtualFixupsOffset - header.GlobalFixupsOffset) / sizeof(int);

		if (!reader.Read(out section.GlobalFixups, global_fixups_count, endian))
		{
			return false;
		}

		// Read the virtual fixups (appear to be unused in version 5)
		int virtual_fixups_count = (header.ExportsOffset - header.VirtualFixupsOffset) / sizeof(int);

		if (!reader.Read(out section.VirtualFixups, virtual_fixups_count, endian))
		{
			return false;
		}

		// TODO: exports/imports appear to be (int offset, strz name) but are maybe unused in Just Cause 2
		// Read exports
		int exports_count = (header.ImportsOffset - header.ExportsOffset) / sizeof(int);

		if (!reader.Read(out section.Exports, exports_count, endian))
		{
			return false;
		}

		// Read imports
		int imports_count = (header.EndOffset - header.ImportsOffset) / sizeof(int);

		if (!reader.Read(out section.Imports, imports_count, endian))
		{
			return false;
		}

		return true;
	}
}

public class Packfile
{
	public PackfileHeader Header;
	public PackfileSectionHeader[] SectionHeaders;
	public PackfileSection[] Sections;
	public byte[] SectionsData;
	public object RootLevelObject;

	public int GetSectionDataIndex(int section_index, int offset = 0)
	{
		if (section_index < Sections.Length)
		{
			return Sections[section_index].BufferIndex + offset;
		}

		return -1;
	}

	public static bool Read(BinaryReader reader, out Packfile packfile)
	{
		packfile = new();

		if (!PackfileHeader.Read(reader, out packfile.Header))
		{
			return false;
		}

		Endian endian = packfile.Header.LayoutRules.LittleEndian ? Endian.Little : Endian.Big;

		if (!ReadSectionHeaders(reader, packfile, endian))
		{
			return false;
		}

		if (!ReadSections(reader, packfile, endian))
		{
			return false;
		}

		foreach (PackfileSection section in packfile.Sections)
		{
			section.ApplyLocalFixups(packfile.SectionsData, endian);
			section.ApplyGlobalFixups(packfile.SectionsData, packfile.Sections, endian);
			// TODO: Figure out why virtual fixups end up destroy local & global fix ups
			//section.ApplyVirtualFixups(packfile.SectionsData, packfile.Sections, endian);
		}

		return ReadRootLevelObject(packfile, endian);
	}

	private static bool ReadSectionHeaders(BinaryReader reader, Packfile packfile, Endian endian)
	{
		int sections_data_length = 0;

		packfile.SectionHeaders = new PackfileSectionHeader[packfile.Header.NumSections];

		for (int i = 0; i < packfile.SectionHeaders.Length; ++i)
		{
			if (!PackfileSectionHeader.Read(reader, out packfile.SectionHeaders[i], endian))
			{
				return false;
			}

			sections_data_length += packfile.SectionHeaders[i].LocalFixupsOffset;
		}

		packfile.SectionsData = new byte[sections_data_length];
		return true;
	}

	private static bool ReadSections(BinaryReader reader, Packfile packfile, Endian endian)
	{
		int sections_data_index = 0;

		packfile.Sections = new PackfileSection[packfile.Header.NumSections];

		for (int i = 0; i < packfile.SectionHeaders.Length; ++i)
		{
			if (!PackfileSection.Read(reader, out packfile.Sections[i], packfile.SectionHeaders[i], packfile.SectionsData, sections_data_index, endian))
			{
				return false;
			}

			sections_data_index += packfile.SectionHeaders[i].LocalFixupsOffset;
		}

		return true;
	}

	private static bool ReadRootLevelObject(Packfile packfile, Endian endian)
	{
		BinaryReader reader = new(new MemoryStream(packfile.SectionsData));

		reader.Seek(packfile.GetSectionDataIndex(
			packfile.Header.ContentsClassNameSectionIndex,
			packfile.Header.ContentsClassNameSectionOffset
		));

		if (!reader.Read(out string class_name, -1))
		{
			return false;
		}

		Log.Info($"Class Name: {class_name}");

		reader.Seek(packfile.GetSectionDataIndex(
			packfile.Header.ContentsSectionIndex,
			packfile.Header.ContentsSectionOffset
		));

		// hkRootLevelContainer
		hkRootLevelContainer root_level_container = new();

		if (!root_level_container.Read(reader, endian))
		{
			return false;
		}

		return true;
	}
}
