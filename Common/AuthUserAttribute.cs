using Microsoft.AspNetCore.Http;
using OnSiteApii.BusinessLogic;
using OnSiteApii.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnSiteApii._Services
{
    public class AuthUserAttribute : AuthorizeAttribute
    {
        public string Role { get; }

        public AuthUserAttribute()
        {
        }

        public AuthUserAttribute(string role)
        {
            Role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext )
        {

            var login = new LoginParams();
            try
            {
                if (httpContext.Request.Headers["Authorization"] != null)
                {
                    var jwt = JwtTokenHelper.ValidateJwtToken(httpContext.Request.Headers["Authorization"].ToString(), out login);
                    if (jwt)
                        httpContext.Items.Add("login", login);
                    return jwt;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log(login.UserId, "AuthUserAttribute", ex.InnerException.ToString());
                throw;
            }

        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}