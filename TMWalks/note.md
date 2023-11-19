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
