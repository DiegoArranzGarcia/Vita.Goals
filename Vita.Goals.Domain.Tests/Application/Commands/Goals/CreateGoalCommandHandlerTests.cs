using Vita.Goals.Application.Commands.Goals.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals;

public class CreateGoalCommandHandlerTests
{

    [Theory]
    [AutoMoqData]
    public async Task GivenCreateGoalCommand_WhenHandle_ThenReturnsGuidOfCreatedGoal(
        [Frozen] Mock<IGoalRepository> repository,
        CreateGoalCommand command,
        CreateGoalCommandHandler sut)
    {
        //Arrange
        ICollection<Goal> goalsCaptured = new List<Goal>();
        repository.Setup(x => x.Add(Capture.In(goalsCaptured)))
                  .Returns(Task.CompletedTask);

        //Act
        Guid createdGoalId = await sut.Handle(command, default);

        //Assert
        goalsCaptured.Should().ContainSingle();
        Goal capturedGoal = goalsCaptured.First();

        capturedGoal.AimDate.Should().Be(command.AimDate);
        capturedGoal.CreatedBy.Should().Be(command.CreatedBy);
        capturedGoal.Title.Should().Be(command.Title);
        capturedGoal.Description.Should().Be(command.Description);
        capturedGoal.Status.Should().Be(GoalStatus.ToBeDefined);
        capturedGoal.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        createdGoalId.Should().Be(capturedGoal.Id);
    }

    [Theory]
    [AutoMoqData]
    public async Task GivenCreateGoalCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
        [Frozen] Mock<IGoalRepository> repository,
        CreateGoalCommand command,
        CreateGoalCommandHandler sut)
    {
        //Arrange
        repository.Setup(x => x.Add(It.IsAny<Goal>()))
                  .Returns(Task.CompletedTask)
                  .Verifiable();

        repository.Setup(x => x.UnitOfWork.SaveEntitiesAsync(default))
                  .ReturnsAsync(1)
                  .Verifiable();

        //Act
        _ = await sut.Handle(command, default);

        //Assert
        repository.Verify();
    }
}
