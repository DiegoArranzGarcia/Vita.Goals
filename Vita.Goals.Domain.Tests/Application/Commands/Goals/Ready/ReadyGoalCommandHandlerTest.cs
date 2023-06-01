using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals.Ready;
public class ReadyGoalCommandHandlerTest
{
    [Theory]
    [AutoMoqData]
    public async Task GivenReadyGoalCommand_ButNotAllowedUser_WhenHandle_ThenThrowsUnauthorizedAccessException(
        [Frozen] Mock<IGoalsRepository> goalRepository,
        Goal goal,
        ReadyGoalCommand command,
        ReadyGoalCommandHandler sut)
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
    public async Task GivenReadyGoalCommand_WhenHandle_ThenReadysTheGoal(
        [Frozen] Mock<IGoalsRepository> goalRepository,
        Fixture fixture,
        ReadyGoalCommand command,
        ReadyGoalCommandHandler sut)
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
        capturedGoal.GoalStatus.Should().Be(GoalStatus.Ready);
    }

    [Theory]
    [AutoMoqData]
    public async Task GivenReadyGoalCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
       [Frozen] Mock<IGoalsRepository> goalRepository,
       Fixture fixture,
       ReadyGoalCommand command,
       ReadyGoalCommandHandler sut)
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
