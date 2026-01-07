using App.Api.Configuration;
using App.Api.Middleware;
using App.Api.Validation;
using App.Api.Validation.DynamicQuestionnaire;
using App.Application.Dto.AuthenticationModule;
using App.Application.Interfaces.Repositories;
using App.Application.Interfaces.Repositories.EmailFactory;
using App.Application.Interfaces.Services;
using App.Application.Interfaces.Services.AuthenticationModule;
using App.Application.Interfaces.Services.Email;
using App.Application.Interfaces.Services.EmailFactory;
using App.Application.Profiles;
using App.Application.Profiles.MasterDatabase;
using App.Application.Profiles.Organization;
using App.Application.Profiles.UserMapper;
using App.Application.Service;
using App.Application.Service.AuthenticationModule;
using App.Application.Service.EmailConfiguration;
using App.Application.Service.EmailProviders;
using App.Application.Service.MasterAdmin;
using App.Common.Constant;
using App.Domain.Entities.EmailFactory;
using App.Infrastructure.Repository.EmailFactory;
using App.Infrastructure.Repository.MasterAdmin;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add CORS policy (add this before AddControllers)
        //builder.Services.AddCors(options =>
        //{
        //    options.AddPolicy("AngularHost",
        //        builder => builder
        //            .WithOrigins("http://localhost:4200", "https://ms.stagingsdei.com:4165")
        //            .AllowAnyMethod()
        //            .AllowAnyHeader()
        //            .AllowCredentials());
        //});
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
        var myEnvironmentVariable = builder.Configuration["ASPNETCORE_ENVIRONMENT"];
        builder.Configuration.AddJsonFile($"appsettings.{myEnvironmentVariable}.json", optional: true, reloadOnChange: true);



        // Initialize static encryption config
        EncryptionConfig.SetConfiguration(builder.Configuration);


        builder.Services.AddAppDependencies();
        builder.Services.AddConfiguredDbContexts(builder.Configuration);

        // AutoMapper Configuration
        builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(DynamicQuestionnaireProfile), typeof(OrganizationMappingProfile), typeof(MasterDatabaseMappingProfile), typeof(UserProfile));

        // Current User Claim Service
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserClaimService, CurrentUserClaimService>();

        //End 
        //Email Factory
        // Register config
        builder.Services.Configure<EmailProviderConfigs>(
            builder.Configuration.GetSection("EmailProviderConfig"));
        builder.Services.AddSingleton(resolver =>
            resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailProviderConfigs>>().Value);

        builder.Services.AddScoped<IEmailProviderConfigRepository, EmailProviderConfigRepository>();

        // Register Email Configuration Service
        builder.Services.AddScoped<IEmailConfigurationService, EmailConfigurationService>();

        // Register services
        builder.Services.AddScoped<IAdminService, AdminService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IEmailProvider, SmtpEmailProvider>(); // default provider
        builder.Services.AddScoped<SmtpConfig>();
        builder.Services.AddScoped<AwsSesConfig>();
        builder.Services.AddScoped<SendGridConfig>();
        builder.Services.AddScoped<AzureConfig>();
        builder.Services.AddScoped<IAdminService, AdminService>();

        // Repositories
        builder.Services.AddScoped<IAdminRepository, AdminRepository>();

        // Authentication service
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


        builder.Services.AddControllers(options =>
        {

           
        });
        //options.Filters.Add<DateTimeConversionFilterService>();
        builder.Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        // Register all validators
        builder.Services.AddValidatorsFromAssemblyContaining<UsersValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateFormValidator>();

        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.Configure<ThirdPartyDto>(builder.Configuration.GetSection("Authentication:Google"));


        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });


        //    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //.AddJwtBearer(options =>
        //{
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,

        //        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        //        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        //        IssuerSigningKey = new SymmetricSecurityKey(
        //            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
        //        ),

        //        RoleClaimType = ClaimTypes.Role // Important for role-based auth
        //    };
        //});

        //    builder.Services.AddAuthorization();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
                ),
                RoleClaimType = ClaimTypes.Role
            };
        })
        .AddJwtBearer("SuperAdminScheme", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["SuperAdminJwtSettings:Issuer"],
                ValidAudience = builder.Configuration["SuperAdminJwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["SuperAdminJwtSettings:Key"])
                ),
                RoleClaimType = ClaimTypes.Role
            };
        });

        builder.Services.AddAuthorization();
        /////////////

        //// AWSSDK S3 (uncomment this for aws s3 bucket access)
        //builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

        //builder.Services.AddAWSService<IAmazonS3>();

        //builder.Services.AddSingleton<IAwsServices, AwsServices>();


        ////Azure Blob storage
        //builder.Services.Configure<AzureBlobStorageSettings>(
        //builder.Configuration.GetSection("AzureBlobStorage"));

        var app = builder.Build();
        app.UseCors("AllowAll");
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        //app.UseMiddleware<JwtMiddleware>();
        //app.UseMiddleware<SuperAdminJwtMiddleware>();
        app.UseWhen(context => context.Request.Path.StartsWithSegments("/super-admin"),
    appBuilder =>
    {
        appBuilder.UseMiddleware<SuperAdminJwtMiddleware>();
    });

        app.UseWhen(context => !context.Request.Path.StartsWithSegments("/super-admin"),
        appBuilder =>
        {
            appBuilder.UseMiddleware<JwtMiddleware>();
        });
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseStaticFiles();

        if (!Directory.Exists("Images"))
            Directory.CreateDirectory("Images");
        //following code is used for access the Images
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Images")),
            RequestPath = "/Images"
        });

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

