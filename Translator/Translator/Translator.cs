using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Translator {
    //PM> Install-Package Microsoft.Net.Http
    //PM> Install-Package Newtonsoft.Json

    internal class Translator {
        //登録されたアプリの一覧
        //https://datamarket.azure.com/developer/applications

        private const string ClientId = "ClientId";
        private const string ClientSecret = "ClientSecret";

        private string _token;

        private async Task<bool> InitializeToken() {
            if (_token == null && _token != "ERROR") {
                try {
                    var client = new HttpClient();
                    const string url = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
                    var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                        {"client_id", ClientId},
                        {"client_secret", ClientSecret},
                        {"scope", "http://api.microsofttranslator.com"},
                        {"grant_type", "client_credentials"},
                    });
                    var response = await client.PostAsync(url, content);
                    //JSON形式のレスポンスからaccess_toneを取得する
                    var adm = JsonConvert.DeserializeObject<AdmAccessToken>(await response.Content.ReadAsStringAsync());
                    _token = adm.access_token;
                    return true;
                }
                catch (Exception) {
                    ;
                }
                _token = "ERROR";
            }
            return false;
        }

        //JSON形式のレスポンスをデシリアライズするためのクラス
        [DataContract]
        public class AdmAccessToken {
            [DataMember]
            public string access_token { get; set; }

            //access_token以外は、取得しないので省略
        }

        public async Task<string> Get(string beforeStr) {
            if (_token == "ERROR") {
                return null;
            }

            //初回のみaccess_tokenを取得する
            if (_token == null) {
                if (!await InitializeToken()) {
                    return null;
                }
            }
            try {
                var client = new HttpClient();
                var url = string.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&to=ja"
                    , WebUtility.UrlEncode(beforeStr));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
                var response = await client.GetStringAsync(url);
                //XML解釈
                var doc = XDocument.Parse(response);
                var afterStr = doc.Root.FirstNode.ToString();
                if (afterStr != beforeStr) {
                    return afterStr;
                }
            }
            catch (Exception) {
                ; //tokenの期限切れの場合の処理は、未実装
            }
            return null;
        }
    }
}