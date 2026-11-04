using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RSVPApi.DataAccess;
using RSVPApi.Models;

namespace RSVPApi.Controllers;

/// <summary>
/// Filter that checks the Basic Authentication header against
/// the users stored in the database.
/// </summary>
public class BasicAuthentication : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Check to be sure the request has an Authorization header
        string authHeader = context.HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // The value should be in the format "Basic <base64-encoded-credentials>"
        string[] authHeaderParts = authHeader.Split(' ');
        if (authHeaderParts.Length != 2 || authHeaderParts[0] != "Basic")
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string credentials;
        try
        {
            // Decode the base64-encoded credentials
            credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderParts[1]));
        }
        catch (FormatException)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // The credentials should be in the format "email:password"
        int splitIndex = credentials.IndexOf(':');
        if (splitIndex <= 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string email = credentials[..splitIndex];
        string password = credentials[(splitIndex + 1)..];

        // Check the login against the Users table
        User? user = new RsvpRepository().ValidateUser(email, password);
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Save the user so the controller can send it back
        context.HttpContext.Items["AuthUser"] = user;
        base.OnActionExecuting(context);
    }
}