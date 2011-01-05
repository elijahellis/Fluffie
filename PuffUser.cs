using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
    public class PuffUser
    {

        public PuffUser(string token, string apiKey)
        {
            HttpWebRequest getUser = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://www.box.net/api/1.0/rest?action=get_account_info&api_key={0}&auth_token={1}", apiKey, token));
            getUser.Method = "GET";
            getUser.UserAgent = Constants.USERAGENT;
            XDocument infoUser = XDocument.Load(getUser.GetResponse().GetResponseStream());

            var userRoot = infoUser.Element("response").Element("user");


            _access_id = userRoot.Element("access_id").Value;
            _userID = userRoot.Element("user_id").Value;
            _SpaceUsd = userRoot.Element("space_used").Value;
            _email = userRoot.Element("email").Value;
            _SpaceAm = userRoot.Element("space_amount").Value;
            _login = userRoot.Element("login").Value;
            _MaxUpSize = userRoot.Element("max_upload_size").Value;
        }

        private string _MaxUpSize;
        public long MaximumUploadSize
        {
            get
            {
                return long.Parse(_MaxUpSize);
            }
        }

        private string _SpaceUsd;
        public long SpaceUsed
        {
            get
            {
                return long.Parse(_SpaceUsd);
            }
        }

        private string _SpaceAm;
        public long AvailibleSpace
        {
            get
            {
                return long.Parse(_SpaceAm);
            }
        }

        private string _userID;
        public string UserID
        {
            get
            {
                return _userID;
            }
        }

        private string _access_id;
        public string AccessID
        {
            get
            {
                return _access_id;
            }
        }

        private string _email;
        public string Email
        {
            get
            {
                return _email;
            }
        }

        private string _login;
        public string Login
        {
            get
            {
                return _login;
            }
        }

    }
}
