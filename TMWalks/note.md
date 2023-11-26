# メモ

## VS Code の環境構築

[チュートリアル](https://learn.microsoft.com/ja-jp/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio-code)

### Solution を作る

`dotnet new sln`

### Solution にプロジェクトを追加する

`dotnet sln add <プロジェクトパス>`

## VS Code でAPIをテスト実行する

- `dotnet run --launch-profile https`
- 上記を実行後に、<https://localhost:portnumber/swagger>

## Swagger

Swagger is a popular tool for docmenting APIs and prividing a user friendly interface for testing and exploring the APIs.

## パッケージの追加

`dotnet add package [パッケージ名]`
e.g. dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 7.0.0

## Microsoft.EntityFrameworkCore.Tools

This is the package that is responsible to run migrations, which will later on create a database for us.

## DbContext Class

- Maintaining Connection To Db
- Track Changes
- Perform CRUD operations
- Brige between domain models and the database

A Dbcontext class is a class that represents a session with a database and provides a set of APIs for performing database operations.  
It also provides a way to define the database schema using entity classes or domain classes that we just built, which maps directly to database tables.  
So we can say that the Dbcontext class is a bridge between your domain model classes and the database.  

Controller <--> DbContext <--> Database

## Mac で Sqlserver は Docker にインストールする必要がある

- イメージのダウンロード
  - `docker pull mcr.microsoft.com/azure-sql-edge`
- コンテナの起動
  - `docker run --cap-add SYS_PTRACE -e 'ACCEPT_EULA=1' -e 'MSSQL_SA_PASSWORD=Password.1' -p 1433:1433 --name azuresqledge -d mcr.microsoft.com/azure-sql-edge`

## SqlServer の Docker-Compose を作成して、git にあげた

[github](https://github.com/hogendan/Docker-SqlServer)

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

