var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add cors domains
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_myAllowSpecificOrigins",
    builder =>
    {
        builder.WithOrigins(
            "https://www.scrapcatapp.com",
            "https://test.scrapcatapp.com",
            "https://staging.scrapcatapp.com",
            "http://localhost"
        );
    });
});
IConfiguration configuration = builder.Configuration;
// Initialize configs on AppSettings.Json
builder.Services.Configure<scabackend.Settings.MySQLSettings>
    (configuration.GetSection("MySQLSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
