using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TCTravel;

// Reference: https://stackoverflow.com/questions/58618964/dependency-injection-inside-jwtbearerevents
// Override JWTBearerEvents methods to inject loggers to JWT events
public class AppJwtBearerEvents : JwtBearerEvents
{
    private readonly ILogger<AppJwtBearerEvents> _logger;
 
    public AppJwtBearerEvents(ILogger<AppJwtBearerEvents> logger)
    {
        _logger = logger;        
    }

    public override Task AuthenticationFailed(AuthenticationFailedContext context)
    {
        _logger.LogInformation("Authentication failed");
        return base.AuthenticationFailed(context);
    }

    public override Task TokenValidated(TokenValidatedContext context)
    {
        _logger.LogInformation("TokenValidated");
        return base.TokenValidated(context);
    }
}