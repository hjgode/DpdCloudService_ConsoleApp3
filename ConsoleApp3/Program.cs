using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            MyDPDCloudService myDPDCloudService = new MyDPDCloudService();
            //MyDPDCloudService.doTest();

            DPDCloudService.AddressType address = MyDPDCloudService.MyAddress("Hellweg Baumarkt", "Musterstrasse", "21b", "12345", "Musterstadt");

            DPDCloudService.ParcelDataType parcelData = MyDPDCloudService.MyParcelData("Filiale 123", "123/1", 19m, "Filaile 123/1");
            DPDCloudService.ParcelDataType parcelData2 = MyDPDCloudService.MyParcelData("Filiale 123", "123/2", 18m, "Filaile 123/2");

            string sParcelNo = "";
            string sErr = "";

            List<DPDCloudService.ParcelDataType> parcelDataList = new List<DPDCloudService.ParcelDataType>();
            parcelDataList.Add(parcelData);
            parcelDataList.Add(parcelData2);

            if (MyDPDCloudService.setOrter(address, parcelDataList.ToArray(), ref sParcelNo, ref sErr))
            {
                //success
                System.Diagnostics.Debug.WriteLine("Success: see " + sParcelNo + ".pdf");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ERR: " + sErr);
            }
        }

        void test()
        {
            DPDCloudService.setOrderRequestType mySetOrderRequest = new DPDCloudService.setOrderRequestType();
            DPDCloudService.OrderDataType myOrderData = new DPDCloudService.OrderDataType();
            DPDCloudService.OrderDataType[] myOrderDataList = new DPDCloudService.OrderDataType[1];

            mySetOrderRequest.Version = 100;
            mySetOrderRequest.Language = "de_DE";

            mySetOrderRequest.PartnerCredentials = new DPDCloudService.PartnerCredentialType();
            mySetOrderRequest.PartnerCredentials.Name = "DPD Cloud Service Alpha2";
            mySetOrderRequest.PartnerCredentials.Token = "33879594E70436D58685";

            mySetOrderRequest.UserCredentials = new DPDCloudService.UserCredentialType();
            mySetOrderRequest.UserCredentials.cloudUserID = 5697191;
            mySetOrderRequest.UserCredentials.Token = "D6C6865595A6F6365516";

            mySetOrderRequest.OrderAction = DPDCloudService.OrderActionType.startOrder;
            mySetOrderRequest.OrderSettings = new DPDCloudService.OrderSettingsType();
            mySetOrderRequest.OrderSettings.LabelSize = DPDCloudService.LabelSizeType.PDF_A6;
            mySetOrderRequest.OrderSettings.LabelStartPosition = DPDCloudService.LabelStartPositionType.UpperLeft;
            mySetOrderRequest.OrderSettings.ShipDate = Convert.ToDateTime("10.11.2023");

            myOrderData.ShipAddress = new DPDCloudService.AddressType();
            myOrderData.ShipAddress.Gender = "none";
            myOrderData.ShipAddress.Salutation = "";
            myOrderData.ShipAddress.FirstName = "";
            myOrderData.ShipAddress.LastName = "";
            myOrderData.ShipAddress.Company = "Hellweg die Profi-Baumärkte";
            myOrderData.ShipAddress.Name = "Marktleitung 999";
            myOrderData.ShipAddress.Street = "Wailandtstr.";
            myOrderData.ShipAddress.HouseNo = "1";
            myOrderData.ShipAddress.ZipCode = "63741";
            myOrderData.ShipAddress.City = "Aschaffenburg";
            myOrderData.ShipAddress.Country = "DEU";
            myOrderData.ShipAddress.State = "";
            myOrderData.ShipAddress.Phone = "+49 2154 88866 50";
            myOrderData.ShipAddress.Mail = "heinz-josef.gode@lodata.de";

            myOrderData.ParcelData = new DPDCloudService.ParcelDataType();
            myOrderData.ParcelData.YourInternalID = "Filiale xyz";
            myOrderData.ParcelData.Content = "Smartphone";
            myOrderData.ParcelData.Weight = 13.5m;
            myOrderData.ParcelData.Reference1 = "Filiale 123";
            myOrderData.ParcelData.Reference2 = "";
            myOrderData.ParcelData.ShipService = new DPDCloudService.ShipServiceType();
            myOrderData.ParcelData.ShipService = DPDCloudService.ShipServiceType.Classic;

            myOrderDataList[0] = myOrderData;
            mySetOrderRequest.OrderDataList = myOrderDataList;

            DPDCloudService.DPDCloudService_v1SoapClient myApiSoapClient = 
                new DPDCloudService.DPDCloudService_v1SoapClient("staging");
            DPDCloudService.setOrderResponseType mySetOrderResponse = new DPDCloudService.setOrderResponseType();

            mySetOrderResponse = myApiSoapClient.setOrder(mySetOrderRequest);

            if (mySetOrderResponse.Ack)
            {
                System.Diagnostics.Debug.WriteLine(mySetOrderResponse.LabelResponse.ToString());

                string parcelNo = mySetOrderResponse.LabelResponse.LabelDataList[0].ParcelNo;
                string intID = mySetOrderResponse.LabelResponse.LabelDataList[0].YourInternalID;

                foreach (DPDCloudService.LabelDataType data in mySetOrderResponse.LabelResponse.LabelDataList)
                {
                    parcelNo = data.ParcelNo;
                    intID = data.YourInternalID;
                    System.Diagnostics.Debug.WriteLine("Response data: " + parcelNo + "/" + intID);
                    Console.WriteLine("Response data: " + parcelNo + "/" + intID);
                }
                try
                {
                    Byte[] Base64Byte = System.Convert.FromBase64String(mySetOrderResponse.LabelResponse.LabelPDF);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    Console.WriteLine("Exception: " + ex.Message);
                }
            }
            else
            {
                foreach (DPDCloudService.ErrorDataType err in mySetOrderResponse.ErrorDataList)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + err.ErrorMsgShort);
                    Console.WriteLine("Error: " + err.ErrorMsgShort);
                }
            }

            Console.ReadKey();

        }
    }
}
