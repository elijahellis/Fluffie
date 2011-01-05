using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;

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
    public class PuffFile
    {
        //Declare the important underlying variables
        private XmlDocument _UnderlyingXML;
        private string _apiKey;
        private string _auth_token;

        //Declare all of value-holding the variables
        private string _file_id;
        private string _filename;
        private string _father_folder_ID;
        private string _sharedstring;
        private string _sharedname;
        private string _sizestring;
        private string _description;
        private string _SHA1_hash;
        private string _created;
        private string _updated;
        
        /// <summary>
        /// Generates a new instance of a file from Box.net
        /// </summary>
        /// <param name="apiKey">Your application's apiKey</param>
        /// <param name="authToken">The user-specific authentication-token</param>
        /// <param name="id">The ID of the file to create an instance of</param> <seealso cref="PuffDirectory.Files"/>
        public PuffFile (string apiKey, string authToken, string id)
        {
            //Set token and apiKey variables
            _apiKey = apiKey;
            _auth_token = authToken;

            //Make the request
            HttpWebRequest getFileInfo = (HttpWebRequest)HttpWebRequest.Create(Constants.BASE_ACTION_URL + Constants.GETFILE + Constants.API_URL_PARAM + apiKey + Constants.AUTH_URL_PARAM + authToken + Constants.FILE_ID + id);

            //System.Windows.Forms.MessageBox.Show(getFileInfo.RequestUri.ToString());
            getFileInfo.UserAgent = Constants.USERAGENT;
            getFileInfo.Method = "GET";

            //Load the underlying document to a class-wide accessible variable
            _UnderlyingXML = new XmlDocument();
            _UnderlyingXML.Load(getFileInfo.GetResponse().GetResponseStream());

            //Add the 'simple' values to the variables
            XmlNode dataNode = _UnderlyingXML.SelectSingleNode("//response//info"); //Get the 'root' node, to save on typing it all out

            _file_id = dataNode.SelectSingleNode("//file_id").InnerText;
            _filename = dataNode.SelectSingleNode("//file_name").InnerText;
            _father_folder_ID = dataNode.SelectSingleNode("//folder_id").InnerText;
            _sharedstring = dataNode.SelectSingleNode("//shared").InnerText;
            if (_sharedstring == "1") { _sharedname = dataNode.SelectSingleNode("//shared_name").InnerText; }
            _sizestring = dataNode.SelectSingleNode("//size").InnerText;
            _description = dataNode.SelectSingleNode("//description").InnerText;
            _SHA1_hash = dataNode.SelectSingleNode("//sha1").InnerText;
            _created = dataNode.SelectSingleNode("//created").InnerText;
            _updated = dataNode.SelectSingleNode("//updated").InnerText;

        }

        /// <summary>
        /// If not shared, makes the current file shared
        /// </summary>
        /// <param name="password">The password to set to the shared link</param>
        /// <returns>The new url of the shared file</returns>
        public string Share()
        {
            HttpWebRequest askShare = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.SHARE_TEMPLATE, _apiKey, _auth_token, "file", _file_id, ""));
            askShare.Method = "GET";
            askShare.UserAgent = Constants.USERAGENT;
            try
            {
                XmlDocument readTag = new XmlDocument();
                readTag.Load(askShare.GetResponse().GetResponseStream());
                string endPoint = readTag.SelectSingleNode("//response//public_name").InnerText;
                return Constants.SHAREURL_BASE + "/" + endPoint;
            }
            catch 
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// If not shared, makes the current file shared
        /// </summary>
        /// <param name="password">The password to set to the shared link</param>
        /// <returns>The new url of the shared file</returns>
        /// <remarks>This method only applies to an upgraded account</remarks>
        public string Share(string password)
        {
            HttpWebRequest askShare = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.SHARE_TEMPLATE, _apiKey, _auth_token, "file", _file_id, password));
            askShare.Method = "GET";
            askShare.UserAgent = Constants.USERAGENT;
            try
            {
                XmlDocument readTag = new XmlDocument();
                readTag.Load(askShare.GetResponse().GetResponseStream());
                string endPoint = readTag.SelectSingleNode("//response//public_name").InnerText;
                return Constants.SHAREURL_BASE + "/" + endPoint;
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool SetDescription(string description)
        {
            HttpWebRequest askShare = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.SETDESCR_TEMPLATE, _apiKey, _auth_token, "file", _file_id, description));
            askShare.Method = "GET";
            askShare.UserAgent = Constants.USERAGENT;
            try
            {
                XmlDocument readTag = new XmlDocument();
                readTag.Load(askShare.GetResponse().GetResponseStream());
                if (readTag.SelectSingleNode("//status").InnerText == "s_set_description")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Moves the current file to a new directory
        /// </summary>
        /// <param name="destinationID">The ID of the folder to move the file</param>
        /// <returns>True is successful, otherwise false</returns>
        public bool Move(string destinationID)
        {
            HttpWebRequest askMove = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.MOVE_TEMPLATE, _apiKey, _auth_token, "file", _file_id, destinationID));
            askMove.Method = "GET";
            askMove.UserAgent = Constants.USERAGENT;
            try
            {
                XmlDocument newDoc = new XmlDocument();
                newDoc.Load(askMove.GetResponse().GetResponseStream());
                if (newDoc.SelectSingleNode("//response//status").InnerText == "s_move_node")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Renames the current file with a provided new name
        /// </summary>
        /// <param name="newName">The nnew name to give the current file</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool Rename(string newName)
        {
            HttpWebRequest askRename = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.RENAME_TEMPLATE, _apiKey, _auth_token, "file", _file_id, newName));
            askRename.Method = "GET";
            askRename.UserAgent = Constants.USERAGENT;
            try
            {
                XmlDocument newDoc = new XmlDocument();
                newDoc.Load(askRename.GetResponse().GetResponseStream());
                if (newDoc.SelectSingleNode("//response//status").InnerText == "s_rename_node")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the current file from the authenticated account
        /// </summary>
        /// <returns>True if successful, otherwise false</returns>
        public bool Delete()
        {
            HttpWebRequest deleteReq = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.DELETE_TEPMPLATE, _apiKey, _auth_token, "file", _file_id));
            deleteReq.Method = "GET";
            deleteReq.UserAgent = Constants.USERAGENT;
            try
            {
                XmlDocument newDoc = new XmlDocument();
                newDoc.Load(deleteReq.GetResponse().GetResponseStream());
                if (newDoc.SelectSingleNode("//response//status").InnerText == "s_delete_node")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }            
        }

        /// <summary>
        /// Gets the (private, and authenticated) location of the file
        /// </summary>
        public string DownloadUrl
        {
            get
            {
                return Constants.DOWNLOAD_BASE_URL + "/" + _auth_token + "/" + _file_id;
            }
        }

        /// <summary>
        /// Gets a string representing the date of the file's last update
        /// </summary>
        public string Updated
        {
            get
            {
                return _updated;
            }
        }

        /// <summary>
        /// Gets a string representing the date of creation
        /// </summary>
        public string Created
        {
            get
            {
                return _created;
            }
        }

        /// <summary>
        /// Gets the SHA1-hash associated with the current file
        /// </summary>
        public string SHA1
        {
            get
            {
                return _SHA1_hash;
            }
        }

        /// <summary>
        /// Gets or sets the item's description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (SetDescription(value))
                {
                    _description = value;
                }                
            }
        }

        /// <summary>
        /// Gets the file's size in bytes
        /// </summary>
        public long FileSize
        {
            get
            {
                return long.Parse(_sizestring);
            }
        }

        /// <summary>
        /// Gets the Url (if shared) of the current file's shared location
        /// </summary>
        public string SharedUrl
        {
            get
            {
                return Constants.SHAREURL_BASE + "/" + _sharedname;
            }
        }

        /// <summary>
        /// Checks if the current file is shared
        /// </summary>
        public bool Shared
        {
            get
            {
                if (_sharedstring == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the parent folder's ID
        /// </summary>
        public string ContainingFolderID
        {
            get
            {
                return _father_folder_ID;
            }
        }

        /// <summary>
        /// Gets the name (including extension) of the current file
        /// </summary>
        public string Name
        {
            get
            {
                return _filename;
            }
        }

        /// <summary>
        /// Gets the ID for the current file
        /// </summary>
        public string ID
        {
            get
            {
                return _file_id;
            }
        }

        



    }
}
