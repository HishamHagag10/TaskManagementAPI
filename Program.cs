using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using TaskManagement.API.Services.NotificationService;
using TaskManagement.API.Services.UserService;
using TaskManagementAPI.Authentication;
using TaskManagement.API.Configuration;
using TaskManagementAPI.Erorrs;
using TaskManagementAPI.Helpers;
using TaskManagementAPI.MeddleWare;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Helpers.Validators.DatabaseValidators;
using TaskManagement.API.Services.Dashboard_Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var constr=builder.Configuration.GetConnectionString("constr");

builder.Services.AddDbContext<AppDbContext>(
    option => option.UseSqlServer(constr)
    ) ;

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false, // For Dev
    ValidateAudience = false, // For Dev 
    RequireExpirationTime = false, // For Dev I will change it
    ValidateLifetime = true
};
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt =>
    {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParameters;
    });
builder.Services.AddIdentity<User, IdentityRole>(option => {
    
    option.SignIn.RequireConfirmedEmail = true;
    
    option.Tokens.EmailConfirmationTokenProvider = 
    TokenOptions.DefaultEmailProvider;
    
    option.Tokens.PasswordResetTokenProvider =
    TokenOptions.DefaultEmailProvider;


}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();

builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<NotificationsManager>();
builder.Services.AddScoped<ITaskNotificationService,TaskNotificationService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IDTOValidator,DTOValidator>();
builder.Services.AddScoped<IDashboardService,DashbaordService>();
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context=>
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString()??"unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientId, par =>
          new FixedWindowRateLimiterOptions
          {
              PermitLimit=20,
              Window = TimeSpan.FromMinutes(1),
              QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
              QueueLimit=0,
          }
        );
    });
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers["Retry-After"] = "60";
        await context.HttpContext.Response.WriteAsync("Guide to Secure User Authentication - Exploring Identity");
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options=>{
    options.InvalidModelStateResponseFactory = actioncontext =>
    {
        var errors = actioncontext.ModelState
        .Where(x=>x.Value!=null && x.Value.Errors.Any())
        .SelectMany(x=>x.Value!.Errors)
        .Select(x=>x.ErrorMessage).ToList();

        var response = new ApiValidationErrorResponse
        {
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    };
});

var hangfireConstr = builder.Configuration.GetConnectionString("hangfireConstr");
// Hnagfire Client
builder.Services.AddHangfire(config=>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseSqlServerStorage(hangfireConstr));

// Hangfire Server
builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    try
    {
        await Helper.SeedAsync(service);

    }catch(Exception ex) 
    {
        Console.WriteLine($"An error occurred while seeding roles: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/error/{0}");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();
app.MapHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<TaskNotificationService>
                    ("Send-deadline-notification",
                    x => x.SendTaskDeadlineApproachingEmailAsync(),
                    Cron.Daily(9,0));

//RecurringJob.AddOrUpdate(() => Console.WriteLine("Hello to hangfire"),"* * * * *");

app.Run();
