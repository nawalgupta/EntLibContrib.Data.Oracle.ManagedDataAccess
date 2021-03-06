﻿//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace EntLibContrib.Data.TestSupport
{
    public class ExecuteScalarFixture
    {
        DbCommand command;
        Database db;

        public ExecuteScalarFixture(Database db,
                                    DbCommand command)
        {
            this.db = db;
            this.command = command;
        }

        public void CanExecuteScalarDoAnInsertion()
        {
            string insertCommand = "Insert into Region values (99, 'Midwest')";
            DbCommand command = db.GetSqlStringCommand(insertCommand);
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                using (RollbackTransactionWrapper transaction = new RollbackTransactionWrapper(connection.BeginTransaction()))
                {
                    db.ExecuteScalar(command, transaction.Transaction);

                    DbCommand rowCountCommand = db.GetSqlStringCommand("select count(*) from Region");
                    int count = Convert.ToInt32(db.ExecuteScalar(rowCountCommand, transaction.Transaction));
                    Assert.AreEqual(5, count);
                }
            }
        }

        public void ExecuteScalarWithCommandTextAndTypeInTransaction()
        {
            int count = -1;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    count = Convert.ToInt32(db.ExecuteScalar(transaction, command.CommandType, command.CommandText));
                    transaction.Commit();
                }
            }

            Assert.AreEqual(4, count);
        }

        public void ExecuteScalarWithIDbCommand()
        {
            int count = Convert.ToInt32(db.ExecuteScalar(command));

            Assert.AreEqual(4, count);
        }

        public void ExecuteScalarWithIDbTransaction()
        {
            int count = -1;
            using (DbConnection connection = db.CreateConnection())
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    count = Convert.ToInt32(db.ExecuteScalar(command, transaction));
                    transaction.Commit();
                }
            }

            Assert.AreEqual(4, count);
        }

        public void ExecuteScalarWithNullIDbCommand()
        {
            int count = Convert.ToInt32(db.ExecuteScalar((DbCommand)null));
        }

        public void ExecuteScalarWithNullIDbCommandAndNullTransaction()
        {
            int count = Convert.ToInt32(db.ExecuteScalar(null, (string)null));
        }

        public void ExecuteScalarWithNullIDbTransaction()
        {
            int count = Convert.ToInt32(db.ExecuteScalar(command, null));
        }
    }
}
