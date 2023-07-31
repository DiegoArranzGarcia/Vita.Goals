using AutoFixture.AutoMoq;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vita.Goals.Api.Endpoints.Goals.Complete;
using Vita.Goals.Api.Endpoints.Goals.Create;
using Vita.Goals.Application.Commands.Goals.Complete;
using Vita.Goals.Application.Commands.Shared;
using Vita.Goals.UnitTests.Attributes;
using Vita.Goals.UnitTests.AutoFixture;

namespace Vita.Goals.UnitTests.Api.Endpoints.Goals;
public class CreateGoalEndpointTests
{
    [Theory]
    [AutoMoqData]
    internal async Task GivenGoalId_WhenCompletingGoal_ThenReturnsNoContent(
        [Frozen] Mock<AutoMapper.IMapper> mapper,
        CreateGoalRequest request)
    {
        var linkGenerator = Mock.Of<LinkGenerator>();

        CreateGoalEndpoint sut = Factory.Create<CreateGoalEndpoint>(
            //ctx => ctx.Response.HttpContext.RequestServices.Add.GetService<LinkGenerator>()
            Mock.Of<ISender>(),
            mapper.Object);

        mapper.Setup(x => x.Map<User>(sut.User))
              .ReturnsUsingFixture(TestsFixture.CreateFixture());

        await sut.HandleAsync(request, ct: default);

        sut.HttpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Created);
        //TODO: More chekcs of CreatedAt
    }

    [Theory]
    [AutoMoqData]
    internal async Task GivenCreateGoalRequest_WhenCreatingGoal_ThenReturnsCreatedAtRoute(
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<AutoMapper.IMapper> mapper,
        User user,
        CreateGoalRequest request)
    {
        CreateGoalEndpoint sut = Factory.Create<CreateGoalEndpoint>(
            sender.Object,
            mapper.Object);

        List<CompleteGoalCommand> commandsCaptured = new();
        sender.Setup(x => x.Send(Capture.In(commandsCaptured), default))
              .Returns(Task.CompletedTask);

        mapper.Setup(x => x.Map<User>(sut.HttpContext.User))
                            .Returns(user);

        await sut.HandleAsync(request, default);

        //TODO: More chekcs of CreatedCommand
        commandsCaptured.Should().ContainSingle()
                        .Which.Should()
                        .Subject.Should().BeEquivalentTo(new { User = user });
    }
}
