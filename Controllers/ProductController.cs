using _20104681JoshMkhariCLDV6212Task2.BlobHandler;
using _20104681JoshMkhariCLDV6212Task2.Models;
using _20104681JoshMkhariCLDV6212Task2.TableHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace _20104681JoshMkhariCLDV6212Task2.Controllers
{
    public class ProductController : Controller
    {
        //For get product
        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))//If the user is updating a product
            {
                //set the name of the table for table storage
                TableManager TableManagerObj = new TableManager("product");

                //retrieve the product object that matches the row key
                List<Product> ProductListObj = TableManagerObj.RetrieveEntity<Product>("RowKey eq '" + id + "'");

                Product ProductObj = ProductListObj.FirstOrDefault();

                //To alert the other methods that the current objective is to update a product
                ForFilePath.edit = true;
                //For keeping the image file path in the event of an edit without changing
                ForFilePath.theFilePath = ProductObj.FilePath;
                //For keeping theblob name in the event of an update that changes the Image in use
                ForFilePath.theOldBlobName = ProductObj.BlobName;

                return View(ProductObj);
            }
            //To alert the other methods that the current objective is to add a new product
            ForFilePath.edit = false;
            return View(new Product());
        }

        //For insert Product
        [HttpPost]
        public ActionResult Index(string id, HttpPostedFileBase uploadFile, FormCollection formData)
        {
            //To store whether a user uploaded a file or not
            Boolean wasAFileUploaded = uploadFile != null; 

            //Storing all the Product values
            Product ProductObj = new Product();
            ProductObj.ItemName = formData["ItemName"] == "" ? null : formData["ItemName"];
            ProductObj.ItemDescription = formData["ItemDescription"] == "" ? null : formData["ItemDescription"];
            double itemPrice;
            if (double.TryParse(formData["ItemPrice"], out itemPrice))
            {
                ProductObj.ItemPrice = double.Parse(formData["ItemPrice"] == "" ? null : formData["ItemPrice"]);
            }
            else
            {
                //Warn the user that they must use a comma
                //MessageBox.Show("Use a ',' instead of a '.' for a value eg: 995,60");
                ForFilePath.edit = false;
                return View(new Product());
            }


            //upload the product picture to get the URI
            foreach (string file in Request.Files)
            {
                uploadFile = Request.Files[file];
            }

            //create the blob container
            BlobManager BlobManagerObj = new BlobManager("productpictures");


            //Check if a upladFile is valid
            if (uploadFile.FileName.Length > 0)
            {
                //Update blob name
                string blobName = uploadFile.FileName;
                //Store blob name with unique identifier to avoid an error that forbids duplicate blobs
                ProductObj.BlobName = blobName + (BlobManagerObj.CountBlobList() + 1);
            }
            else
            {
                ProductObj.BlobName = ForFilePath.theOldBlobName;
            }
            ForFilePath.currentBlobName = ProductObj.BlobName;

            string fileAbsoluteUri;
            if (ForFilePath.edit && wasAFileUploaded)//if an item is being updated and an image was uploaded 
            {
                //First delete the old blob as it will be replaced
                BlobManagerObj.DeleteBlob(ForFilePath.theOldBlobName);
                //update the URI for the new blobS
                fileAbsoluteUri = BlobManagerObj.UploadFile(uploadFile, ForFilePath.currentBlobName);
                ProductObj.FilePath = fileAbsoluteUri.ToString();


            }
            else
            {
                if (ForFilePath.edit && !wasAFileUploaded)//if an item is being eddited and an image was not uploaded
                {
                    ProductObj.FilePath = ForFilePath.theFilePath;// keep old URI path
                }
                else
                {
                    if (!ForFilePath.edit && wasAFileUploaded) //if an item is not being edited and an image is uploaded
                    {
                        //Upload a blob
                        fileAbsoluteUri = BlobManagerObj.UploadFile(uploadFile, ForFilePath.currentBlobName);
                        ProductObj.FilePath = fileAbsoluteUri.ToString();
                    }
                    else
                    {
                        //Alert user that they did not select an image
                        ForFilePath.edit = false;
                        //MessageBox.Show("No image selected!!");
                        return RedirectToAction("Get");
                    }
                }
            }

            //Insert into table
            if (string.IsNullOrEmpty(id))
            {
                ProductObj.PartitionKey = ProductObj.ItemDescription;
                ProductObj.RowKey = Guid.NewGuid().ToString();

                TableManager TableManagerObj = new TableManager("product");
                TableManagerObj.InsertEntity<Product>(ProductObj, true);
            }
            //update
            else
            {
                //First delete the old table element as the item description may match, also delete the blob item
                Delete(id);

                //Then update the table, the blob has already been inserted
                ProductObj.PartitionKey = ProductObj.ItemDescription;
                ProductObj.RowKey = id;

                TableManager TableManagerObj = new TableManager("product");
                TableManagerObj.InsertEntity<Product>(ProductObj, false);

            }
            //Reset ForFilePath elements
            ForFilePath.theFilePath = null;
            ForFilePath.edit = false;

            //Store currently used Blobname
            ForFilePath.theOldBlobName = ProductObj.BlobName;
            return RedirectToAction("Get");
        }

        //get Product list
        public ActionResult Get()
        {
            TableManager TableManagerObj = new TableManager("product");

            //retrieve the Product object that matches the row key
            List<Product> ProductListObj = TableManagerObj.RetrieveEntity<Product>(null);
            return View(ProductListObj);
        }

        //delete Product
        public ActionResult Delete(string id)
        {
            //retrieve product to be deleted
            TableManager TableManagerObj = new TableManager("product");

            //retrieve the product object that matches the row key
            List<Product> ProductListObj = TableManagerObj.RetrieveEntity<Product>("RowKey eq '" + id + "'");
            Product ProductObj = ProductListObj.FirstOrDefault();

            //If the user is not updating a product, Delete the corresponding blob item
            if (!ForFilePath.edit)
            {
                BlobManager BlobManagerObj = new BlobManager("productpictures");
                BlobManagerObj.DeleteBlob(ProductObj.BlobName);
            }
            //Reset edit parameter
            ForFilePath.edit = false;
            //Delete the product
            TableManagerObj.DeleteEntity<Product>(ProductObj);
            return RedirectToAction("Get");
        }
    }
}