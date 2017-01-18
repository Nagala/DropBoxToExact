using ExactOnline.Client.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBox
{
    public class Connector
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly Uri _callbackUrl;
        private readonly UserAuthorization _authorization;

        public string EndPoint
        {
            get
            {
                return "https://start.exactonline.co.uk";
            }
        }

        public Connector(string clientId, string clientSecret, Uri callbackUrl)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _callbackUrl = callbackUrl;
            _authorization = new UserAuthorization();
            
        }
        
        public string GetAccessToken()
        {
            UserAuthorizations.Authorize(_authorization, EndPoint, _clientId, _clientSecret, _callbackUrl);

            return _authorization.AccessToken;
        }

    }
}
