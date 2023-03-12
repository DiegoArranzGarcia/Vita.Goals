using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalApi.Endpoint;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using Vita.Goals.Application.Commands.Tasks;

namespace Vita.Goals.Api.Endpoints.Tasks.Create;
internal class CreateTaskEndpoint : IEndpoint<IResult, CreateTaskRequest, ClaimsPrincipal, CancellationToken>
{
    private readonly ISender _sender;

    public CreateTaskEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks", ([FromBody] CreateTaskRequest request, ClaimsPrincipal user, CancellationToken cancellationToken) => HandleAsync(request, user, cancellationToken))
           .Produces(StatusCodes.Status201Created)
           .ProducesProblem(StatusCodes.Status401Unauthorized)
           .WithMetadata(new SwaggerOperationAttribute())
           .WithTags("Tasks")
           .RequireAuthorization();
    }

    public async Task<IResult> HandleAsync(CreateTaskRequest request, ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(user.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            return Results.Unauthorized();

        CreateTaskCommand command = new(request.GoalId, request.Title, request.PlannedDateStart, request.PlannedDateEnd);
        Guid createdTaskId = await _sender.Send(command, cancellationToken);

        //Response.Headers.Add("Access-Control-Allow-Headers", "Location");
        //Response.Headers.Add("Access-Control-Expose-Headers", "Location");

        return Results.CreatedAtRoute(routeName: $"api/tasks/{createdTaskId}", routeValues: new { id = createdTaskId }, value: null);
    }
}
