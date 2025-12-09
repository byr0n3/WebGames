using System;

namespace WebGames.Database.Models
{
	[Flags]
	public enum UserFlags
	{
		None = 0,
		Active = 1 << 0,
		Confirmed = 1 << 1,
		Superuser = 1 << 2,
	}
}
