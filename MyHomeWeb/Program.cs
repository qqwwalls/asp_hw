var app = WebApplication.Create(args);

app.MapGet("/", () => "Welcome to MyHomeWeb!");
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");
app.MapGet("/api/status", () => new { message = "MyHomeWeb is running", time = DateTime.Now });

app.Run();
