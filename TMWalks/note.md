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
