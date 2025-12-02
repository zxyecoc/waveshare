using Rental.Data.interfaces; // ���� using
using Rental.Data; // ������ ����, �� ����������� AppDBContent
using Microsoft.EntityFrameworkCore; // ��� ���������� �� ���� �����
using Rental.Data.Repository;
using Rental.Data.Models; // ������ ���� ��� �������
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������������ � ����� dbsettings.json
IConfigurationRoot _confString = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("dbsettings.json")
    .Build();

// ��������� �����
builder.Services.AddMvc(options => options.EnableEndpointRouting = false); // ��������� �������� MVC ��� ������������� �� ����� �����

// ϳ��������� �� ���� �����
builder.Services.AddDbContext<AppDBContent>(options =>
    options.UseSqlServer(_confString.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<AppDBContent>();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDBContent>()
    .AddDefaultTokenProviders();

// ������ ���� ������
builder.Services.AddTransient<IAllCars, CarRepository>();
builder.Services.AddTransient<ICarsCategory, CategoryRepository>();
builder.Services.AddTransient<IAllOrders, OrdersRepository>();

// ��������� �������� ��� IHttpContextAccessor, ���� � �������
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped(sp => RentalCart.GetCart(sp));

builder.Services.AddMemoryCache(); // ��� ������������ ���������
builder.Services.AddSession(); // ��������� �������� ����

var app = builder.Build();

// ������������ ������� ������� ������
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // ������� ������������ ���� ����� ������������� �������������

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "carDetails",
    pattern: "CarDetails/{id:int}",
    defaults: new { controller = "CarDetails", action = "Index" });

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "categoryFilter",
    pattern: "{controller=Cars}/{action}/{category?}");

using (var scope = app.Services.CreateScope())
{
    AppDBContent content = scope.ServiceProvider.GetRequiredService<AppDBContent>();
    UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await DBObjects.Initial(content, userManager, roleManager);
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContent>();
    await SeedData.Initialize(db);
}

app.Run();