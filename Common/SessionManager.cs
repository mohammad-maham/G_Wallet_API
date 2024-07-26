using G_APIs.BussinesLogic.Interface;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace G_APIs.Common;

public static class SessionManager
{
    public static void Set<T>(this IHttpContextAccessor session, string key, T value)
    {
        session.HttpContext.Session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T? Get<T>(this IHttpContextAccessor session, string key)
    {
        var value = session.HttpContext.Session.GetString(key);

        return value == null ? default :
            JsonConvert.DeserializeObject<T>(value);
    }

}

