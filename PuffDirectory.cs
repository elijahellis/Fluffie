using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
    public class PuffDirectory
    {

        //Declare some inportant variables
        private XmlDocument _UnderlyingXML; 
        private string _apiKey;
        private string _token;

        public PuffDirectory(string apiKey, string token, string id = Constants.ROOTFOLDER)
        {
            HttpWebRequest getDirectory = (HttpWebRequest)HttpWebRequest.Create(Constants.BASE_ACTION_URL + Constants.GETTREE + Constants.API_URL_PARAM + apiKey + Constants.AUTH_URL_PARAM + token + Constants.FOLDER_ID_URL_PARAM + id + "&params[]=" + Constants.NOZIP_PARAM);
            getDirectory.Method = "GET";
            getDirectory.UserAgent = Constants.USERAGENT;

            _apiKey = apiKey;
            _token = token;


            XmlDocument fullTree = new XmlDocument();
            fullTree.Load(getDirectory.GetResponse().GetResponseStream());
            _UnderlyingXML = fullTree;
            _FolderID = fullTree.SelectSingleNode("//response//tree/folder").Attributes["id"].Value;
            _FolderName = fullTree.SelectSingleNode("//response//tree/folder").Attributes["name"].Value;
            _shared = fullTree.SelectSingleNode("//response//tree/folder").Attributes["shared"].Value;

        }

        public string Share()
        {
            HttpWebRequest askShare = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.SHARE_TEMPLATE, _apiKey, _token, "folder", _FolderID, ""));
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

        public string Share(string password)
        {
            HttpWebRequest askShare = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.SHARE_TEMPLATE, _apiKey, _token, "folder", _FolderID, password));
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

        public bool Move(string destinationID)
        {
            HttpWebRequest askMove = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.MOVE_TEMPLATE, _apiKey, _token, "folder", _FolderID, destinationID));
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

        public bool SetDescription(string description)
        {
            HttpWebRequest askShare = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.SETDESCR_TEMPLATE, _apiKey, _token, "folder", _FolderID, description));
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

        public bool Rename(string newName)
        {
            HttpWebRequest askRename = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.RENAME_TEMPLATE, _apiKey, _token, "folder", _FolderID, newName));
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

        public bool Delete()
        {
            HttpWebRequest deleteReq = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.DELETE_TEPMPLATE, _apiKey, _token, "folder", _FolderID));
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

        private List<PuffFile> allFiles = new List<PuffFile>();
        public PuffFile[] Files
        {
            get
            {
                try
                {
                    foreach (XmlNode node in _UnderlyingXML.SelectNodes("//response/tree/folder/files/file"))
                    {
                        if (node != null)
                        {
                            //System.Windows.Forms.MessageBox.Show(node.Attributes[0].InnerText);
                            allFiles.Add(new PuffFile(_apiKey, _token, node.Attributes[0].InnerText));
                        }
                    }
                }
                catch
                {
                    //System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
                return allFiles.ToArray();
            }
        }

        private List<PuffDirectory> allFolders = new List<PuffDirectory>();
        public PuffDirectory[] Children
        {
            get
            {
                try
                {
                    foreach (XmlNode node in _UnderlyingXML.SelectNodes("//tree/folder/folders/folder"))
                    {
                        if (node != null)
                        {
                            //System.Windows.Forms.MessageBox.Show(node.Attributes["id"].InnerText);
                            allFolders.Add(new PuffDirectory(_apiKey, _token, node.Attributes["id"].InnerText));
                        }
                    }
                }
                catch 
                {
                    //System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
                
                return allFolders.ToArray();
            }
        }

        private string _shared;
        public bool Shared
        {
            get
            {
                if (_shared == "0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private string _FolderName;
        public string Name
        {
            get
            {
                return
                    _FolderName;
            }
        }

        private string _FolderID;
        public string ID
        {
            get
            {
                return _FolderID;
            }
        }

    }
}
