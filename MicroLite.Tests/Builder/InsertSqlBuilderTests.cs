﻿namespace MicroLite.Tests.Builder
{
    using System;
    using MicroLite.Builder;
    using MicroLite.Dialect.MsSql;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="InsertSqlBuilder"/> class.
    /// </summary>
    public class InsertSqlBuilderTests : IDisposable
    {
        public InsertSqlBuilderTests()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(UnitTestConfig.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
        }

        public void Dispose()
        {
            ObjectInfo.MappingConvention = null;
        }

        [Fact]
        public void InsertIntoSpecifyingTableName()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO Table () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertIntoSpecifyingTableNameWithSqlCharacters()
        {
            var sqlBuilder = new InsertSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO [Table] () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertIntoSpecifyingType()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Into(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO Sales.Customers () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertIntoSpecifyingTypeWithSqlCharacters()
        {
            var sqlBuilder = new InsertSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Into(typeof(Customer))
                .ToSqlQuery();

            Assert.Empty(sqlQuery.Arguments);
            Assert.Equal("INSERT INTO [Sales].[Customers] () VALUES ()", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertIntoValues()
        {
            var sqlBuilder = new InsertSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .Value("Column1", "Foo")
                .Value("Column2", 12)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);

            Assert.Equal("INSERT INTO Table (Column1, Column2) VALUES (?, ?)", sqlQuery.CommandText);
        }

        [Fact]
        public void InsertIntoValuesWithSqlCharacters()
        {
            var sqlBuilder = new InsertSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .Into("Table")
                .Value("Column1", "Foo")
                .Value("Column2", 12)
                .ToSqlQuery();

            Assert.Equal(2, sqlQuery.Arguments.Count);
            Assert.Equal("Foo", sqlQuery.Arguments[0]);
            Assert.Equal(12, sqlQuery.Arguments[1]);

            Assert.Equal("INSERT INTO [Table] ([Column1], [Column2]) VALUES (@p0, @p1)", sqlQuery.CommandText);
        }
    }
}