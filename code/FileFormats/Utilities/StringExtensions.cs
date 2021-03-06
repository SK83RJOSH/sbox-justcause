namespace JustCause.FileFormats.Utilities;

using System;
using System.Text;

public static class StringExtensions
{
	private static uint HashJenkins(byte[] data, int index, int length, uint seed)
	{
		uint a, b, c;

		a = b = c = seed + (uint)length;

		int i = index;
		while (i + 12 < length)
		{
			a += (uint)data[i++] |
				 ((uint)data[i++] << 8) |
				 ((uint)data[i++] << 16) |
				 ((uint)data[i++] << 24);
			b += (uint)data[i++] |
				 ((uint)data[i++] << 8) |
				 ((uint)data[i++] << 16) |
				 ((uint)data[i++] << 24);
			c += (uint)data[i++] |
				 ((uint)data[i++] << 8) |
				 ((uint)data[i++] << 16) |
				 ((uint)data[i++] << 24);

			a -= c;
			a ^= (c << 4) | (c >> (32 - 4));
			c += b;
			b -= a;
			b ^= (a << 6) | (a >> (32 - 6));
			a += c;
			c -= b;
			c ^= (b << 8) | (b >> (32 - 8));
			b += a;
			a -= c;
			a ^= (c << 16) | (c >> (32 - 16));
			c += b;
			b -= a;
			b ^= (a << 19) | (a >> (32 - 19));
			a += c;
			c -= b;
			c ^= (b << 4) | (b >> (32 - 4));
			b += a;
		}

		if (i < length)
		{
			a += data[i++];
		}

		if (i < length)
		{
			a += (uint)data[i++] << 8;
		}

		if (i < length)
		{
			a += (uint)data[i++] << 16;
		}

		if (i < length)
		{
			a += (uint)data[i++] << 24;
		}

		if (i < length)
		{
			b += (uint)data[i++];
		}

		if (i < length)
		{
			b += (uint)data[i++] << 8;
		}

		if (i < length)
		{
			b += (uint)data[i++] << 16;
		}

		if (i < length)
		{
			b += (uint)data[i++] << 24;
		}

		if (i < length)
		{
			c += (uint)data[i++];
		}

		if (i < length)
		{
			c += (uint)data[i++] << 8;
		}

		if (i < length)
		{
			c += (uint)data[i++] << 16;
		}

		if (i < length)
		{
			c += (uint)data[i] << 24;
		}

		c ^= b;
		c -= (b << 14) | (b >> (32 - 14));
		a ^= c;
		a -= (c << 11) | (c >> (32 - 11));
		b ^= a;
		b -= (a << 25) | (a >> (32 - 25));
		c ^= b;
		c -= (b << 16) | (b >> (32 - 16));
		a ^= c;
		a -= (c << 4) | (c >> (32 - 4));
		b ^= a;
		b -= (a << 14) | (a >> (32 - 14));
		c ^= b;
		c -= (b << 24) | (b >> (32 - 24));

		return c;
	}

	public static uint HashJenkins(this string input)
	{
		byte[] data = Encoding.ASCII.GetBytes(input);
		return HashJenkins(data, 0, data.Length, 0xDEADBEEF);
	}

	public static KeyType HashJenkins<KeyType>(this string input)
	{
		// TODO: Do this in a not shitty way
		return (KeyType)Convert.ChangeType(input.HashJenkins(), typeof(KeyType));
	}
}
