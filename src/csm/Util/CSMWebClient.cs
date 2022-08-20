using System;
using System.Net;

namespace CSM.Util
{
    class CSMWebClient : WebClient
    {
        private WebRequest _request = null;

        protected override WebRequest GetWebRequest(Uri address)
        {
            this._request = base.GetWebRequest(address);
            // Force IPv4 for now, TODO: Remove when CSM supports IPv6
            if (this._request is HttpWebRequest webRequest)
            {
                webRequest.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
                {
                    if (remoteEndPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return new IPEndPoint(IPAddress.Any, 0);
                    }

                    throw new InvalidOperationException("no IPv4 address");
                };

                webRequest.AllowAutoRedirect = false;
            }

            return this._request;
        }

        public HttpStatusCode StatusCode()
        {
            HttpStatusCode result;

            if (this._request == null)
            {
                throw (new InvalidOperationException("Unable to retrieve the status code, maybe you haven't made a request yet."));
            }

            if (base.GetWebResponse(this._request) is HttpWebResponse response)
            {
                result = response.StatusCode;
            }
            else
            {
                throw (new InvalidOperationException("Unable to retrieve the status code, maybe you haven't made a request yet."));
            }

            return result;
        }
    }
}
