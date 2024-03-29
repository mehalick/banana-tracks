﻿using BananaTracks.Domain.Entities;

namespace BananaTracks.Domain.Extensions;

public static class EntityExtensions
{
	/// <summary>
	/// Filters collection to only include active entities.
	/// </summary>
	public static IEnumerable<T> Active<T>(this IEnumerable<T> entities) where T : EntityBase
	{
		return entities.Where(i => i.Status == EntityStatus.Active);
	}
}
