using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Xml;

/* This file is part of the Fluffie Library.

    Fluffie is free software-library: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Fluffie is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Fluffie.  If not, see <http://www.gnu.org/licenses/>.
 */

namespace Fluffie
{
    public class UploadMethods
    {        

        private string SimpleUploadFile(string fileData, string authToken)
        {
            var newrequest = new System.Net.WebClient();
            var Response = newrequest.UploadFile(Constants.UPLOAD_BASEURL + authToken + "/0", "POST", fileData);            
            return Encoding.UTF8.GetString(Response);
        }

        public HttpWebRequest UploadSingularFile(string filename, string name, byte[] fileData, string Url)
        {
            //Build the basic request
            HttpWebRequest askUpload = (HttpWebRequest)HttpWebRequest.Create(Url);
            askUpload.Method = "POST";
            askUpload.KeepAlive = true; //Keep this alive! We're uploading data here!
            askUpload.UserAgent = Constants.USERAGENT; //Make it all special with my very own user-agent
            string dataBoundary = RandomBoundary();//Generate a random boundary to keep things interesting
            askUpload.ContentType = "multipart/form-data; boundary=" + dataBoundary;

            using (MemoryStream formBytes = new MemoryStream()) //Use a MemoryStream to prep all the data
            {
                string fileHeaderTemplate = "--" + dataBoundary + "\r\n" + @"Content-Disposition: form-data; name=""{0}""; filename=""{1}""" + "\r\n" + "Content-Type: {2}" + "\r\n\r\n";
                
                /*
                 * This should resemble this: http://www.w3.org/TR/html401/interact/forms.html
                 * --"boundary"
                 * Content-Disposition: form-data; name="whatever"; filename="whatever.ext"
                 * Content-Type: "mimetype" (or just a regular octet-stream)
                 * *just a line
                 * *just a line (where the file is read to)
                 * 
                 * --"boundary"(--) *depends on the location of writing; if at the end of the document, the two extra dashes are appended
                 * 
                 */
                
                //Convert the template, plus the formatters into a byte-array (UTF8)
                byte[] fileHeaderbytes = Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate, name, filename, "application/octet-stream"));

                //Writing out the headers and the file begins. This is obviously a singular thing, foreach or for can be used for multiples
                formBytes.Write(fileHeaderbytes, 0, fileHeaderbytes.Length);
                formBytes.Write(fileData, 0, fileData.Length);

                //This is that magical trailing boundary (notice the extra dashes)
                byte[] trailingBoundary = Encoding.UTF8.GetBytes("\r\n--" + dataBoundary + "--\r\n");
                formBytes.Write(trailingBoundary, 0, trailingBoundary.Length);


                //Set the length of the request (length of the MemoryStream in this case)
                askUpload.ContentLength = formBytes.Length;

                //Assign a varible for the requesting stream
                Stream reqStream = askUpload.GetRequestStream();
                reqStream.Write(formBytes.ToArray(), 0, formBytes.ToArray().Length); //Write ALL of the data to the stream
                reqStream.Close(); //Close the stream, to disallow writing and finalize the data
            }
            
            //Return the built request for further manipulation
            return askUpload;
        }

        public HttpWebRequest CreateUploadRequest(NameValueCollection keysAndVals, Dictionary<string, byte[]> files, string Url)
        {
            //Build the basic request
            HttpWebRequest askUpload = (HttpWebRequest)HttpWebRequest.Create(Url);
            askUpload.Method = "POST";
            askUpload.KeepAlive = true; //Keep this alive! We're uploading data here!
            askUpload.UserAgent = Constants.USERAGENT; //Make it all special with my very own user-agent
            string dataBoundary = RandomBoundary();//Generate a random boundary to keep things interesting
            askUpload.ContentType = "multipart/form-data; boundary=" + dataBoundary;

            using (MemoryStream formBytes = new MemoryStream()) //Use a MemoryStream to prep all the data
            {
                //Build all the extra-values and such usually unecessary for simple uploading

                string HeaderTemplate = "--" + dataBoundary + "\r\n" + @"Content-Disposition: form-data; name=""{0}""" + "\r\n\r\n{1}\r\n";
                int i;
                for (i = 0; i < keysAndVals.Count; i ++)
                {
                byte[] Headerbytes = Encoding.UTF8.GetBytes(string.Format(HeaderTemplate, keysAndVals.Keys[i], keysAndVals[i]));                
                formBytes.Write(Headerbytes, 0, Headerbytes.Length);//Write the header's converted byte-array
                }

                //Write-out all the given data in the dictionary (keys=filename, values=binary data)
                string fileHeaderTemplate = "--" + dataBoundary + "\r\n" + @"Content-Disposition: form-data; name=""{0}""; filename=""{1}""" + "\r\nContent-Type: {2}\r\n\r\n";
                i = 0;
                foreach (string key in files.Keys)
                {
                    i ++; //Make sure we add 1 to i, since this is a foreach loop
                    byte[] attemptedValue;
                    files.TryGetValue(key, out attemptedValue);
                    var fileHeaderBytes = Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate, "file" + i.ToString(), key, "application/octet-stream"));
                    formBytes.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);//Write the header
                    formBytes.Write(attemptedValue, 0, attemptedValue.Length);//Write the binary (assuming it was retrieved)
                    var addNewline = Encoding.UTF8.GetBytes("\r\n");
                    formBytes.Write(addNewline, 0, addNewline.Length);//Write a new line
                }
                




                //Write the very last, trailing boundary to the request
                byte[] trailingBoundary = Encoding.UTF8.GetBytes("--" + dataBoundary + "--\r\n");
                formBytes.Write(trailingBoundary, 0, trailingBoundary.Length);


                askUpload.ContentLength = formBytes.Length;//Set the length to the extrenuous data plus that of the files (all held in the MemoryStream)

                //Write the MemoryStream to the request stream, and close the request
                Stream reqStream = askUpload.GetRequestStream();
                reqStream.Write(formBytes.ToArray(), 0, formBytes.ToArray().Length);
                reqStream.Close();
            }
            
            return askUpload;//Return the finalized HttpWebRequest
        }

        private string GetKey(byte[] value, Dictionary<string, byte[]> dictionary)
        {
            string returnKey = "";
            foreach (string key in dictionary.Keys)
            {
                byte[] outValue;
                dictionary.TryGetValue(key, out outValue);
                if (outValue == value)
                {
                    returnKey = key;
                }
            }
            return returnKey;
        }

        private string RandomBoundary()
        {
            Random newRand = new Random();
            int numberOfChar = newRand.Next(0, 20);
            string stringTotouch = string.Empty;
            int i;
            for (i = 0; i < newRand.Next(10, 20); i++)
            {
                stringTotouch += "-";
            }
            for (i = 0; i < numberOfChar; i++)
            {
                stringTotouch += newRand.Next(0, 1000).ToString();
            }
            return stringTotouch;
        }


    }

    public class UploadItem
    {

        public UploadItem(string authToken, string folderId)
        {
            _token = authToken;
            _folder = folderId;
        }

        public string Upload(string fileName)
        {            
            var fileinform = new FileInfo(fileName);
            byte[] fileBytes = new byte[fileinform.Length];
            fileinform.OpenRead().Read(fileBytes, 0, (int)fileinform.Length);
            var item = new UploadMethods().UploadSingularFile(fileinform.Name, "file", fileBytes, Constants.UPLOAD_BASEURL + Token + "/" + Folder).GetResponse().GetResponseStream();
            System.Xml.XmlDocument response = new System.Xml.XmlDocument();
            response.Load(item);
            return response.SelectSingleNode("//response//file").Attributes["id"].InnerText;
        }

        public string[] Upload(string[] fileNames)
        {
            var nameBin = new Dictionary<string, byte[]>();
            foreach (string file in fileNames)
            {
                byte[] fileBuffer;
                fileBuffer = File.ReadAllBytes(file);
                nameBin.Add(file, fileBuffer);
                fileBuffer = null;
            }
            var newMultiUpload = new UploadMethods();
            var uploadReq = newMultiUpload.CreateUploadRequest(new NameValueCollection(), nameBin, Constants.UPLOAD_BASEURL + _token + "/" + _folder);
            List<string> fileIds = new List<string>();
            XmlDocument newXml = new XmlDocument();
            newXml.Load(uploadReq.GetResponse().GetResponseStream());
            foreach (XmlNode node in newXml.SelectNodes("/response/files//file"))
            {
                fileIds.Add(node.Attributes["id"].InnerText);
            }
            return fileIds.ToArray();
        }

        private string _token;
        public string Token
        {
            get
            {
                return _token;
            }
        }

        private string _folder;
        public string Folder
        {
            get
            {
                return _folder;
            }
        }

    }

}
