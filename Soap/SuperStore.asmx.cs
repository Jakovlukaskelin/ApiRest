using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace Soap
{
    /// <summary>
    /// Summary description for SuperStore
    /// </summary>
    [WebService(Namespace = "http://localhost:44328/SuperStore.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SuperStore : WebService
    {

        [WebMethod]
        public string GetSuperStoreInfo(string superStore)
        {
            var solutionDirectoryPath = FindSolutionPath();
            string xmlFilePath = Path.Combine((string)solutionDirectoryPath, "SuperStoreXML.xml");
            string superStoreSearch = Path.Combine((string)solutionDirectoryPath, "SuperStoreSearch.xml");
            // trazi orginal xml i stavi u search ako i posoji overwirte
            File.Copy(xmlFilePath, superStoreSearch, true);

            XmlDocument doc = new XmlDocument();
            doc.Load(superStoreSearch);

            //Xpath usage
            XmlNode node = doc.SelectSingleNode($"/Orders/Order[RowID='{superStore}']");
            if (node != null)
            {
                string id = node.SelectSingleNode("RowID")?.InnerText;
                string orderId = node.SelectSingleNode("OrderId")?.InnerText;
                string orderDate = node.SelectSingleNode("OrderDate")?.InnerText;
                string shipDate = node.SelectSingleNode("ShipDate")?.InnerText;
                string shipMode = node.SelectSingleNode("ShipMode")?.InnerText;
                string customerId = node.SelectSingleNode("CustomerId")?.InnerText;
                string segment = node.SelectSingleNode("Segment")?.InnerText;
                string customerName = node.SelectSingleNode("CustomerName")?.InnerText;
                string country = node.SelectSingleNode("Country")?.InnerText;
                string city = node.SelectSingleNode("City")?.InnerText;
                string state = node.SelectSingleNode("State")?.InnerText;
                string postCode = node.SelectSingleNode("PostCode")?.InnerText;
                string region = node.SelectSingleNode("Region")?.InnerText;
                string productId = node.SelectSingleNode("ProductId")?.InnerText;
                string category = node.SelectSingleNode("Category")?.InnerText;
                string subCategory = node.SelectSingleNode("SubCategory")?.InnerText;
                string productName = node.SelectSingleNode("ProductName")?.InnerText;
                string sales = node.SelectSingleNode("Sales")?.InnerText;
                string quantity = node.SelectSingleNode("Quantity")?.InnerText;
                string discount = node.SelectSingleNode("Discount")?.InnerText;
                string profit = node.SelectSingleNode("Profit")?.InnerText;


                // handle null values
                id = id ?? "Unknown";
                orderId = orderId ?? "Unknown";
                orderDate = orderDate ?? "Unknown";
                shipDate = shipDate ?? "Unknown";
                shipMode = shipMode ?? "Unknown";
                customerId = customerId ?? "Unknown";
                segment = segment ?? "Unknown";
                customerName = customerName ?? "Unknown";
                country = country ?? "Unknown";
                city = city ?? "Unknown";
                state = state ?? "Unknown";
                postCode = postCode ?? "Unknown";
                region = region ?? "Unknown";
                productId = productId ?? "Unknown";
                category = category ?? "Unknown";
                subCategory = subCategory ?? "Unknown";
                productName = productName ?? "Unknown";
                sales = sales ?? "Unknown";
                quantity = quantity ?? "Unknown";
                discount = discount ?? "Unknown";
                profit = profit ?? "Unknown";

                return $"Row Id: {id} \nOrder Id: {orderId} \nOrder Date: {orderDate}\nship Date: {shipDate}\nShipMode: {shipMode}\nCustomer Id: {customerId}" +
                    $"\nsegment: {segment}\nCustomer Name: {customerName}\nCountry: {country}\nCity: {city}\nState: {state}\nPost Code: {postCode}" +
                    $"\nRegion: {region}\nProduct Id: {productId}\nCategory: {category}\nSubCategory: {subCategory}\nProduct Name: {productName}" +
                    $"\nSales: {sales}\nQuantity: {quantity}\nDiscount: {discount}\nProfit: {profit} ";
            }

            throw new Exception("SuperStore not found.");

        }

        private object FindSolutionPath()
        {
            string solutionDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; // Gets the bin/debug directory path
            for (int i = 0; i < 2; i++)
            {
                var directoryInfo = Directory.GetParent(solutionDirectoryPath);
                if (directoryInfo != null)
                {
                    solutionDirectoryPath = directoryInfo.FullName;
                }
                else
                {
                    throw new Exception($"Could not find parent directory at level {i}");
                }
            }
            return solutionDirectoryPath;
        }
    }
}
