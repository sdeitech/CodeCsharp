using App.Common.Middleware;
using App.SuperAdmin.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppDependencies();
builder.Services.AddConfiguredDbContexts(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
//builder.Services.AddFluentValidationAutoValidation()
//    .AddFluentValidationClientsideAdapters();
//builder.Services.AddValidatorsFromAssemblyContaining<UsersValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
