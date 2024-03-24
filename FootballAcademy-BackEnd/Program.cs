
using Carter;
using FluentValidation;
using FootballAcademy_BackEnd.Database;
using FootballAcademy_BackEnd.Extensions;
using FootballAcademy_BackEnd.Providers;
using FootballAcademy_BackEnd.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;
var AppKey = builder.Configuration.GetValue<string>("SiteSettings:AppKey");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<FootballAcademyDBContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

JWTStartupConfig.ConfigureJWt(builder.Services, builder.Configuration);


builder.Services.AddSingleton<JWTProvider>(new JWTProvider(AppKey));

builder.Services.AddAuthorization();


builder.Services.AddScoped<Authprovider>(services =>
{
    var scope = services.GetRequiredService<IServiceScopeFactory>();
    return new Authprovider(scope);
});

Paginator.SetHttpContextAccessor(builder.Services.BuildServiceProvider().GetService<IHttpContextAccessor>());

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSwaggerGen(option => SwaggerDoc.OpenAuthentication(option));

builder.Services.AddMediatR(config => config.RegisterServicesFromAssemblies(assembly));

builder.Services.AddCarter();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

builder.Services.AddValidatorsFromAssembly(assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapCarter();
app.UseHttpsRedirection();


app.Run();
