using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.BlogRepository;
using HealthyFoodWebsite.Repositories.LoggerRepository;
using HealthyFoodWebsite.Repositories.OrderRepository;
using HealthyFoodWebsite.Repositories.ProductRepository;
using HealthyFoodWebsite.Repositories.ShoppingBag;
using HealthyFoodWebsite.Repositories.TestimonialRepository;
using HealthyFoodWebsite.EmailTemplate;
using HealthyFoodWebsite.Hubs;
using HealthyFoodWebsite.Repositories.BlogSubscriberRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Mail;
using System.Net;
using HealthyFoodWebsite.Repositories.ContactUsRepository;
using Microsoft.AspNetCore.Builder;
using System;
using HealthyFoodWebsite.Application_Services.MigrationsApplier;

namespace HealthyFoodWebsite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            builder.Services.AddDbContext<HealthyFoodDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultString"));
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AbstractLoggerRepository, LoggerRepository>();
            builder.Services.AddScoped<AbstractProductRepository, ProductRepository>();
            builder.Services.AddScoped<AbstractTestimonialRepository, TestimonialRepository>();
            builder.Services.AddScoped<AbstractBlogPostRepository, BlogPostRepository>();
            builder.Services.AddScoped<AbstractBlogSubscriberRepository, BlogSubscriberRepository>();
            builder.Services.AddScoped<AbstractContactUsRepository, ContactUsRepository>();
            builder.Services.AddScoped<AbstractShoppingBagRepository, ShoppingBagRepository>();
            builder.Services.AddScoped<AbstractOrderRepository, OrderRepository>();
            builder.Services.AddFluentEmail("mostafaashrafelsersi@gmail.com")
                .AddSmtpSender(new SmtpClient
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("mostafaashrafelsersi@gmail.com", "irltatbdxctqaczz"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Host = "smtp.gmail.com",
                    Port = 587
                })
                .AddRazorRenderer();
            builder.Services.AddSignalR(options =>
            {
                options.DisableImplicitFromServicesParameters = true;
            });
            builder.Services.AddScoped<EmailFactory>();
            builder.Services.AddScoped<ImageUploader.ImageUploader>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/GetNotFoundPage";
                    await next();
                }
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=GetView}/{id?}");

            app.MapHub<OrderHub>("/order-hub");

            app.ApplyMigrationsIfNot();

            app.Run();
        }
    }
}