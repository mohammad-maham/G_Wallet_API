using G_APIs.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Font = System.Drawing.Font;

namespace G_APIs.Common;


public static class Common
{

    public static string GetHash(string text)
    {
        return SHA256.Create()
            .ComputeHash(Encoding.UTF8.GetBytes(text))
            .Aggregate("", (x, y) => x + y);

    }

    //public static void BypassCertificateError()
    //{
    //    ServicePointManager.ServerCertificateValidationCallback +=
    //        delegate (
    //            Object sender1,
    //            X509Certificate certificate,
    //            X509Chain chain,
    //            SslPolicyErrors sslPolicyErrors)
    //        {
    //            return true;
    //        };
    //}
    public static string Base64Encode(string txt)
    {
        var txtBytes = System.Text.Encoding.UTF8.GetBytes(txt);
        return System.Convert.ToBase64String(txtBytes);
    }

   
}

