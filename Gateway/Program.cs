using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// ���������� Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot();

// ��������� ����� ����� Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 8080);
});

var app = builder.Build();

app.UseOcelot().Wait();

app.Run();
