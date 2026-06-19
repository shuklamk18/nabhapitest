using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace APIsNABH.Attributes
{
	public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
	{
		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) { }

		public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
		{
			if (policyName != null)
			{

				if (policyName.StartsWith("Permission:"))
				{
					var permission = policyName.Replace("Permission:", "");
					var policy = new AuthorizationPolicyBuilder().AddRequirements(new PermissionRequirement(permission)).Build();

					return Task.FromResult<AuthorizationPolicy?>(policy);

				}
				return base.GetPolicyAsync(policyName);

			}
			return base.GetPolicyAsync(policyName);


		}
	}
}
