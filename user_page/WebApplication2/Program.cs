using WebApplication2;
using WebApplication2.pdf_gen;
using QuestPDF.Infrastructure;
using Stripe;
using DotNetEnv;
using static WebApplication2.DB_Connection;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Optional: tweak cookie name, idle timeout, etc.
    options.Cookie.Name = ".WebApp2.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});


builder.Services.AddSingleton<DB_Connection>();
builder.Services.AddRazorPages();
builder.Services.AddSession();    



Env.Load();                                     

var stripeSecret = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                 ?? Environment.GetEnvironmentVariable("Stripe_secret_key"); 
var stripePub    = Environment.GetEnvironmentVariable("Stripe_key");

if (string.IsNullOrWhiteSpace(stripeSecret))
    throw new InvalidOperationException("Stripe secret key not found in environment.");

StripeConfiguration.ApiKey = stripeSecret;      

builder.Configuration["Stripe:PubKey"] = stripePub ?? string.Empty;



var app = builder.Build();



var dbConnection = new DB_Connection();




QuestPDF.Settings.License = LicenseType.Community;


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();                       // force https in Production
}
else
{
    app.UseDeveloperExceptionPage();     // nice error page
    
}



app.UseHttpsRedirection();               
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();



app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();


