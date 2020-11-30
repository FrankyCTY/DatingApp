using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
  public class ExceptionMiddleware
  {
    private readonly RequestDelegate _next; // pass to next middleware
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env) // ILogger, IHostEnvironment are injected by the {Use} method
    {
      _next = next;
      _env = env;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) // InvokeAsync method will be invoked by the {Use} method in startup.cs middleware part
    {
      try
      {
        await _next(context); // error will propagate from back to top in the middleware pipeline to find the middleware to handle the error (here)
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, ex.Message);
        context.Response.ContentType = "application.json"; // 1. Preparing the error json context
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        // 2. Prepare error json content base on environment mode
        var response = _env.IsDevelopment() ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
        : new ApiException(context.Response.StatusCode, "Internal Server Error");

        // 3. JsonSerializer the error json context with the custom response
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await context.Response.WriteAsync(json); // 4. Send the error json context
      }
    }
  }
}