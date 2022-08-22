using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace _20104681JoshMkhariCLDV6212Task2.TableHandler
{
    public class TableManager
    {
        //connection to storage account
        private CloudTable table;

        public TableManager(string _CloudTablename)
        {
            //check if the table name is empty or null
            if (string.IsNullOrEmpty(_CloudTablename))
            {
                throw new ArgumentNullException("Table", "Table name connot be empty");
            }
            try
            {
                //get azure storage account conneciton string
                string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=abscupermarketsanj;AccountKey=Fp7rUpD+Rph1Hx9vWr4IlpegRh4mex4R91D3W9qBEIQALBsqY19msS+NbOr+D1K9OPF48mxM0m1renfnGWApCg==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

                //create table if not exists
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                table = tableClient.GetTableReference(_CloudTablename);
                table.CreateIfNotExists();
            }
            catch (StorageException StorageExceptionObj)
            {
                throw StorageExceptionObj;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //retrieve all product GET R
        public List<T> RetrieveEntity<T>(String Query = null) where T : TableEntity, new()
        {
            try
            {
                TableQuery<T> DataTableQuery = new TableQuery<T>();
                if (!string.IsNullOrEmpty(Query))
                {
                    DataTableQuery = new TableQuery<T>().Where(Query);
                }
                IEnumerable<T> IDataList = table.ExecuteQuery(DataTableQuery);
                List<T> DataList = new List<T>();
                foreach (var singleData in IDataList)
                    DataList.Add(singleData);
                return DataList;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Insert or Update/Replace C U
        public void InsertEntity<T>(T entity, bool forInsert = true) where T : TableEntity, new()
        {
            try
            {
                if (forInsert)
                {
                    //Insert a new entity
                    var InsertOperation = TableOperation.Insert(entity);
                    table.Execute(InsertOperation);
                }
                else
                {
                    //Update an entity
                    var InsertOrReplaceOperation = TableOperation.InsertOrReplace(entity);
                    table.Execute(InsertOrReplaceOperation);
                }
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Delete product D
        public bool DeleteEntity<T>(T entity) where T : TableEntity, new()
        {
            try
            {
                var DeleteOperation = TableOperation.Delete(entity);
                table.Execute(DeleteOperation);
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }
    }
}