using _20104681JoshMkhariCLDV6212Task2.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Windows;

namespace _20104681JoshMkhariCLDV6212Task2.BlobHandler
{
    public class BlobManager
    {
        //connection to container 
        private static BlobContainerClient theContainer { get; set; }
        public BlobManager(string ContainerName)
        {
            //check if the container name is empty or null
            if (string.IsNullOrEmpty(ContainerName))
            {
                throw new ArgumentNullException("Container", "Container name connot be empty");
            }
            try
            {
                //get azure storage account conneciton string
                string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=abscupermarketsanj;AccountKey=Fp7rUpD+Rph1Hx9vWr4IlpegRh4mex4R91D3W9qBEIQALBsqY19msS+NbOr+D1K9OPF48mxM0m1renfnGWApCg==;EndpointSuffix=core.windows.net";


                //create container if not exists
                BlobContainerClient container = new BlobContainerClient(ConnectionString, ContainerName);
                container.CreateIfNotExists();
                theContainer = container;
                theContainer.SetAccessPolicy(PublicAccessType.Blob);
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Upload Blob Insert or Update/Replace C U
        public string UploadFile(HttpPostedFileBase uploadFile, String blobName)
        {
            string AbsoluteUri;
            if (uploadFile == null)
            {
                //Ensure not to delete the current blob
                ForFilePath.edit = true;
                return null;
            }
            try
            {
                //Retrieve the file path of wht is to be uploaded and store it as a BlobClient
                BlobClient myBlob = theContainer.GetBlobClient(ForFilePath.currentBlobName);

                //upload blob
                myBlob.Upload(uploadFile.InputStream);
                //Assign URI
                AbsoluteUri = myBlob.Uri.AbsoluteUri;
            }
            catch (Exception)
            {
                //Ensure URI has an empty value
                ForFilePath.theFilePath = " ";
                AbsoluteUri = ForFilePath.theFilePath; ;
            }
            return AbsoluteUri;
        }

        //Count all blobs and return count, Used to assign unique number to each blobName
        public int CountBlobList()
        {
            int count = 0;
            List<string> _blobList = new List<string>();
            foreach (BlobItem blob in theContainer.GetBlobs())
            {
                count++;
            }
            return count;
        }


        //Delete Blob
        public void DeleteBlob(string blobName)
        {
            try
            {
                //Compare each blob within the container
                foreach (BlobItem blob in theContainer.GetBlobs())
                {
                    //delete the correct blob
                    if (blob.Name.Equals(blobName))
                    {
                        theContainer.DeleteBlob(blob.Name);
                        break;
                    }

                }
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }

        }
    }
}