using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CheckNamespace
namespace Orckestra.Overture.ServiceModel
{
	public static class ExtendEntityBase
	{
		public static T FindById<T>(this IEnumerable<T> entities, string entityId)
			where T : EntityBase<string>
		{
			if (entities == null) { return null; }

			return
				string.IsNullOrWhiteSpace(entityId)
				? entities.FirstOrDefault()
				: entities.FirstOrDefault(
					v => string.Equals(v.Id, entityId, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
