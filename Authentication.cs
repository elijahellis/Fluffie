using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
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
    public class Authentication
    {

        private string _APIKEY;
        private string _Ticket;
        /// <summary>
        /// Allows for authentication with Box.net
        /// </summary>
        /// <param name="apiKey">The user's api-key</param>
        public Authentication(string apiKey)
        {
            _APIKEY = apiKey;
            _Ticket = GetTicket();
        }

        /// <summary>
        /// The ticket from Box.net for retrieving the authentication token
        /// </summary>
        public string Ticket
        {
            get
            {
                return _Ticket;
            }
        }

        /// <summary>
        /// The url to pass for user-authentication before getting the authentication token
        /// </summary>
        public string AuthenticationURL
        {
            get
            {
                return "https://www.box.net/api/1.0/auth/" + _Ticket;
            }
        }

        /// <summary>
        /// The api key being used
        /// </summary>
        public string APIKey
        {
            get
            {
                return _APIKEY;
            }
        }

        private string GetTicket()
        {
            string FinalTicket;
            HttpWebRequest askTicket = (HttpWebRequest)HttpWebRequest.Create("https://www.box.net/api/1.0/rest?action=get_ticket&api_key=" + _APIKEY);
            askTicket.Method = "GET";
            askTicket.UserAgent = Constants.USERAGENT;
            var getTicket = new XmlDocument();
            getTicket.LoadXml((new System.IO.StreamReader(askTicket.GetResponse().GetResponseStream()).ReadToEnd()));
            FinalTicket = getTicket.SelectSingleNode("//response/ticket").InnerText;
            return FinalTicket;
        }

        /// <summary>
        /// Gets the authentication token for further Box.net functionality after the user has logged in
        /// </summary>
        /// <param name="ticket">The current ticket being used</param>
        /// <returns>The proper authentication token for API calls</returns>
        public string GetAuthToken(string ticket)
        {
            HttpWebRequest askAuth = (HttpWebRequest)HttpWebRequest.Create(string.Format("https://www.box.net/api/1.0/rest?action=get_auth_token&api_key={0}&ticket={1}",_APIKEY,_Ticket));
            askAuth.Method = "GET";
            askAuth.UserAgent = Constants.USERAGENT;
            XmlDocument tokenResp = new XmlDocument();
            tokenResp.LoadXml(new System.IO.StreamReader(askAuth.GetResponse().GetResponseStream()).ReadToEnd());
            return tokenResp.SelectSingleNode("//response/auth_token").InnerText;
        }

        public void LogOut(string authToken)
        {
            HttpWebRequest askLogout = (HttpWebRequest)HttpWebRequest.Create(string.Format(Constants.LOGOUT_TEMPLATE,_APIKEY,authToken));
            askLogout.Method = "GET";
            askLogout.UserAgent = Constants.USERAGENT;
            askLogout.GetResponse();
        }

        

    }
}
