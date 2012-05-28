﻿namespace MicroLite.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlQueryBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class SqlQueryBuilderTests
    {
        [Test]
        public void SelectFrom()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByAscending()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .OrderByAscending("Column1")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n ORDER BY Column1 ASC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromOrderByDescending()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .OrderByDescending("Column1")
                .ToSqlQuery();

            CollectionAssert.IsEmpty(sqlQuery.Arguments);
            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n ORDER BY Column1 DESC", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhere()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .ToSqlQuery();

            Assert.AreEqual(1, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhereAnd()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .AndWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)\r\n AND (Column2 = @p1)", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhereComplex()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2", "Column3")
                .From("Table")
                .Where("Column1 = @p0 OR @p0 IS NULL", "Foo")
                .AndWhere("Column2 BETWEEN @p0 AND @p1", DateTime.Today.AddDays(-1), DateTime.Today)
                .OrWhere("Column3 IN (@p0, @p1, @p2, @p3)", 1, 2, 3, 4)
                .ToSqlQuery();

            Assert.AreEqual(7, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual(DateTime.Today.AddDays(-1), sqlQuery.Arguments[1]);
            Assert.AreEqual(DateTime.Today, sqlQuery.Arguments[2]);
            Assert.AreEqual(1, sqlQuery.Arguments[3]);
            Assert.AreEqual(2, sqlQuery.Arguments[4]);
            Assert.AreEqual(3, sqlQuery.Arguments[5]);
            Assert.AreEqual(4, sqlQuery.Arguments[6]);

            Assert.AreEqual("SELECT Column1, Column2, Column3\r\n FROM Table\r\n WHERE (Column1 = @p0 OR @p0 IS NULL)\r\n AND (Column2 BETWEEN @p1 AND @p2)\r\n OR (Column3 IN (@p3, @p4, @p5, @p6))", sqlQuery.CommandText);
        }

        [Test]
        public void SelectFromWhereOr()
        {
            var sqlQuery = SqlQueryBuilder.Select("Column1", "Column2")
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .OrWhere("Column2 = @p0", "Bar")
                .ToSqlQuery();

            Assert.AreEqual(2, sqlQuery.Arguments.Count);
            Assert.AreEqual("Foo", sqlQuery.Arguments[0]);
            Assert.AreEqual("Bar", sqlQuery.Arguments[1]);

            Assert.AreEqual("SELECT Column1, Column2\r\n FROM Table\r\n WHERE (Column1 = @p0)\r\n OR (Column2 = @p1)", sqlQuery.CommandText);
        }
    }
}