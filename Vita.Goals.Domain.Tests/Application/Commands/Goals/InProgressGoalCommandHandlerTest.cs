using Vita.Goals.Application.Commands.Goals.InProgress;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals;
public class InProgressGoalCommandHandlerTest
{
    [Theory]
    [AutoMoqData]
    public async Task GivenInProgressGoalCommand_ButNotAllowedUser_WhenHandle_ThenThrowsUnauthorizedAccessException(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Goal goal,
        InProgressGoalCommand command,
        InProgressGoalCommandHandler sut)
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
    public async Task GivenInProgressGoalCommand_WhenHandle_ThenChangesToInProgressTheGoal(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Fixture fixture,
        InProgressGoalCommand command,
        InProgressGoalCommandHandler sut)
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

        capturedGoal.Id.Should().Be(goal.Id);
        capturedGoal.Status.Should().Be(GoalStatus.InProgress);
    }

    [Theory]
    [AutoMoqData]
    public async Task GivenInProgressGoalCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
       [Frozen] Mock<IGoalRepository> goalRepository,
       Fixture fixture,
       InProgressGoalCommand command,
       InProgressGoalCommandHandler sut)
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
        goalRepository.Verify();
    }
}
