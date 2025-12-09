using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace WebGames.Utilities
{
	internal static class DisplayName
	{
		public static string Get(LambdaExpression lambda)
		{
			if (lambda.Body is MemberExpression member)
			{
				return member.Member.GetCustomAttribute<DisplayAttribute>()?.GetName() ??
					   member.Member.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
					   member.Member.Name;
			}

			throw new System.ArgumentException("Invalid lambda expression", nameof(lambda));
		}
	}
}
