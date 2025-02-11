using DataAccess.Database;
using DataAccess.Repository;
using DataAccess.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using PetShopClient.Helper;
using PetShopClient.Util.Implement;
using PetShopClient.Util.Interface;
using System.Net;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PetShopContext>();

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderDetailsService, OrderDetailsService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPetServiceService, PetServiceService>();
builder.Services.AddScoped<IPetService, PetService>();
builder.Services.AddScoped<ISpeciesService, SpeciesService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPlannedServiceService, PlannedServiceService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<ICaseService, CaseService>();

builder.Services.AddScoped<IPayPalService, PayPalService>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1200);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Access/Login";
        options.LogoutPath = "/Access/Logout";
        options.AccessDeniedPath = "/Access/AccessDenied";
    });

// Configure email service using FluentEmail and Gmail SMTP
builder.Services.AddFluentEmail("phamnhan.27122000@gmail.com", "Confirm Email")
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient("smtp.gmail.com", 587)
    {
        Credentials = new NetworkCredential("phamnhan.27122000@gmail.com", "kqnw txys wkuo kdts"),
        EnableSsl = true
    });
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<SendEmail>();

// Register IUrlHelper using ActionContextAccessor
builder.Services.AddScoped<IUrlHelper>(x =>
{
    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
    return new UrlHelper(actionContext);
});
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action}/{id?}",
    defaults: new { controller = "Admin", action = "Main" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

