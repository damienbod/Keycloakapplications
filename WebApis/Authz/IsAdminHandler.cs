using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApis.Authz;

public class IsAdminHandler : AuthorizationHandler<IsAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdminRequirement requirement)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        if (requirement == null)
            throw new ArgumentNullException(nameof(requirement));

        //var claimIdentityprovider = context.User.Claims.FirstOrDefault(t => t.Type == "idp");

        // TODO check for
        // resource_access": {
        //"oidc-code-pkce-angular": {
        //    "roles": [
        //      "admin"
        // check that our keycloak was used to signin
        //if (claimIdentityprovider != null
        //    && claimIdentityprovider.Value == "...")
        //{
        //    context.Succeed(requirement);
        //}

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}