using Microsoft.Azure.Cosmos.Table;
using System;

namespace _20104681JoshMkhariCLDV6212Task2.Models
{
    public class Product : TableEntity
    {
        public Product() { } //Model Class

        public string FilePath { get; set; } //Stores the Absoulute URI for the product 
        public string ItemName { get; set; } //Stores the Product Name for the product

        public string ItemDescription { get; set; } //Stores the Description for the product and is also a Partition key

        public Double ItemPrice { get; set; } //Stores the Product Price for the product

        public string BlobName { get; set; } //Stores the name of the Blob associated with the product
    }

    public class ForFilePath //Used to keep track of objects that need to be used without being cleared
    {
        public static Boolean edit { get; set; }//To determine if the user is adding a new product or updating one
        public static String theFilePath { get; set; } // Stores the Absoulute URI for the product being updated
        public static String theOldBlobName { get; set; }// Stores the blobName for the blob that was previously updated

        public static String currentBlobName { get; set; }// Stores the blobName for the blob being updated

    }
}