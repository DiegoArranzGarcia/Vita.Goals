using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Goals.Update;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals;
public class UpdateGoalCommandHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GivenUpdateGoalCommand_ButNotAllowedUser_WhenHandle_ThenThrowsUnauthorizedAccessException(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Goal goal,
        UpdateGoalCommand command,
        UpdateGoalCommandHandler sut)
    {
        //Arrange
        goalRepository.Setup(x => x.FindById(command.Id, default))
                      .ReturnsAsync(goal);

        //Act
        Func<Task> act = () => sut.Handle(command, default);

        //Assert
        await act.Should().ThrowExactlyAsync<UnauthorizedAccessException>();
    }

    [Theory]
    [AutoMoqData]
    public async Task GivenUpdateGoalCommand_WhenHandle_ThenUpdatesTheGoal(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Fixture fixture,
        UpdateGoalCommand command,
        UpdateGoalCommandHandler sut)
    {
        //Arrange
        fixture.Inject(command.User.Id);
        Goal goal = fixture.Create<Goal>();

        goalRepository.Setup(x => x.FindById(command.Id, default))
                      .ReturnsAsync(goal);

        ICollection<Goal> goalsCaptured = new List<Goal>();
        goalRepository.Setup(x => x.Update(Capture.In(goalsCaptured)))
                      .Returns(Task.CompletedTask);

        //Act
        await sut.Handle(command, default);

        //Assert
        goalsCaptured.Should().ContainSingle();
        Goal capturedGoal = goalsCaptured.First();

        capturedGoal.Title.Should().Be(command.Title);
        capturedGoal.AimDate.Should().Be(command.AimDate);
        capturedGoal.Description.Should().Be(command.Description);

        capturedGoal.Id.Should().Be(goal.Id);
        capturedGoal.Status.Should().Be(goal.Status);        
        capturedGoal.CreatedOn.Should().Be(goal.CreatedOn);
        capturedGoal.CreatedBy.Should().Be(goal.CreatedBy);
    }

    [Theory]
    [AutoMoqData]
    public async Task GivenUpdateGoalCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
       [Frozen] Mock<IGoalRepository> goalRepository,
       Fixture fixture,
       UpdateGoalCommand command,
       UpdateGoalCommandHandler sut)
    {
        //Arrange
        fixture.Inject(command.User.Id);
        Goal goal = fixture.Create<Goal>();

        goalRepository.Setup(x => x.FindById(command.Id, default))
                      .ReturnsAsync(goal)
                      .Verifiable();

        goalRepository.Setup(x => x.Update(It.IsAny<Goal>()))
                      .Returns(Task.CompletedTask)
                      .Verifiable();

        goalRepository.Setup(x => x.UnitOfWork.SaveEntitiesAsync(default))
                      .ReturnsAsync(1)
                      .Verifiable();

        //Act
        await sut.Handle(command, default);

        //Assert
        goalRepository.VerifyAll();
    }
}
