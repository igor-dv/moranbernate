﻿using System;
using System.Collections.Generic;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;
using OhioBox.Moranbernate.Querying;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	public class UpdateByQueryTests
	{
		[Test]
		public void Update_WhenSingleColumnSpecified_GeneratesSQL()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b.Set(x => x.Long, 3),
					q => q.In(x => x.Id, new[] { 1L, 2, 3 }),
					parameters
				);

			Assert.AreEqual("UPDATE `table_name` SET `LongColumnName` = ?p0 WHERE (`Id` IN (?p1,?p2,?p3));", sql);
			Assert.AreEqual(parameters[0], 3);
		}

		[Test]
		public void Update_CustomMapping_WhenSingleColumnSpecified_GeneratesSQL()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<CustomTypeObject>()
				.GetSql(
					b => b.Set(x => x.SomeIntArray, new[] { 1, 2, 3 }),
					q => q.Equal(x => x.Id, 1),
					parameters
				);

			Assert.AreEqual("UPDATE `table_name` SET `SomeIntArray` = ?p0 WHERE (`Id` = ?p1);", sql);
			Assert.AreEqual(parameters[0], "1,2,3");
		}


		[Test]
		public void Update_WhenColumnIsReadyOnly_Throws()
		{
			Assert.Throws<UpdateByQueryException>(() => new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b.Set(x => x.Id, 3),
					q => q.In(x => x.Id, new[] {1L, 2, 3}),
					new List<object>()
				)
			);
		}
	}
}