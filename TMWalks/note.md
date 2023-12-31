# メモ

---

## VS Code の環境構築

[チュートリアル](https://learn.microsoft.com/ja-jp/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio-code)

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
2. Update-Database

上記は VS Code ではできなかったので、以下のコマンドで実行する
[参考](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
[参考(続き)](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

1. dotnet tool install --global  dotnet-ef --version 7.0
2. dotnet tool update --global dotnet-ef --version 7.0
3. cd TMWalks/TMWalk.API
   1. プロジェクトファイルがあるフォルダに移動する
4. dotnet ef migrations add "Initial Migration"
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

- Userd to transfer data between different layers
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

## Soring

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
