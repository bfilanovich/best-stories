# Best Stories API.

## Run

To build and run the application you need [.NET SDK 8.0](https://download.visualstudio.microsoft.com/download/pr/6f043b39-b3d2-4f0a-92bd-99408739c98d/fa16213ea5d6464fa9138142ea1a3446/dotnet-sdk-8.0.407-win-x64.exe).

Use this command to run the application from the solution directory:
```
dotnet run --project .\BestStories.Api\BestStories.Api.csproj --launch-profile http
```

Run using Docker
```
docker build -t best-stories-api:local -f .\BestStories.Api\Dockerfile . && docker run --rm -p 8080:8080 best-stories-api:local
```

After application has started, it can be tested using an endpoint `http://localhost:8080/v1/stories/top` with an optional query parameter `count`. 

Curl: 
```
curl http://localhost:8080/v1/stories/top?count=15
```

Swagger URL: `http://localhost:8080/swagger/index.html`. 
