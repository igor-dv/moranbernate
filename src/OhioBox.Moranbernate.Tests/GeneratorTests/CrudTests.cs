﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;
using OhioBox.Moranbernate.Querying;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	[TestFixture]
	public class CrudTests
	{
		[Test]
		public void DeleteByQuery()
		{
			var deleteByQuery = new DeleteByQuery<CompositeIdSample>();
			var parameters = new List<object>();
			var sql = deleteByQuery.GetSql(w => w.Equal(x => x.Id1, 5), parameters);

			Assert.AreEqual("DELETE FROM `composite_id_table` WHERE (`Id1` = ?p0);", sql);
			Assert.AreEqual(5, parameters[0]);
		}

		[Test]
		public void Insert_Composite()
		{
			var generator = new InsertGenerator<CompositeIdSample>();
			Assert.AreEqual("INSERT INTO `composite_id_table`(`Id1`, `Id2`, `SomeValue`, `SomeDate`) VALUES (?p0, ?p1, ?p2, ?p3);", generator.GetSql());
		}

		[Test]
		public void Insert_Simple()
		{
			var generator = new InsertGenerator<SimpleObject>();
			Console.WriteLine(generator.GetSql());
			Assert.AreEqual("INSERT INTO `table_name`(`LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum`) VALUES (?p0, ?p1, ?p2, ?p3, ?p4, ?p5, ?p6); SELECT LAST_INSERT_ID();", generator.GetSql());
		}

		[Test]
		public void Update()
		{
			var generator = new UpdateGenerator<CompositeIdSample>();
			Assert.AreEqual("UPDATE `composite_id_table` SET `SomeValue` = ?p0, `SomeDate` = ?p1 WHERE `Id1` = ?p2 AND `Id2` = ?p3;", generator.GetSql());
		}

		[Test]
		public void Delete()
		{
			var generator = new DeleteGenerator<CompositeIdSample>();
			Assert.AreEqual("DELETE FROM `composite_id_table` WHERE `Id1` = ?p0 AND `Id2` = ?p1;", generator.GetSql());
		}

		[Test]
		public void Upsert_Composite()
		{
			var generator = new UpsertGenerator<CompositeIdSample>();
			Console.WriteLine(generator.GetSql());
			Assert.AreEqual("INSERT INTO `composite_id_table`(`Id1`, `Id2`, `SomeValue`, `SomeDate`) VALUES (?p0, ?p1, ?p2, ?p3) ON DUPLICATE KEY UPDATE `SomeValue` = ?p2, `SomeDate` = ?p3;", generator.GetSql());
		}

		[Test]
		public void Upsert()
		{
			var generator = new UpsertGenerator<SimpleObject>();
			Assert.Throws<UpsertNotSupportedOnAutoGeneratedIdTypesException>(() => generator.GetSql());
		}

		[Test]
		public void PartialUpdate()
		{
			var generator = new PartialUpdateGenerator<SimpleObject>(new Expression<Func<SimpleObject, object>>[] {
				 x => x.Long
			});
			Assert.AreEqual("UPDATE `table_name` SET `LongColumnName` = ?p0 WHERE `Id` = ?p1;", generator.GetSql());
		}

		[Test]
		public void CountByQuery()
		{
			var countByQuery = new CountByQuery<CompositeIdSample>();
			var parameters = new List<object>();
			var sql = countByQuery.GetSql(w => w.Equal(x => x.Id1, 5), parameters);

			Assert.AreEqual("SELECT COUNT(*) FROM `composite_id_table` WHERE (`Id1` = ?p0);", sql);
			Assert.AreEqual(5, parameters[0]);
		}

		[Test]
		public void CountByQuery_NoQuery()
		{
			var countByQuery = new CountByQuery<CompositeIdSample>();
			var parameters = new List<object>();
			var sql = countByQuery.GetSql(null, parameters);

			Assert.AreEqual("SELECT COUNT(*) FROM `composite_id_table`;", sql);
		}
	}
}