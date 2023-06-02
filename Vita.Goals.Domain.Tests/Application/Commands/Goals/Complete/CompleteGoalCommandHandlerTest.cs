using Vita.Goals.Application.Commands.Goals.Complete;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals.Complete;
public class CompleteGoalCommandHandlerTest
{
    [Theory]
    [AutoMoqData]
    public async Task GivenCompleteGoalCommand_ButNotAllowedUser_WhenHandle_ThenThrowsUnauthorizedAccessException(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Goal goal,
        CompleteGoalCommand command,
        CompleteGoalCommandHandler sut)
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
    public async Task GivenCompleteGoalCommand_WhenHandle_ThenCompletesTheGoal(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Fixture fixture,
        CompleteGoalCommand command,
        CompleteGoalCommandHandler sut)
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
        capturedGoal.Status.Should().Be(GoalStatus.Completed);
    }

    [Theory]
    [AutoMoqData]
    public async Task GivenCompleteGoalCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
       [Frozen] Mock<IGoalRepository> goalRepository,
       Fixture fixture,
       CompleteGoalCommand command,
       CompleteGoalCommandHandler sut)
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
