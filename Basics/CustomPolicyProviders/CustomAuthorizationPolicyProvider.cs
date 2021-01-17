using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basics.CustomPolicyProviders
{

    /// <summary>
    /// Custom Attribute 
    /// </summary>
    public class SecurityLevelAttribute : AuthorizeAttribute
    {
        public SecurityLevelAttribute(int level)
        {
            //sets the name of the policy into a structure that we're expecting
            Policy = $"{DynamicPolicies.SecurityLevel}.{level}";
        }
    }

    //Type
    public static class DynamicPolicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }

        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";
    }

    public static class DynamicAuthPolicyFactory
    {

        public static AuthorizationPolicy Create(string policyName)
        {
            var parts = policyName.Split('.');
            var type = parts[0];
            var value = parts[1];
            switch (type)
            {
                case DynamicPolicies.Rank:
                    return new AuthorizationPolicyBuilder()
                              .RequireClaim("Rank", value)
                              .Build();
                case DynamicPolicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                              .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value)))
                              .Build();

                default: return null; 
            }
        }
    }

    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; }

        public SecurityLevelRequirement(int level)
        {
            Level = level;
        }

        public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SecurityLevelRequirement requirement)
            {
                var claimValue = context.User.Claims.FirstOrDefault(x => x.Type == DynamicPolicies.SecurityLevel)?.Value ?? "0";

                if(Convert.ToInt32(claimValue) >= requirement.Level)
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }

        }

    }
    public class CustomAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {

        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {

        }

        /// <summary>
        /// GetPolicyAsync is what gets called when our middleware tries to authorize and looks at the policy
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            //we're expecting policy to be structured {type}.{value}
            //look at the policyname and see if it's something we expect, if so, create an authorization policy that has requirements
            foreach(string customPols in DynamicPolicies.Get())
            {

                if(policyName.StartsWith(customPols))
                {
                    var policy = DynamicAuthPolicyFactory.Create(policyName);

                    return Task.FromResult(policy);

                }

            }

            return base.GetPolicyAsync(policyName);
        }
    }
}
