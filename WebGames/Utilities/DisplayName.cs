using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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

		public static string Get<TEnum>(TEnum value) where TEnum : Enum
		{
			var name = Enum.GetName(typeof(TEnum), value);

			Debug.Assert(name is not null);

			var fieldInfo = typeof(TEnum).GetField(name);

			Debug.Assert(fieldInfo is not null);

			return fieldInfo.GetCustomAttribute<DisplayAttribute>(true)?.GetName() ?? name;
		}
	}
}
