using System;

public class Authentication
{
    private GenericXmlSecurityToken token;
    private string site = "https://my.site.com"
    private string appliesTo = "http://my.site.com"
    private string authUsernameEndpoint = "https://sts-prod.site.com/adfs/services/trust/13/usernamemixed";

    public Authentication(PSCredential credential)
    {
        ADFSUsernameMixedTokenProvider tokenProvider = new ADFSUsernameMixedTokenProvider(new Uri(authUsernameEndpoint));
        token = tokenProvider.RequestToken(credential.UserName, credential.Password, appliesTo);
    }
    public CookieContainer GetFedAuthCookies()
    {
        string prepareToken = WrapInSoapMessage(token, appliesTo);
        string samlServer = site.EndsWith("/") ? site : site + "/";
        string stringData = $"wa=wsignin1.0&wresult={HttpUtility.UrlEncode(prepareToken)}&wctx={HttpUtility.UrlEncode("rm=1&id=passive&ru=%2f")}";

        CookieContainer cookies = new CookieContainer();
        HttpWebRequest request = WebRequest.Create(samlServer) as HttpWebRequest;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.CookieContainer = cookies;
        request.AllowAutoRedirect = false;
        byte[] data = Encoding.UTF8.GetBytes(stringData);
        request.ContentLength = data.Length;

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        {
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseFromServer = reader.ReadToEnd();
                }
            }
        }

        return cookies;
    }
    private string WrapInSoapMessage(GenericXmlSecurityToken token, string site)
    {
        string validFrom = token.ValidFrom.ToString("o");
        string validTo = token.ValidTo.ToString("o");
        string securityToken = token.TokenXml.OuterXml;
        string soapTemplate = @"<t:RequestSecurityTokenResponse xmlns:t=""http://schemas.xmlsoap.org/ws/2005/02/trust""><t:Lifetime><wsu:Created xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">{0}</wsu:Created><wsu:Expires xmlns:wsu=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">{1}</wsu:Expires></t:Lifetime><wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy""><wsa:EndpointReference xmlns:wsa=""http://www.w3.org/2005/08/addressing""><wsa:Address>{2}</wsa:Address></wsa:EndpointReference></wsp:AppliesTo><t:RequestedSecurityToken>{3}</t:RequestedSecurityToken><t:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</t:TokenType><t:RequestType>http://schemas.xmlsoap.org/ws/2005/02/trust/Issue</t:RequestType><t:KeyType>http://schemas.xmlsoap.org/ws/2005/05/identity/NoProofKey</t:KeyType></t:RequestSecurityTokenResponse>";

        return string.Format(soapTemplate, validFrom, validTo, site, securityToken);
    }
}
