using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Infrastructure.Persistence.ServiceHelpers
{
    public static class UrlHelperExtention
    {
        // public static string EmailConfirmationLink(this IUrlHelper urlHelper,string controller ,Guid userId, string code, string scheme)
        // {
        //     return urlHelper.Action(
        //         action: controller,
        //         controller: "Users",
        //         values: new { userId, code },
        //         protocol: scheme)!;
        // }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper,string controller ,Guid userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: controller!,
                controller: "Users",
                values: new { userId, code },
                protocol: scheme)!;
        }
    }
}