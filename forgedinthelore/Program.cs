using forgedinthelore_net.Data;
using forgedinthelore_net.Extensions;
using forgedinthelore_net.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services UNDER this line
//Extension to put all custom services in. Only add services to program.cs if they are not made by us.
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddCors();

builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//TODO: Move to SwaggerExtension
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo {Title = "Restaurant API", Version = "v1"});
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
//Add services ABOVE this line
var app = builder.Build();


if (args.Length == 1 && args[0].ToLower() == "seed")
{
    using var scope = app.Services.CreateScope();
    await Seed.Run(scope);
}

//Configure app UNDER this line
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Websocket config
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

//Request config
// app.UseHttpsRedirection();
app.UseCors(policy => policy
    .WithOrigins("https://localhost:4200","http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Add different endpoints
app.MapControllers();
// app.MapHub<HubClass>("hubs/path");

app.Run();