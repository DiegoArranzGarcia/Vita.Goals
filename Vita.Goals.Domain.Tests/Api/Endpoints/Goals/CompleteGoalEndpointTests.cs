using AutoFixture.AutoMoq;
using FastEndpoints;
using MediatR;
using System.Net;
using Vita.Goals.Api.Endpoints.Goals.Complete;
using Vita.Goals.Application.Commands.Goals.Complete;
using Vita.Goals.Application.Commands.Shared;
using Vita.Goals.UnitTests.Attributes;
using Vita.Goals.UnitTests.AutoFixture;

namespace Vita.Goals.UnitTests.Api.Endpoints.Goals;
public class CompleteGoalEndpointTests
{
    [Theory]
    [AutoMoqData]
    internal async Task GivenGoalId_WhenCompletingGoal_ThenReturnsNoContent(
        Guid goalId,
        EmptyRequest request)
    {
        CompleteGoalEndpoint sut = Factory.Create<CompleteGoalEndpoint>(
            ctx => ctx.Request.RouteValues.Add("id", goalId),
            Mock.Of<ISender>(),
            Mock.Of<AutoMapper.IMapper>());

        await sut.HandleAsync(request, default);

        sut.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
    }

    [Theory]
    [AutoMoqData]
    internal async Task GivenGoalId_WhenCompletingGoal_ThenReturnsSendsCompleteGoalCommand(
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<AutoMapper.IMapper> mapper,
        User user,
        Guid goalId,
        EmptyRequest request)
    {
        CompleteGoalEndpoint sut = Factory.Create<CompleteGoalEndpoint>(
            ctx => ctx.Request.RouteValues.Add("id", goalId),
            sender.Object,
            mapper.Object);

        List<CompleteGoalCommand> commandsCaptured = new();
        sender.Setup(x => x.Send(Capture.In(commandsCaptured), default))
              .Returns(Task.CompletedTask);

        mapper.Setup(x => x.Map<User>(sut.HttpContext.User))
                           .Returns(user);

        await sut.HandleAsync(request, default);

        commandsCaptured.Should().ContainSingle()
                        .Which.Should()
                        .Subject.Should().BeEquivalentTo(new { Id = goalId, User = user });
    }
}
