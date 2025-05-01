using WebApplication2;
using WebApplication2.pdf_gen;
using QuestPDF.Infrastructure;
using static WebApplication2.DB_Connection;
using static WebApplication2.FuelType;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddSession();          

var app = builder.Build();


var dbConnection = new DB_Connection();
await dbConnection.Start();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
QuestPDF.Settings.License = LicenseType.Community;
//PdfGen.Generate(1, 2, FuelType.Diesel, 6.00f, 1.20f, 100.0f);
app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();                       
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
