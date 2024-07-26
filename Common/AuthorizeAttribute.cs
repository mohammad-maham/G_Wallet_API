﻿using Accounting.BusinessLogics.IBusinessLogics;
using Accounting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace Accounting.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IAuthentication? _auth;
        private readonly ILogger<AuthorizeAttribute>? _logger;

        public AuthorizeAttribute()
        {
            ILoggerFactory? factory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            ILogger<AuthorizeAttribute>? logger = factory.CreateLogger<AuthorizeAttribute>();
            _logger = logger;
            _auth ??= (new AuthenticationService());
        }

        public AuthorizeAttribute(ILogger<AuthorizeAttribute> logger, IAuthentication auth)
        {
            _logger = logger;
            _auth = auth;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            StringValues user = context.HttpContext.Request.Headers[HeaderNames.Authorization];

            AuthenticationHeaderValue.TryParse(user, out AuthenticationHeaderValue? headerValue);

            if (headerValue == null || headerValue.Parameter == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized!" })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                bool isValidToken = _auth!.VerifyTokenAsync(headerValue.Parameter).Result;
                if (!isValidToken)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized!" })
                    { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
        }
    }
}
