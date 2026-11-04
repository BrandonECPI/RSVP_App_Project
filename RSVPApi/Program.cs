var builder = WebApplication.CreateBuilder(args);

// Add controllers plus Swagger (the API test page)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Turn on the Swagger page at /swagger
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();