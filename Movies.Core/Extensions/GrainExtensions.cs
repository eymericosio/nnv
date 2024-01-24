// ReSharper disable once CheckNamespace
using Orleans;
using Orleans.Runtime;

namespace Movies.Core.Extensions;

public static class GrainExtensions
{
	/// <summary>
	/// Returns the primary key of the grain of any type as a string.
	/// </summary>
	/// <param name="grain"></param>
	/// <returns></returns>
	public static string GetPrimaryKeyAny(this IAddressable grain)
	{
		return grain.GetPrimaryKeyString()
			   ?? (grain.IsPrimaryKeyBasedOnLong()
				   ? grain.GetPrimaryKeyLong().ToString()
				   : grain.GetPrimaryKey().ToString());
	}
}
