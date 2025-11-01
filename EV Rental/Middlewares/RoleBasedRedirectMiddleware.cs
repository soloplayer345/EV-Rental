using DataAccessLayer.Enums;
using EV_Rental.Helpers;

namespace EV_Rental.Middlewares
{
    public class RoleBasedRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Bỏ qua các static files và login/register
            if (path.StartsWith("/css") || path.StartsWith("/js") || 
                path.StartsWith("/lib") || path.StartsWith("/account/login") || 
                path.StartsWith("/account/register") || path == "/")
            {
                await _next(context);
                return;
            }

            // Kiểm tra xem user đã đăng nhập chưa
            var userRole = SessionHelper.GetUserRole(context.Session);

            if (string.IsNullOrEmpty(userRole))
            {
                // Chưa đăng nhập, redirect về login
                context.Response.Redirect("/Account/Login");
                return;
            }

            // Parse role
            if (Enum.TryParse<AccountRole>(userRole, out var role))
            {
                // Kiểm tra quyền truy cập theo role
                if (role == AccountRole.Admin)
                {
                    // Admin có thể truy cập mọi nơi
                    await _next(context);
                    return;
                }
                else if (role == AccountRole.Staff)
                {
                    // Staff chỉ được truy cập /Staff/*
                    if (!path.StartsWith("/staff") && path != "/account/logout")
                    {
                        context.Response.Redirect("/Staff/Dashboard");
                        return;
                    }
                }
                else if (role == AccountRole.Renter)
                {
                    // Renter chỉ được truy cập /Renter/*
                    if (!path.StartsWith("/renter") && path != "/account/logout")
                    {
                        context.Response.Redirect("/Renter/Index");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method để dễ dàng sử dụng middleware
    public static class RoleBasedRedirectMiddlewareExtensions
    {
        public static IApplicationBuilder UseRoleBasedRedirect(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RoleBasedRedirectMiddleware>();
        }
    }
}
