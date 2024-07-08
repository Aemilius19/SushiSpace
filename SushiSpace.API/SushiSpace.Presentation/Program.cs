using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

using SushiSpace.Application.Data.Context;
using SushiSpace.Application.Mapper;
using SushiSpace.Application.Repositories.Abstractions;
using SushiSpace.Application.Repositories.Implementations;
using SushiSpace.Bussines.Services.Abstractions;
using SushiSpace.Bussines.Services.Concretes;
using SushiSpace.Bussines.Services.Concretes.SushiSpace.Bussines.Services.Concretes;
using SushiSpace.Core.DTOs;
using SushiSpace.Core.Entities;
using SushiSpace.Core.Helper;
using SushiSpace.Core.Helper.Mail;
using System.Text.Json.Serialization;

namespace SushiSpace.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                
                
                
                
                
                .AddFluentValidation(x => x.RegisterValidatorsFromAssembly(typeof(ProductDTOValidator).Assembly));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore-swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<SushiSpaceDb>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("default"));
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAutoMapper(typeof(MapProfile).Assembly);

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderProductRepository, OrderProductRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddTransient<IMailService, MailService>();

            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IProductServices, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            }).AddEntityFrameworkStores<SushiSpaceDb>().AddDefaultTokenProviders();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Время жизни cookie
                    options.LoginPath = "/User/Login"; // Путь к странице логина
                    options.AccessDeniedPath = "/User/AccessDenied"; // Путь к странице доступа запрещен
                    options.LogoutPath = "/User/Logout";
                });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.LoginPath = "/User/Login";
                options.AccessDeniedPath = "/User/AccessDenied";
                options.SlidingExpiration = true;
            });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();
            app.UseCors(policy =>
                policy.WithOrigins("http://localhost:5173/", "https://localhost:7152/")
                    .AllowAnyMethod()
                    .WithHeaders(HeaderNames.ContentType)
            );

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSession(); // Эта строка должна идти перед UseAuthentication и UseAuthorization

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
           

            app.MapControllers();

            app.Run();
        }
    }
}
