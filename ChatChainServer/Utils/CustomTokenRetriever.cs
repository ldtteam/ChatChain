using System;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ChatChainServer.Utils
{
    public class CustomTokenRetriever
    {
        internal const string TokenItemsKey = "idsrv4:tokenvalidation:token";
        // custom token key change it to the one you use for sending the access_token to the server
        // during websocket handshake
        internal const string SignalRTokenKey = "signalr_token";

        static Func<HttpRequest, string> AuthHeaderTokenRetriever { get; set; }
        static Func<HttpRequest, string> QueryStringTokenRetriever { get; set; }

        static CustomTokenRetriever()
        {
            AuthHeaderTokenRetriever = TokenRetrieval.FromAuthorizationHeader();
            QueryStringTokenRetriever = TokenRetrieval.FromQueryString();
        }

        public static string FromHeaderAndQueryString(HttpRequest request)
        {
            string token = AuthHeaderTokenRetriever(request);

            if (string.IsNullOrEmpty(token))
            {
                token = QueryStringTokenRetriever(request);
            }

            if (string.IsNullOrEmpty(token))
            {
                token = request.HttpContext.Items[TokenItemsKey] as string;
            }

            if (string.IsNullOrEmpty(token) && request.Query.TryGetValue(SignalRTokenKey, out StringValues extract))
            {
                token = extract.ToString();
            }

            return token;
        }
    }
}