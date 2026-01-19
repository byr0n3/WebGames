using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebGames.Database.Extensions
{
	internal static class EntityTypeExtensions
	{
		extension(IEntityType entityType)
		{
			public IProperty GetPropertyByColumn(string column)
			{
				foreach (var property in entityType.GetProperties())
				{
					if (string.Equals(property.GetColumnName(), column, StringComparison.Ordinal))
					{
						return property;
					}
				}

				throw new ArgumentException($"No property with column '{column}' found", nameof(column));
			}
		}
	}
}
