using AutoMapper.Configuration.Annotations;
using Lab3.Exceptions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace Lab3.MiddleWare
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }
        public async Task HandleException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "internal server error";
            switch (ex)
            {
                case NotFoundException:
                    statusCode = 404;
                    message = ex.Message;
                    break;

                case BadRequestException:
                    statusCode = 400;
                    message = ex.Message;
                    break;
                case UnauthorizedException:
                    statusCode = 401;
                    message = ex.Message;
                    break;
            }
            _logger.LogError(ex, "[ExceptionMiddleware] An unhandled exception occurred while processing the request Path:{Path},Method :{Method}.", context.Request.Path, context.Request.Method);

            var response = new
            {
                StatusCode = statusCode,
                Message = message,
                Details = _env.IsDevelopment() ? ex.Message : null
            };
            context.Response.StatusCode = statusCode;
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
