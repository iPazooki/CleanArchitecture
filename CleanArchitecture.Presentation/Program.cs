WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices()
    .AddInfrastructurePersistenceServices(builder.Configuration)
    .AddPresentationServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// API Endpoints
app.MapPost("/create-book", async (ISender sender, CreateBookCommand command) =>
    {
        Result<int> result = await sender.Send(command);

        return Results.Created($"/create-book/{result.Value}", result.Value);
    })
    .WithOpenApi()
    .WithSummary("Creates a new book")
    .WithDescription("Creates a new book with the specified title and genre.")
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.Created, "The ID of the created book", typeof(int)))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

app.MapPut("/update-book", async (ISender sender, UpdateBookCommand command) =>
    {
        Result result = await sender.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    })
    .WithOpenApi()
    .WithSummary("Updates an existing book")
    .WithDescription("Updates an existing book with the specified ID.")
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NoContent, "The book was successfully updated"))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

app.MapDelete("/delete-book", async (ISender sender, [FromBody] DeleteBookCommand command) =>
    {
        Result result = await sender.Send(command);

        return result.IsSuccess
            ? Results.NoContent()
            : Results.BadRequest(string.Join(',', result.Errors.Select(x => x.Message)));
    })
    .WithOpenApi()
    .WithSummary("Deletes an existing book")
    .WithDescription("Deletes an existing book with the specified ID.")
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NoContent, "The book was successfully deleted"))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.BadRequest, "Invalid input parameters"))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

app.MapGet("/get-book/{id:int}", async (ISender sender, int id) =>
    {
        Result<BookDto> result = await sender.Send(new GetBookQuery(id));

        return result.IsSuccess
            ? Results.Ok(result)
            : Results.NotFound();
    })
    .WithOpenApi()
    .WithSummary("Gets a book by ID")
    .WithDescription("Gets a book with the specified ID.")
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.OK, "The book was found", typeof(BookDto)))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.NotFound, "The book was not found"))
    .WithMetadata(new SwaggerResponseAttribute((int)HttpStatusCode.InternalServerError, "An error occurred while processing the request", typeof(ProblemDetails)));

app.UseExceptionHandler();

app.Run();