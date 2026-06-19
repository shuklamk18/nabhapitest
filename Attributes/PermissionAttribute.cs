using Microsoft.AspNetCore.Authorization;

namespace APIsNABH.Attributes
{
	public class PermissionAttribute : AuthorizeAttribute
	{
		public PermissionAttribute(string permission) {

			Policy = $"Permission:{permission}";
		}
	}

	public class PermissionRequirement : IAuthorizationRequirement
	{
		public string permission { get;  }
		public PermissionRequirement(string permission) { 
			this.permission = permission;
		}
	}

	public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			if (context.User.HasClaim("premissions", requirement.permission))
			{
				context.Succeed(requirement);
			}
			return Task.CompletedTask;
		}
	}
}
