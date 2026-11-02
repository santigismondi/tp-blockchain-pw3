using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TpBlockchainPW3.Web.Data;
using TpBlockchainPW3.Web.Services;
using TpBlockchainPW3.Web.Services.Interfaces;
using TpBlockchainPW3.Web.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<TpBlockchainPW3.Web.Areas.Identity.IdentityErrorDescriberEs>(); // errores de Identity en español
builder.Services.AddControllersWithViews(options =>
{
    // Mensajes de validación genéricos de ASP.NET en español (valen también para las Razor Pages de Identity)
    var mensajes = options.ModelBindingMessageProvider;
    mensajes.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor de «{campo}».");
    mensajes.SetMissingKeyOrValueAccessor(() => "Falta un valor obligatorio.");
    mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "Faltan los datos del formulario.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"«{valor}» no es un valor válido para {campo}.");
    mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"«{valor}» no es un valor válido.");
    mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor ingresado en {campo} no es válido.");
    mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor ingresado no es válido.");
    mensajes.SetValueIsInvalidAccessor(valor => $"«{valor}» no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"{campo} tiene que ser un número.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El valor tiene que ser un número.");
});

builder.Services.AddScoped<IPdfService, QuestPdfService>();
builder.Services.Configure<BlockchainOptions>(builder.Configuration.GetSection(BlockchainOptions.Seccion));
builder.Services.AddSingleton<IHashService, Sha256HashService>();
builder.Services.AddSingleton<IBlockchainService, NethereumBlockchainService>();
builder.Services.AddScoped<IDocumentoService, DocumentoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
