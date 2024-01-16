# メモ

---

## VS Code の環境構築

[チュートリアル](https://learn.microsoft.com/ja-jp/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio-code)

### Project を作る

`dotnet new webapi -o TodoApi -f net7.0`
`cd TodoApi`
`dotnet add package Microsoft.EntityFrameworkCore.InMemory -v 7.0.0`

### Solution を作る

`dotnet new sln`

### Solution にプロジェクトを追加する

`dotnet sln add <プロジェクトパス>`

---

## VS Code でAPIをテスト実行する

- `dotnet run --launch-profile https`
- 上記を実行後に、<https://localhost:portnumber/swagger>

---

## Swagger

Swagger is a popular tool for docmenting APIs and prividing a user friendly interface for testing and exploring the APIs.

---

## パッケージの追加

`dotnet add package [パッケージ名]`

- e.g. dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 7.0.0

---

## Microsoft.EntityFrameworkCore.Tools

This is the package that is responsible to run migrations, which will later on create a database for us.

---

## DbContext Class

- Maintaining Connection To Db
- Track Changes
- Perform CRUD operations
- Brige between domain models and the database

A Dbcontext class is a class that represents a session with a database and provides a set of APIs for performing database operations.  
It also provides a way to define the database schema using entity classes or domain classes that we just built, which maps directly to database tables.  
So we can say that the Dbcontext class is a bridge between your domain model classes and the database.  

Controller <--> DbContext <--> Database

---

## Mac で Sqlserver は Docker にインストールする必要がある

- イメージのダウンロード
  - `docker pull mcr.microsoft.com/azure-sql-edge`
- コンテナの起動
  - `docker run --cap-add SYS_PTRACE -e 'ACCEPT_EULA=1' -e 'MSSQL_SA_PASSWORD=Password.1' -p 1433:1433 --name azuresqledge -d mcr.microsoft.com/azure-sql-edge`

---

## SqlServer の Docker-Compose を作成して、git にあげた

[github](https://github.com/hogendan/Docker-SqlServer)

---

## Run EF Core Migration

Package manager コンソールから以下を実行する

1. Add-Migration "Name Of Migration"
   1. ex: Add-Migration "Initial Migration"
   2. 複数のDbContextがある場合は、-Context パラメータで明示的にDbContextを指定する必要がある
      1. ex: Add-Migration "Initial Migration" -Context "TMWalkAuthDbContext"
2. Update-Database
   1. こちらも複数のDbContextがある場合は、-Context で明示的に指定する必要がある。

上記は VS Code ではできなかったので、以下のコマンドで実行する
[参考](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
[参考(続き)](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

1. dotnet tool install --global  dotnet-ef --version 7.0
2. dotnet tool update --global dotnet-ef --version 7.0
3. cd TMWalks/TMWalk.API
   1. プロジェクトファイルがあるフォルダに移動する
4. dotnet ef migrations add "Initial Migration"
   1. 複数DbContextがある場合は以下のように -c を使用して明示的に指定する。
      1. dotnet ef migrations add "Creating Auth Database" -c "TMWalksAuthDbContext"
5. dotnet ef database update
   1. ここで色々エラーが出て1つずつ潰していった
      1. ConnectionString を以下のように変更して解決
         1. "Server=localhost;Database=TMWalksDb;TrustServerCertificate=True;User ID=sa;Password=Password.1"

---

## Controller クラスの属性について

### ApiController

ApiController atribute will tell this application that this controller is for API use.  
こういうの↓

``` c#
[ApiController]
public class RegionsController : ControllerBase
{
}
```

### Route

The Route attribute is basically defining the route whenever a user enters this route along with the application URL, it will be pointed to the region's controller.  
例：<https://localhost:1234/api/regions>

``` c#
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
}
```

---

## DTOs

Data Transfer Objects

- User to transfer data between different layers
- Typically contain a subset of the properties in the domain model
- For example transferring data over a network

### DTOs vs Domain Models

Entityframework core domain models hava a mapping between the tables in the database and the domain models that are used internally in the application.  
When you use entity framework to talk to a table, it gives us the domain model because the DbContext class only knows about the domain models, but because we have the domain models within the API, it is a good practice to add a layer of DTOs.  
That is the data transfer objects and that is what we send back to the client.  
So we never send the domain model back to the client, but we send the DTO instead.  

### Advantages Of DTOs

- Separation of Concerns
  - DTOs can be designed to match the business requirements.
- Performance
- Security
- Versioning

---

## Repository Pattern

- Desing pattern to separate the data access layer from the application
- Provides interface without exposing implementation
- Helps create abstraction

The Repository class is responsible for performing CRUD operations.

### Benefits

- Decoupling
  - That is decoupling the data access layer from the rest of the application, which makes it easier to maintain and test the application.
- Consistency
  - Providing a standard interface for accessing data which improves the consistency and readability of the code. Now every connection to the database goes through the repository.
- Performance
- Multiple data source (switchng)
  - We can also improve the performance ot the application by using caching, batching or other optimization techniques supporting multiple data sources, which allows the application to switch between different data sources without affecting the application logic.

## Automapper

DomainModel と DTO のマッピングをしてくれるライブラリ

`dotnet add package Automapper.Extensions.Microsoft.DependencyInjection`

## EF を使用してテストデータを作成する方法

1. DbContext クラスに以下のメソッドを追加する。

``` c#
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data for Difficulities
        var difficulties = new List<Difficulty> {
            new Difficulty {
                Id = Guid.NewGuid(),
                Name = "Easy"
            },
        };
        // Seed difficulties to the database
        modelBuilder.Entity<Difficulty>().HasData(difficulties);
    }

```

2. VS Code の TERMINAL から以下のコマンドを実行する
   1. `dotnet ef migrations add "Seeding data for Difficulties and Regions"`
   2. `otnet ef database update`

## Model Validation と Customer Validation

1. ModelクラスにValidationアノテーションをつける

``` c#
public class AddRegionRequestDto
{
    [Required]
    [MinLength(3, ErrorMessage = "Code has to be a minimum of 3 characters")]
    [MaxLength(3, ErrorMessage = "Code has to be a maxmum of 3 characters")]
    public string Code { get; set; }
    [Required]
    [MaxLength(100, ErrorMessage = "Name has to be a maxmum of 100 characters")]
    public string Name { get; set; }
    public string? RegionImageUrl { get; set; }
}
```

2. ActionメソッドでValidate実行

``` c#
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
      if (ModelState.IsValid) {
        // 省略
      }
      return BadRequest(ModelState);
    }

```

3. Action共通でValidationをさせるために、CustomValidationクラスを作成

``` c#
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid) 
        {
            context.Result = new BadRequestResult();
        }
    }
}
```

4. 作成したValidateModelを使用するために、Actionメソッドで指定して、Validate処理を削除

``` c#
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
        // ModelState.IsValid を削除

        // 省略 
    }
```

## Filtering

URLパラメータからカラム名、フィルタ内容を取得する方法

- Actionメソッド
  - filterOn にフィルターするカラム名を指定
  - filterQuery にフィルタする値を指定

``` c#
    // GET Walks
    // GET: /api/walks?filterOn=Name&filterQuery=Track
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
    {
        var walks = await walkRepository.GetAllAsync(filterOn, filterQuery);

        return Ok(mapper.Map<List<WalkDto>>(walks));
    }
```

- Repository メソッド
  - カラム分 if 文が増えるのがいけてない気がするが。。。
    - 使う側からしたら使いやすいか。WebAPIなのでfilterOnに指定できる名称などの仕様書が公開するはずだし。
  - あと複数カラムの場合はどうするのか。

``` c#
    public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null)
    {
        var walks = dbContext.Walks.Include("Region").Include("Difficulty").AsQueryable();

        if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
        {
            if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = walks.Where(x => x.Name.Contains(filterQuery));
            }
        }

        return await walks.ToListAsync();
    }
``` 

## Sorting

``` c# Controller
    // GET Walks
    // GET: /api/walks?filterOn=Name&filterQuery=Track&isAscending=true
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? filterOn,
                                            [FromQuery] string? filterQuery,
                                            [FromQuery] string? sortBy,
                                            [FromQuery] bool? isAscending)
    {
        var walks = await walkRepository.GetAllAsync(filterOn, filterQuery,sortBy,isAscending ?? true);

        return Ok(mapper.Map<List<WalkDto>>(walks));
    }
```

``` c# Repository
    public async Task<List<Walk>> GetAllAsync(string? filterOn = null,
                                              string? filterQuery = null,
                                              string? sortBy = null,
                                              bool isAcsending = true)
    {
        var walks = dbContext.Walks.Include("Region").Include("Difficulty").AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
        {
            if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = walks.Where(x => x.Name.Contains(filterQuery));
            }
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                walks = isAcsending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
            }
            else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase)) {
                walks = isAcsending ? walks.OrderBy(x => x.LengthInKmj) :walks.OrderByDescending(x => x.LengthInKmj);
            }
        }

        return await walks.ToListAsync();
    }
```

## Pagination

``` c# Controller
    // GET Walks
    // GET: /api/walks?filterOn=Name&filterQuery=Track&isAscending=true&pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? filterOn,
                                            [FromQuery] string? filterQuery,
                                            [FromQuery] string? sortBy,
                                            [FromQuery] bool? isAscending,
                                            [FromQuery] int pageNumber= 1,
                                            [FromQuery] int pageSize = 1000)
    {
        var walks = await walkRepository.GetAllAsync(filterOn, filterQuery,sortBy,isAscending ?? true, pageNumber, pageSize);

        return Ok(mapper.Map<List<WalkDto>>(walks));
    }
```

``` c# Repository
    public async Task<List<Walk>> GetAllAsync(string? filterOn = null,
                                              string? filterQuery = null,
                                              string? sortBy = null,
                                              bool isAcsending = true,
                                              int pageNumber = 1,
                                              int pageSize = 1000)
    {
        var walks = dbContext.Walks.Include("Region").Include("Difficulty").AsQueryable();

        // 省略

        // Pagination
        var skipResults = (pageNumber - 1) * pageSize;

        return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
    }
```

## Authentication & Authorization

### Authentication

- The process to determine a user's Identity
- Username and Password
- By using authentication, we check if we trust the user

### Authorization

- User permission
- Roles, Policies, Claims
- Check if User has ReadOnly or ReadWrite Role

## JWT (JSON Web Tokens)

1. User が website にアクセス
2. ユーザー名とパスワードをAPIサーバーへ送る
3. APIは JWT Token を返す
4. ユーザーは JWT Token を使ってAPIを呼び出す

### Nuget から JWT ライブラリをインストール

- `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer`
- `dotnet add package Microsoft.IdentityModel.Tokens`
- `dotnet add package System.IdentityModel.Tokens.Jwt`
- `dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore`

## Setting Up Auth Database

- Create New Connection String
- Create New DbContext with Roles (Seed Data)
- Inject DbContext and Idetity (ASP.NET Core Identity)
- Run EF Core Migrations

## コード側対応

Auth用ConnectionString, Jwt設定を追加

``` json appsetting.json
  "ConnectionStrings": {
    "TMWalksConnectionString": "Server=localhost;Database=TMWalksDb;TrustServerCertificate=True;User ID=sa;Password=Password.1",
    "TMWalksAuthConnectionString": "Server=localhost;Database=TMWalkAuthDb;TrustServerCertificate=True;User ID=sa;Password=Password.1"
  },
  "Jwt": {
    "Key": "bbusouJSGGededff879879HHgstKOREHATEKITOU",
    "Issuer": "https://localhost:7017",
    "Audience": "https://localhost:7017"
  }

```

Auth用DbContext の Inject 追加, Jwt の Inject 追加

``` c# Program.cs
builder.Services.AddDbContext<TMWalksAuthDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("TMWalksAuthConnectionString"))
);

// 省略

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

// 省略

app.UseAuthentication();
app.UseAuthorization();

```

Authされていないリクエストを弾くように コントローラー修正 (Authorize 属性追加)

``` c# Contoller
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RegionsController : ControllerBase
{
  // 省略
}
```

Auth用DbContextクラス追加

``` c#
public class TMWalksAuthDbContext : IdentityDbContext
{
    public TMWalksAuthDbContext(DbContextOptions<TMWalksAuthDbContext> options) : base(options)
    {
    }
}

// 元々のDbContextと共存させるためにはGeneric(DbContextOptions<TMWalksDbContext>)にしないと実行時エラーになる
public TMWalksDbContext(DbContextOptions<TMWalksDbContext> dbContextOptions) : base(dbContextOptions)
{

}

```

## EF Core Migration To Create Identity Database

1. Auth用DbContextクラスで OnModelCreating を Override する
   1. IdentityRole を追加
   2. Nuget でインストールした Microsoft.AspNetCore.Identity にある
2. Migration 実行
   1. `dotnet ef migrations add "Creating Auth Database" -c "TMWalksAuthDbContext"`
   2. `dotnet ef database update -c "TMWalksAuthDbContext"`

``` c# TMWalksAuthDbContext.cs
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var readerRoleId = "4a947f13-a49d-4882-bec9-e6c700ddf326";
        var writerRoleId = "4420a419-4ac4-4bde-a0e5-97dc4153812f";

        var roles = new List<IdentityRole> {
            new IdentityRole {
                Id = readerRoleId,
                ConcurrencyStamp = readerRoleId,
                Name = "Reader",
                NormalizedName = "Reader".ToUpper()
            },
            new IdentityRole {
                Id = writerRoleId,
                ConcurrencyStamp = writerRoleId,
                Name = "writer",
                NormalizedName = "writer".ToUpper()
            },
        };

        builder.Entity<IdentityRole>().HasData(roles);
    }
```

## Identity 設定を Inject する

``` c# Program.cs
builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("TMWalks")
    .AddEntityFrameworkStores<TMWalksAuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});
```

## Identity の Controller 側実装

登録リクエスト用DTO

``` c# Dto
public class RegisterRequestDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public string[] Roles { get; set; }
}
```

Controller側ポイント

- UserManager を Inject する
- UserManager を使用して、ユーザー作成・ユーザーへのRoleの割当てを行う

``` c# Controller
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> userManager;

    public AuthController(UserManager<IdentityUser> userManager)
    {
        this.userManager = userManager;
    }

    // POST: /api/Auth/Register
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        var identityUser = new IdentityUser
        {
            UserName = registerRequestDto.Username,
            Email = registerRequestDto.Username
        };

        var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);

        if (identityResult.Succeeded)
        {
            // Add roles to this User
            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
            {
                identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                if (identityResult.Succeeded)
                {
                    return Ok("User was registered! Please login.");
                }
            }
        }

        return BadRequest("Something went wrong");
    }
}
```

## Identity を使用した ログイン実装

Controller

``` c# Contoller
    // POST: /api/Auth/Login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

        if (user != null)
        {
            var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (checkPasswordResult)
            {
                return Ok();
            }
        }

        return BadRequest("Username or password incorrect");
    }
```

DTO

``` c#
public class LoginRequestDto
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
```

## JWT Token の生成

``` c# Repository (Interface は省略)
public class TokeRepository : ITokenRepository
{
    private readonly IConfiguration configuration;

    public TokeRepository(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public string CreateJWTToken(IdentityUser user, List<string> roles)
    {
        // Create claims (Token を作るために必要)
        var claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.Email, user.Email));

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

## TokenRepository を Inject して、Token を生成する (Controller側の呼び出し部分)

Inject を追記

``` c# Program.cs
  builder.Services.AddScoped<ITokenRepository, TokeRepository>();
```

Controller から Repository 呼び出し

``` c# AuthController.cs
    // POST: /api/Auth/Login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

        if (user != null)
        {
            var checkPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (checkPasswordResult)
            {
                // Get Roles for this user
                var roles = await userManager.GetRolesAsync(user);
                
                if (roles != null)
                {
                    // Create Token
                    var jwtToken = tokenRepository.CreateJWTToken(user, roles.ToList());
                    
                    var response = new LoginResponseDto
                    {
                        JwtToken = jwtToken
                    };

                    return Ok(response);
                }
            }
        }

        return BadRequest("Username or password incorrect");
    }
```

### Postman を使う時の token 指定方法

1. API URL 指定: e.g. https://localhost:7017/api/Regions
2. Headers タブ
3. KEY欄に `Authorization` を追記
4. VALUE欄に `Bearer [token]` を追記
   1. token は 上述のコード `new JwtSecurityTokenHandler().WriteToken(token)` で生成した値を入力する

## Roles ベースの Authorization

Contollerクラスに対して設定していた Authorize 属性を Action単位にして、それぞれに 許可する Role を指定する。

``` c# RegionController.cs
    [HttpGet]
    [Authorize(Roles = "Reader, Writer")]
    public async Task<IActionResult> GetAll()
    {
    }

    [HttpPost]
    [ValidateModel]
    [Authorize(Roles = "Writer")]
    public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
    {
    }
```

## Swagger に Authorization を追加する

Program.cs の `builder.Services.AddSwaggerGen` を以下のように変更する。

``` c# Program.cs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TM Walks API", Version = "v1" });
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
```

Debug 実行時に起動した Swagger の右上に `Authorization`ボタンがあり、Value欄に以下を入力する。(Postmanと同じ)
`Bearer [token]`

## Image Update用 DomainModel 作成

DomainModel 作成

``` c# DomainModel
public class Image
{
    public Guid Id { get; set; }

    [NotMapped]
    public IFormFile File { get; set; }

    public string FileName { get; set; }
    public string? FileDescription { get; set; }
    public string FileExtension { get; set; }
    public long FileSizeInBytes { get; set; }
    public string FilePath { get; set; }
}
```

DomainModel を DbContext へ追加

``` c# TMWalksDbContext
// 省略

    public DbSet<Image> Images { get; set; }

// 省略
```

EF Migration コマンド実行

1. `dotnet ef migrations add "Adding Images Table" -c "TMWalksDbContext"`
2. `dotnet ef database update -c "TMWalksDbContext"`

## Image Upload Repository 作成と呼び出し

ImageRepositoryインターフェース作成

``` c#
public interface IImageRepository
{
    Task<Image> Upload(Image image);
}
```

ImageRepository実装クラス

- Image は ローカルフォルダ(プロジェクト内のImagesフォルダ)に保存
- 上記の URL を生成するために、 IHttpContextAccessor を Inject
- ローカルフォルダに保存後、DBへ保存
  - DBへはファイルは保存しない。ローカルフォルダへのURLパスを保存する

```c#
public class LocalImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly TMWalksDbContext dbContext;

    public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
                                IHttpContextAccessor httpContextAccessor,
                                TMWalksDbContext dbContext)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }

    public async Task<Image> Upload(Image image)
    {
        var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath,
                                         "Images",
                                         $"{image.FileName}{image.FileExtension}");

        // Upload Image to Local Path
        using var stream = new FileStream(localFilePath, FileMode.Create);
        await image.File.CopyToAsync(stream);

        // https://localhosssst:1234/images/image.jpg

        var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

        image.FilePath = urlFilePath;

        // Add Image to the Images table
        await dbContext.AddAsync(image);
        await dbContext.SaveChangesAsync();

        return image;
    }
}
```

HttpContextAccessor クラスを Inject するための設定を Program.cs に追加  

```c#
builder.Services.AddHttpContextAccessor();
```

Image Upload用DTO

``` c#
public class ImageUploadRequestDto
{
    [Required]
    public IFormFile File { get; set; }
    [Required]
    public string FileName { get; set; }
    public string? FileDescription { get; set; }
}
```

Upload した画像を https アクセスできるように Program.cs を編集する

```c#
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
});
```

## Logging

Serilog というログライブラリを使う。

### コンソールにログ出力

- `dotnet add package Serilog`
- `dotnet add package Serilog.AspNetCore`
- `dotnet add package Serilog.Sinks.Console`

設定と Inject を Program.cs に書く

- ログをコンソールに出力
- 最小レベルは Info

```c#
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
```

Controller で使用する

Inject

```c#
    public RegionsController(TMWalksDbContext dbContext,
                             IRegionRepository regionRepository,
                             IMapper mapper,
                             ILogger<RegionsController> logger)
    {
        this.dbContext = dbContext;
        this.regionRepository = regionRepository;
        this.mapper = mapper;
        this.logger = logger;
    }
```

ログ出力

```c#
    public async Task<IActionResult> GetAll()
    {
        try
        {
            logger.LogInformation("GetAllRegions Action Method was invoked");

            // Get Data From Database - Domain models
            var regionsDomain = await regionRepository.GetAllAsync();

            logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");

            // Return DTOs
            return Ok(mapper.Map<List<RegionDto>>(regionsDomain));
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
    }
```

### テキストファイルにログ出力

以下のライブラリを追加

- `dotnet add package dotnet add package Serilog.Sinks.File`
- プロジェクトフォルダ直下に Logs フォルダ作成

Program.cs 修正

```c#
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/TMWalks_Log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information()
    .CreateLogger();
```

### Global Exception Handler (Actionメソッドに書かなくても例外をキャッチしてログ出力)

例外キャッチクラス作成 ExceptionHandlerMiddleware

- RequestDelegate: リクエスト処理をするためのもの
- 上記に HttpContext を渡すことで「リクエスト処理中」を捉えることができる

```c#
public class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> logger;
    private readonly RequestDelegate next;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        this.logger = logger;
        // RequestDelegate は Requestプロセスの完了を表すタスクを返す。これを使って HTTP Request を処理することができる。
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // httpContext 呼び出し中に何かあったら例外をハンドルして、ログに書く。
            await next(httpContext);
        }
        catch (System.Exception ex)
        {
            var errorId = Guid.NewGuid();

            // Log This Exception
            logger.LogError(ex, $"{errorId} : {ex.Message}");

            // Return A Custom Error Reponse
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var error =new 
            {
                Id = errorId,
                ErrorMessage = "Something went wrong! We are looking into resolving this.",
            };

            await httpContext.Response.WriteAsJsonAsync(error);
        }
    　}
  　}
```

上記Handlerクラスを使うために、Program.cs を修正

```c#
app.UseMiddleware<ExceptionHandlerMiddleware>();
```

## Versioning

- Controller の Route に version番号を追加する。(URLにv1などをつけてアクセスできるようにする)
- Version 毎のActionメソッドを作る。
- ActionメソッドとVersion番号を対応させる
- Swagger をVersion対応させるために、設定クラスを作る。-> ConfigureSwaggerOption.cs

### 必要パッケージ

- `dotnet add package Microsoft.AspNetCore.Mvc.Versioning`
- `dotnet add package Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer`
  - こちらは、Swagger対応で必要

### Controller (CountryController)

- Route にバージョン番号含める
- ApiVersion に全Version指定する
- VersionとActionメソッドを対応させる

```c#
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[ApiController]
public class CountryController : ControllerBase
{
    [MapToApiVersion("1.0")]
    [HttpGet]
    public IActionResult GetV1() 
    {
        var dto = new CountryDtoV1 {
            Id = 1,
            Name = "Japan v1",
        };

        return Ok(dto);
    }

    [MapToApiVersion("2.0")]
    [HttpGet]
    public IActionResult GetV2() 
    {
        var dto = new CountryDtoV2 {
            Id = 1,
            CountryName = "Japan v2",
        };

        return Ok(dto);
    }
}
```

### Program.cs

```c#
builder.Services.AddApiVersioning(options => 
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
```

### Swagger で Version 対応させるために

Swagger で実行しようとするとブラウザにエラー表示されるため、以下の対応が必要になる。

- ConfigureSwaggerOptions.cs を生成して、設定を書く

```c#
public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider apiVersionDescriptionProvider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        this.apiVersionDescriptionProvider = apiVersionDescriptionProvider;
    }
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var item in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(item.GroupName, CreateVersionInfo(item));
        }
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = "Your Versioned API",
            Version = description.ApiVersion.ToString()
        };

        return info;
    }
}
```

- Program.cs を書き換える

``` c#
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 省略

builder.Services.AddSwaggerGen(options =>
{
    // Swagger で version 指定して実行できるようにする場合は以下をコメントアウトする。(ConfigureSwaggerOptionクラスの設定とぶつかってビルドエラーになる)
    // options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "TM Walks API", Version = "v1" });

    // 省略
});

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

```

## Consuming Our Web API

作成したWebAPIを呼び出すための ASP.NET MVC プロジェクトを作成する

- `dotnet new mvc -o TMWalker.UI`
- `dotnet sln add TMWalker.UI/TMWalker.UI.csproj`

UI Controller を作成し、API呼び出しをする

- TMWalks.API の URL は以下を見る。
  - `TMWalks.API/Properties/launchSettings.json #applicationUrl`
- IHttpClientFactory を使用して、APIにアクセスする
- API の戻り値を json で受け取って DTO へ変換して cshtml で使用する

``` c#
// Program.cs
builder.Services.AddHttpClient();
```

``` c#
public class RegionsController : Controller
{
    private readonly IHttpClientFactory httpClientFactory;

    public RegionsController(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        List<RegionDto> response = new ();

        try
        {
            // Get All Regions from Web API
            var client = httpClientFactory.CreateClient();

            var httpResponseMessage = await client.GetAsync("https://localhost:7017/api/regions");

            // HTTP response が false の時に例外発生する
            httpResponseMessage.EnsureSuccessStatusCode();

            response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
        }
        catch (System.Exception)
        {
            // Log the exceptions
        }

        return View(response);
    }
}
```

``` html
@model IEnumerable<TMWalker.UI.RegionDto>

<h1 class="mt-3">Regions</h1>

<table class="table table-bordered">
    <thead>
        <tr>
            <th>Id</th>
            <th>Code</th>
            <th>Name</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var region in Model)
        {
            <tr>
                <td>@region.Id</td>
                <td>@region.Code</td>
                <td>@region.Name</td>
            </tr>
        }
    </tbody>
</table>
```
