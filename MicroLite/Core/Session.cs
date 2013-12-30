﻿// -----------------------------------------------------------------------
// <copyright file="Session.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Core
{
    using System;
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.Query;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="ISession"/>.
    /// </summary>
    internal sealed class Session : ReadOnlySession, ISession, IAdvancedSession
    {
        private readonly IListener[] listeners;

        internal Session(
            ISessionFactory sessionFactory,
            IConnectionManager connectionManager,
            IObjectBuilder objectBuilder,
            IListener[] listeners)
            : base(sessionFactory, connectionManager, objectBuilder)
        {
            this.listeners = listeners;
        }

        public new IAdvancedSession Advanced
        {
            get
            {
                return this;
            }
        }

        public bool Delete(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.listeners.Each(l => l.BeforeDelete(instance));

            var objectInfo = ObjectInfo.For(instance.GetType());
            var identifier = objectInfo.GetIdentifierValue(instance);

            var sqlBuilder = new DeleteSqlBuilder(this.SqlDialect.SqlCharacters)
                .From(objectInfo.ForType)
                .WhereEquals(objectInfo.TableInfo.IdentifierColumn, identifier);

            var sqlQuery = sqlBuilder.ToSqlQuery();

            this.listeners.Each(l => l.BeforeDelete(instance, sqlQuery));

            var rowsAffected = this.Execute(sqlQuery);

            this.listeners.Reverse(l => l.AfterDelete(instance, rowsAffected));

            return rowsAffected == 1;
        }

        public bool Delete(Type type, object identifier)
        {
            this.ThrowIfDisposed();

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var objectInfo = ObjectInfo.For(type);

            var sqlBuilder = new DeleteSqlBuilder(this.SqlDialect.SqlCharacters)
                .From(type)
                .WhereEquals(objectInfo.TableInfo.IdentifierColumn, identifier);

            var sqlQuery = sqlBuilder.ToSqlQuery();

            var rowsAffected = this.Execute(sqlQuery);

            return rowsAffected == 1;
        }

        public int Execute(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                using (var command = this.ConnectionManager.CreateCommand())
                {
                    this.SqlDialect.BuildCommand(command, sqlQuery);

                    var result = command.ExecuteNonQuery();

                    this.ConnectionManager.CommandCompleted(command);

                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        public T ExecuteScalar<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                using (var command = this.ConnectionManager.CreateCommand())
                {
                    this.SqlDialect.BuildCommand(command, sqlQuery);

                    var result = command.ExecuteScalar();

                    this.ConnectionManager.CommandCompleted(command);

                    var resultType = typeof(T);
                    var typeConverter = TypeConverter.For(resultType);
                    var converted = (T)typeConverter.ConvertFromDbValue(result, resultType);

                    return converted;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        public void Insert(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.listeners.Each(l => l.BeforeInsert(instance));

            var sqlQuery = this.SqlDialect.CreateQuery(StatementType.Insert, instance);

            this.listeners.Each(l => l.BeforeInsert(instance, sqlQuery));

            var identifier = this.ExecuteScalar<object>(sqlQuery);

            this.listeners.Reverse(l => l.AfterInsert(instance, identifier));
        }

        public void InsertOrUpdate(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.HasDefaultIdentifierValue(instance))
            {
                this.Insert(instance);
            }
            else
            {
                this.Update(instance);
            }
        }

        public bool Update(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.listeners.Each(l => l.BeforeUpdate(instance));

            var sqlQuery = this.SqlDialect.CreateQuery(StatementType.Update, instance);

            this.listeners.Each(l => l.BeforeUpdate(instance, sqlQuery));

            var rowsAffected = this.Execute(sqlQuery);

            this.listeners.Reverse(l => l.AfterUpdate(instance, rowsAffected));

            return rowsAffected == 1;
        }

        public bool Update(ObjectDelta objectDelta)
        {
            this.ThrowIfDisposed();

            if (objectDelta == null)
            {
                throw new ArgumentNullException("objectDelta");
            }

            var objectInfo = ObjectInfo.For(objectDelta.ForType);

            var builder = new UpdateSqlBuilder(this.SqlDialect.SqlCharacters).Table(objectDelta.ForType);

            foreach (var change in objectDelta.Changes)
            {
                builder.SetColumnValue(change.Key, change.Value);
            }

            builder.Where(objectInfo.TableInfo.IdentifierColumn, objectDelta.Identifier);

            var sqlQuery = builder.ToSqlQuery();

            var rowsAffected = this.Execute(sqlQuery);

            return rowsAffected == 1;
        }
    }
}