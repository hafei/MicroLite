﻿// -----------------------------------------------------------------------
// <copyright file="ISqlDialect.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Dialect
{
    using System;
    using System.Data;

    /// <summary>
    /// The interface for a class which builds an <see cref="SqlQuery"/> for a object instance.
    /// </summary>
    internal interface ISqlDialect
    {
        /// <summary>
        /// Creates an SqlQuery to count the number of records which would be returned by the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>An <see cref="SqlQuery"/> to count the number of records which would be returned by the specified SqlQuery.</returns>
        SqlQuery CountQuery(SqlQuery sqlQuery);

        /// <summary>
        /// Creates an SqlQuery with the specified statement type for the specified instance.
        /// </summary>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="instance">The instance to generate the SqlQuery for.</param>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown if the statement type is not supported.</exception>
        SqlQuery CreateQuery(StatementType statementType, object instance);

        /// <summary>
        /// Creates an SqlQuery with the specified statement type for the specified type and identifier.
        /// </summary>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="forType">The type of object to create the query for.</param>
        /// <param name="identifier">The identifier of the instance to create the query for.</param>
        /// <returns>The created <see cref="SqlQuery" />.</returns>
        /// <exception cref="NotSupportedException">Thrown if the statement type is not supported.</exception>
        SqlQuery CreateQuery(StatementType statementType, Type forType, object identifier);

        /// <summary>
        /// Creates an SqlQuery to page the records which would be returned by the specified SqlQuery based upon the page number and number of results per page.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="page">The page number to get the results for.</param>
        /// <param name="resultsPerPage">The number of results to be shown per page.</param>
        /// <returns>A <see cref="SqlQuery"/> to return the paged results of the specified query.</returns>
        SqlQuery PageQuery(SqlQuery sqlQuery, long page, long resultsPerPage);
    }
}