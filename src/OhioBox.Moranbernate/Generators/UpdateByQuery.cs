﻿using System;
using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Generators
{
	internal class UpdateByQuery<T>
		where T : class
	{
		private readonly ClassMap<T> _map;

		public UpdateByQuery()
		{
			_map = MappingRepo<T>.GetMap();

		}

		public string GetSql(Action<IKeyValuePairBuilder<T>> action, Action<IRestrictable<T>> restriction, List<object> parameters)
		{
			var builder = new KeyValuePairBuilder<T>();
			action(builder);

			var properties = builder.GetEnumerable().ToArray();

			if (properties.Any(x => x.Item1.ReadOnly))
				throw new UpdateByQueryException("Update by query columns contained at least one read only column");

			var columns = properties
				.Select(x => {
					var sql = x.Item1.ColumnName + " = " + _map.CreateParameter("p" + parameters.Count);
					var value = x.Item1.ConvertValue(x.Item2);
					parameters.Add(value);
					return sql;
				}).ToArray();

			var restrictable = new Restrictable<T>();
			restriction(restrictable);

			var where = restrictable.BuildRestrictionsIncludeWhere(parameters, _map.Dialect);

			return string.Format("UPDATE {0} SET {1}{2};", _map.TableName, string.Join(", ", columns), where);
		}
	}

	public class UpdateByQueryException : Exception
	{
		public UpdateByQueryException(string message) : base(message)
		{
		}
	}
}