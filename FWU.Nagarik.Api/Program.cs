using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FWU Nagarik API v1");
    options.RoutePrefix = string.Empty;
});

app.MapGet("/api/student/verify", async (
    string registration_number,
    string dobAD,
    IStudentService studentService) =>
{
    if (string.IsNullOrWhiteSpace(registration_number))
        return Results.BadRequest(new { message = "registration_number is required" });

    if (string.IsNullOrWhiteSpace(dobAD))
        return Results.BadRequest(new { message = "dobAD is required" });

    var result = await studentService.VerifyStudentAsync(registration_number, dobAD);

    if (result == null)
        return Results.NotFound(new { message = "No record found for the given registration number / DOB" });

    return Results.Ok(result);
})
.WithName("VerifyStudent");

app.MapGet("/api/student/transcript", async (
    string registration_number,
    string dobAD,
    IStudentService studentService) =>
{
    if (string.IsNullOrWhiteSpace(registration_number))
        return Results.BadRequest(new { message = "registration_number is required" });

    if (string.IsNullOrWhiteSpace(dobAD))
        return Results.BadRequest(new { message = "dobAD is required" });

    var result = await studentService.GetTranscriptAsync(registration_number, dobAD);

    if (result == null)
        return Results.NotFound(new { message = "No record found for the given registration number / DOB" });

    return Results.Ok(result);
})
.WithName("GetTranscript");

app.Run();