using Elegance.Enums;

namespace WebGames.Models
{
	[Enum]
	public enum ClaimType
	{
		Invalid = -1,
		[EnumValue("id")] Id,
		[EnumValue("username")] Username,
		[EnumValue("email")] Email,
		[EnumValue("created")] Created,
	}
}
