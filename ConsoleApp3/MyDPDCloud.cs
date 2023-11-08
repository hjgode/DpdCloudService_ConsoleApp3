using System;
using System.Globalization;
using System.Collections.Generic;

namespace ConsoleApp3
{
    public class MyDPDCloudService
    {
        public enum CredentialType
        {
            stage,
            production
        }

        public class MyCredentials
        {
            public DPDCloudService.PartnerCredentialType partnerCredentials
            {
                get
                {
                    DPDCloudService.PartnerCredentialType cred = new DPDCloudService.PartnerCredentialType();
                    cred.Name = "DPD Cloud Service Alpha2";
                    cred.Token = "33879594E70436D58685";
                    return cred;
                }
            }
            public DPDCloudService.PartnerCredentialType partnerCredentialsStage
            {
                get
                {
                    DPDCloudService.PartnerCredentialType cred = new DPDCloudService.PartnerCredentialType();
                    cred.Name = "DPD Sandbox";
                    cred.Token = "06445364853584D75564";
                    return cred;
                }

            }
            public DPDCloudService.UserCredentialType userCredentials
            {
                get
                {
                    DPDCloudService.UserCredentialType cred = new DPDCloudService.UserCredentialType();
                    cred.cloudUserID = 5697191;
                    cred.Token = "D6C6865595A6F6365516";
                    return cred;
                }
            }
            public DPDCloudService.UserCredentialType userCredentialsStage
            {
                get
                {
                    DPDCloudService.UserCredentialType cred = new DPDCloudService.UserCredentialType();
                    cred.cloudUserID = 2784301;
                    cred.Token = "41453373646A726C4F34";
                    return cred;
                }
            }

        }

        public static DPDCloudService.ParcelDataType MyParcelData(string sFiliale, string sInternalID,
            decimal dWeight, string sReference1)
        {
            DPDCloudService.ParcelDataType myParcelData;

            myParcelData = new DPDCloudService.ParcelDataType();
            myParcelData.YourInternalID = sInternalID;
            myParcelData.Content = "Kassen-PC";
            myParcelData.Weight = dWeight;
            myParcelData.Reference1 = sReference1;
            myParcelData.Reference2 = "";
            myParcelData.ShipService = new DPDCloudService.ShipServiceType();
            myParcelData.ShipService = DPDCloudService.ShipServiceType.Classic;
            return myParcelData;
        }

        public static DPDCloudService.AddressType MyAddress(string sCompany, string sStreet, string sHausNo, string sPLZ, string sCity)
        {
            DPDCloudService.AddressType myShipAddress;
            myShipAddress = new DPDCloudService.AddressType();
            myShipAddress.Gender = "";
            myShipAddress.Salutation = "";
            myShipAddress.FirstName = "";
            myShipAddress.LastName = "";
            myShipAddress.Company = sCompany;// "Hellweg die Profi-Baumärkte";
            myShipAddress.Name = "Marktleitung";
            myShipAddress.Street = sStreet; //"Wailandtstr.";
            myShipAddress.HouseNo = sHausNo; //"1";
            myShipAddress.ZipCode = sPLZ; //"63741";
            myShipAddress.City = sCity; //"Aschaffenburg";
            myShipAddress.Country = "DEU";
            myShipAddress.State = "";
            myShipAddress.Phone = "+49 2154 88866 50";
            myShipAddress.Mail = "heinz-josef.gode@lodata.de";
            return myShipAddress;
        }
        public static bool setOrter(DPDCloudService.AddressType adresse, DPDCloudService.ParcelDataType[] parceldata, ref string sParcelNo, ref string sError)
        {
            bool bRet = false;
            DPDCloudService.setOrderRequestType mySetOrderRequest = new DPDCloudService.setOrderRequestType();
            DPDCloudService.OrderDataType myOrderData = new DPDCloudService.OrderDataType();
            List<DPDCloudService.OrderDataType> myOrderDataList = new List<DPDCloudService.OrderDataType>();

            mySetOrderRequest.Version = 100;
            mySetOrderRequest.Language = "de_DE";

            MyCredentials myCredentials = new MyCredentials();

            mySetOrderRequest.PartnerCredentials = new DPDCloudService.PartnerCredentialType();
            mySetOrderRequest.PartnerCredentials = myCredentials.partnerCredentialsStage;
            //            mySetOrderRequest.PartnerCredentials.Name = myCredentials. "DPD Cloud Service Alpha2";
            //            mySetOrderRequest.PartnerCredentials.Token = "33879594E70436D58685";

            mySetOrderRequest.UserCredentials = new DPDCloudService.UserCredentialType();
            mySetOrderRequest.UserCredentials = myCredentials.userCredentialsStage;
            //            mySetOrderRequest.UserCredentials.cloudUserID = 5697191;
            //            mySetOrderRequest.UserCredentials.Token = "D6C6865595A6F6365516";

            mySetOrderRequest.OrderAction = DPDCloudService.OrderActionType.startOrder;
            mySetOrderRequest.OrderSettings = new DPDCloudService.OrderSettingsType();
            mySetOrderRequest.OrderSettings.LabelSize = DPDCloudService.LabelSizeType.PDF_A6;
            mySetOrderRequest.OrderSettings.LabelStartPosition = DPDCloudService.LabelStartPositionType.UpperLeft;

            DateTimeFormatInfo deDtfi = new CultureInfo("de-DE", false).DateTimeFormat;

            mySetOrderRequest.OrderSettings.ShipDate = DateTime.Now;// new DateTime(2023, 11, 9);

            //create orderDataList
            foreach (DPDCloudService.ParcelDataType parcel in parceldata) {
                myOrderData = new DPDCloudService.OrderDataType();
                myOrderData.ShipAddress = adresse;// new DPDCloudService.AddressType();

                myOrderData.ParcelData = parcel;// new DPDCloudService.ParcelDataType();

                myOrderDataList.Add(myOrderData);
            }
            mySetOrderRequest.OrderDataList = myOrderDataList.ToArray();

            DPDCloudService.DPDCloudService_v1SoapClient myApiSoapClient =
            new DPDCloudService.DPDCloudService_v1SoapClient("staging");
            /*
                    <client>
                        <endpoint
                                  address="https://cloud.dpd.com/services/v1/DPDCloudService.asmx"
                            binding="basicHttpBinding" bindingConfiguration="DPDCloudService_v1Soap"
                            contract="DPDCloudService.DPDCloudService_v1Soap" 
                                  name="DPDCloudService_v1Soap" />
                        <endpoint name="staging"
                                  address="https://cloud-stage.dpd.com/services/v1/DPDCloudService.asmx"
                            binding="basicHttpBinding" bindingConfiguration="DPDCloudService_v1Soap"
                            contract="DPDCloudService.DPDCloudService_v1Soap"  />
                    </client>

            */
            DPDCloudService.setOrderResponseType mySetOrderResponse = new DPDCloudService.setOrderResponseType();

            mySetOrderResponse = myApiSoapClient.setOrder(mySetOrderRequest);

            if (mySetOrderResponse.Ack)
            {
                bRet = true;
                System.Diagnostics.Debug.WriteLine(mySetOrderResponse.LabelResponse.ToString());

                string parcelNo = mySetOrderResponse.LabelResponse.LabelDataList[0].ParcelNo;
                string intID = mySetOrderResponse.LabelResponse.LabelDataList[0].YourInternalID;

                foreach (DPDCloudService.LabelDataType data in mySetOrderResponse.LabelResponse.LabelDataList)
                {
                    parcelNo = data.ParcelNo;
                    sParcelNo += parcelNo +"_";
                    intID = data.YourInternalID;
                    System.Diagnostics.Debug.WriteLine("Response data: " + parcelNo + "/" + intID);
                    //Console.WriteLine("Response data: " + parcelNo + "/" + intID);
                }
                try
                {
                    Byte[] Base64Byte = System.Convert.FromBase64String(mySetOrderResponse.LabelResponse.LabelPDF);
                    System.IO.FileStream w = new System.IO.FileStream(sParcelNo + ".pdf", System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
                    w.Write(Base64Byte, 0, Base64Byte.Length);
                    w.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                    bRet = false;
                    sError = "File write: " + ex.Message;
                    //Console.WriteLine("Exception: " + ex.Message);
                }
            }
            else
            {
                bRet = false;
                foreach (DPDCloudService.ErrorDataType err in mySetOrderResponse.ErrorDataList)
                {
                    System.Diagnostics.Debug.WriteLine("Error: " + err.ErrorMsgShort);
                    //Console.WriteLine("Error: " + err.ErrorMsgShort);
                    sError += err.ErrorMsgShort + ", ";
                }
            }

            return bRet;
        }

        public static void doTest()
        {
            DPDCloudService.setOrderRequestType mySetOrderRequest = new DPDCloudService.setOrderRequestType();
            DPDCloudService.OrderDataType myOrderData = new DPDCloudService.OrderDataType();
            List<DPDCloudService.OrderDataType> myOrderDataList = new List<DPDCloudService.OrderDataType>();

            mySetOrderRequest.Version = 100;
            mySetOrderRequest.Language = "de_DE";

            MyCredentials myCredentials = new MyCredentials();

            mySetOrderRequest.PartnerCredentials = new DPDCloudService.PartnerCredentialType();
            mySetOrderRequest.PartnerCredentials = myCredentials.partnerCredentialsStage;
            //            mySetOrderRequest.PartnerCredentials.Name = myCredentials. "DPD Cloud Service Alpha2";
            //            mySetOrderRequest.PartnerCredentials.Token = "33879594E70436D58685";

            mySetOrderRequest.UserCredentials = new DPDCloudService.UserCredentialType();
            mySetOrderRequest.UserCredentials = myCredentials.userCredentialsStage;
            //            mySetOrderRequest.UserCredentials.cloudUserID = 5697191;
            //            mySetOrderRequest.UserCredentials.Token = "D6C6865595A6F6365516";

            mySetOrderRequest.OrderAction = DPDCloudService.OrderActionType.startOrder;
            mySetOrderRequest.OrderSettings = new DPDCloudService.OrderSettingsType();
            mySetOrderRequest.OrderSettings.LabelSize = DPDCloudService.LabelSizeType.PDF_A6;
            mySetOrderRequest.OrderSettings.LabelStartPosition = DPDCloudService.LabelStartPositionType.UpperLeft;

            DateTimeFormatInfo deDtfi = new CultureInfo("de-DE", false).DateTimeFormat;

            mySetOrderRequest.OrderSettings.ShipDate = new DateTime(2023,11,9);

            myOrderData.ShipAddress = new DPDCloudService.AddressType();
            myOrderData.ShipAddress.Gender = "";
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

            myOrderDataList.Add(myOrderData);
            //second parcel
            myOrderData.ParcelData = new DPDCloudService.ParcelDataType();
            myOrderData.ParcelData.YourInternalID = "Filiale xyz/2";
            myOrderData.ParcelData.Content = "Smartphone";
            myOrderData.ParcelData.Weight = 13.5m;
            myOrderData.ParcelData.Reference1 = "Filiale 123/2";
            myOrderData.ParcelData.Reference2 = "";
            myOrderData.ParcelData.ShipService = new DPDCloudService.ShipServiceType();
            myOrderData.ParcelData.ShipService = DPDCloudService.ShipServiceType.Classic;
            /*
            Response data: 09985052731146/Filiale xyz/2
            Response data: 09985052731146/Filiale xyz/2
            Response data: 09985052731147/Filiale xyz/2
            Response data: 09985052731147/Filiale xyz/2

            */
            myOrderDataList.Add(myOrderData);

            mySetOrderRequest.OrderDataList = myOrderDataList.ToArray();

            DPDCloudService.DPDCloudService_v1SoapClient myApiSoapClient =
            new DPDCloudService.DPDCloudService_v1SoapClient("staging");
            /*
                    <client>
                        <endpoint
                                  address="https://cloud.dpd.com/services/v1/DPDCloudService.asmx"
                            binding="basicHttpBinding" bindingConfiguration="DPDCloudService_v1Soap"
                            contract="DPDCloudService.DPDCloudService_v1Soap" 
                                  name="DPDCloudService_v1Soap" />
                        <endpoint name="staging"
                                  address="https://cloud-stage.dpd.com/services/v1/DPDCloudService.asmx"
                            binding="basicHttpBinding" bindingConfiguration="DPDCloudService_v1Soap"
                            contract="DPDCloudService.DPDCloudService_v1Soap"  />
                    </client>

            */
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
