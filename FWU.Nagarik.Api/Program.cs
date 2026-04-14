using Microsoft.EntityFrameworkCore;
using FWU.Nagarik.Api.Data;
using FWU.Nagarik.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentService, StudentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

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
});

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
});

app.Run();