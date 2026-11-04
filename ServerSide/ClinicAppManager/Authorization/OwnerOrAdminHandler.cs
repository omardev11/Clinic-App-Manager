using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ClinicAppManager.Authorization
{
    public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OwnerOrAdminRequirement requirement,
            string AspNetUserId)
        {
            if (context.User.IsInRole("user") || context.User.IsInRole("admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(userId) &&
                !string.IsNullOrWhiteSpace(AspNetUserId) &&
                userId == AspNetUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
