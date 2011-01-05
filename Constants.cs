using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    static class Constants
    {
        //Folder-tree constants
        public const string ROOTFOLDER = "0";
        public const string GETTREE = "get_account_tree";
        public const string NOZIP_PARAM = "nozip";
        public const string SIMPLE_PARAM = "simple";
        public const string NOFILES_PARAM = "nofiles";
        public const string ONELEVEL_PARAM = "onelevel";
        public const string FOLDER_ID_URL_PARAM = "&folder_id=";

        //File retrieval constants
        public const string DOWNLOAD_BASE_URL = "https://www.box.net/api/1.0/download";
        public const string GETFILE = "get_file_info";
        public const string FILE_ID = "&file_id=";

        //Generic Constants
        public const string BASE_ACTION_URL = "https://www.box.net/api/1.0/rest?action=";
        public const string DELETE_TEPMPLATE = "https://www.box.net/api/1.0/rest?action=delete&api_key={0}&auth_token={1}&target={2}&target_id={3}";
        public const string RENAME_TEMPLATE = "https://www.box.net/api/1.0/rest?action=rename&api_key={0}&auth_token={1}&target={2}&target_id={3}&new_name={4}";
        public const string MOVE_TEMPLATE = "https://www.box.net/api/1.0/rest?action=move&api_key={0}&auth_token={1}&target={2}&target_id={3}&destination_id={4}";
        public const string SHARE_TEMPLATE = "https://www.box.net/api/1.0/rest?action=public_share&api_key={0}&auth_token={1}&target={2}&target_id={3}&password={4}";
        public const string SETDESCR_TEMPLATE = "https://www.box.net/api/1.0/rest?action=set_description&api_key={0}&auth_token={1}&target={2}&target_id={3}&description={4}";
        public const string ADDTOBOX_TEMPLATE = "https://www.box.net/api/1.0/rest?action=add_to_mybox&api_key={0}&auth_token={1}&file_id={2}&public_name={3}&folder_id={4}&tags[]={5}";
        public const string ADDTAG_TEMPLATE = "https://www.box.net/api/1.0/rest?action=add_to_tag&api_key=p6eu1n2tblaeebnfkyby4gfkg6mrz0xp&auth_token=qydz66sfc6pqb2mnk565izn5m870ghs2&tags[]=test_tag&tags[]=test_tag1&target={}&target_id={}";
        public const string LOGOUT_TEMPLATE = "https://www.box.net/api/1.0/rest?action=logout&api_key={0}&auth_token={1}";
        public const string SHAREURL_BASE = "http://www.box.net/shared";
        public const string API_URL_PARAM = "&api_key=";
        public const string AUTH_URL_PARAM = "&auth_token=";
        public const string USERAGENT = "Textie-Client";
        public const string NEWLINE = "\r\n";

        //Uploading Constants
        public const string UPLOAD_BASEURL = "https://upload.box.net/api/1.0/upload/";


        //Request constants
        public const string GETMETHOD = "GET";
        public const string POSTMETHOD = "POST";
        


    }
}
