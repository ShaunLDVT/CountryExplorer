using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CountryExplorer.API.Middleware
{
	public class GlobalExceptionHandlerMiddleware: IMiddleware
	{
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				context.Response.ContentType = "application/json";

				ProblemDetails details = new ProblemDetails()
				{
					Status = context.Response.StatusCode,
					Title = "Server Error",
					Type = "Server Error",
					Detail = "An internal server error has occurred.",
				};

				var json = JsonSerializer.Serialize(details);

				await context.Response.WriteAsync(json);
			}
		}
	}
}
