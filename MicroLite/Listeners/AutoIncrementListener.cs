﻿// -----------------------------------------------------------------------
// <copyright file="AutoIncrementListener.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    using System;
    using System.Globalization;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="IListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.AutoIncrement is used.
    /// </summary>
    public sealed class AutoIncrementListener : Listener
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.AutoIncrementListener");

        /// <summary>
        /// Invoked after the SqlQuery to insert the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result.</param>
        public override void AfterInsert(object instance, object executeScalarResult)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (executeScalarResult == null)
            {
                throw new ArgumentNullException("executeScalarResult");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.AutoIncrement)
            {
                var propertyInfo = objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

                var identifierValue = Convert.ChangeType(executeScalarResult, propertyInfo.PropertyType, CultureInfo.InvariantCulture);

                log.TryLogDebug(Messages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, identifierValue.ToString());
                propertyInfo.SetValue(instance, identifierValue, null);
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to delete the record from the database is created.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has not been set.</exception>
        public override void BeforeDelete(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.AutoIncrement)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierNotSetForDelete);
                }
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has already been set.</exception>
        public override void BeforeInsert(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.AutoIncrement)
            {
                if (!objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierSetForInsert);
                }
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is executed.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        /// <param name="sqlQuery">The SqlQuery to be executed.</param>
        public override void BeforeInsert(object instance, SqlQuery sqlQuery)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.AutoIncrement)
            {
                sqlQuery.CommandText += ";SELECT last_insert_rowid()";
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has not been set.</exception>
        public override void BeforeUpdate(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.AutoIncrement)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}