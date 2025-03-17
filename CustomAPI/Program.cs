var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 5001; // Ensure HTTPS is enabled
});
var app = builder.Build();
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthorization();
app.MapControllers();
app.Run();