using AutoFixture;
using Vita.Goals.Application.Commands.Tasks.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

using DomainTask = Vita.Goals.Domain.Aggregates.Tasks.Task;
using DomainTaskStatus = Vita.Goals.Domain.Aggregates.Tasks.TaskStatus;

namespace Vita.Goals.UnitTests.Application.Commands.Tasks;

public class CreateTaskCommandHandlerTests
{
    [Theory]
    [AutoMoqData]
    internal async Task GivenCreateTaskCommand_ButNotAllowedUser_WhenHandle_ThenThrowsUnauthorizedAccessException(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Goal goal,
        CreateTaskCommand command,
        CreateTaskCommandHandler sut)
    {
        //Arrange
        goalRepository.Setup(x => x.FindById(command.GoalId, default))
                      .ReturnsAsync(goal);

        //Act
        Func<Task> act = () => sut.Handle(command, default);

        //Assert
        await act.Should().ThrowExactlyAsync<UnauthorizedAccessException>();
    }

    [Theory]
    [AutoMoqData]
    internal async Task GivenCreateTaskCommand_WhenHandle_ThenReturnsGuidOfCreatedTask(
        [Frozen] Mock<Vita.Goals.Domain.Aggregates.Tasks.ITaskRepository> taskRepository,
        [Frozen] Mock<IGoalRepository> goalRepository,
        Fixture fixture,
        CreateTaskCommand command,
        CreateTaskCommandHandler sut)
    {
        //Arrange
        fixture.Inject(command.User.Id);
        Goal goal = fixture.Create<Goal>();

        goalRepository.Setup(x => x.FindById(command.GoalId, default))
                      .ReturnsAsync(goal);

        ICollection<DomainTask> tasksCaptured = new List<DomainTask>();
        taskRepository.Setup(x => x.Add(Capture.In(tasksCaptured)))
                      .Returns(Task.CompletedTask);

        //Act
        Guid createdTaskId = await sut.Handle(command, default);

        //Assert
        tasksCaptured.Should().ContainSingle();
        DomainTask capturedTask = tasksCaptured.First();

        capturedTask.PlannedDate.Should().Be(command.PlannedDate);
        capturedTask.Title.Should().Be(command.Title);
        capturedTask.Status.Should().Be(DomainTaskStatus.Ready);
        capturedTask.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        createdTaskId.Should().Be(capturedTask.Id);
    }

    [Theory]
    [AutoMoqData]
    internal async Task GivenCreateTaskCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
        [Frozen] Mock<Vita.Goals.Domain.Aggregates.Tasks.ITaskRepository> repository,
        [Frozen] Mock<IGoalRepository> goalRepository,
        Fixture fixture,
        CreateTaskCommand command,
        CreateTaskCommandHandler sut)
    {
        //Arrange
        //TODO: Refactor this because it's used in most of tests
        fixture.Inject(command.User.Id);
        Goal goal = fixture.Create<Goal>();

        goalRepository.Setup(x => x.FindById(command.GoalId, default))
                      .ReturnsAsync(goal);

        repository.Setup(x => x.Add(It.IsAny<DomainTask>()))
                  .Returns(Task.CompletedTask)
                  .Verifiable();

        repository.Setup(x => x.UnitOfWork.SaveEntitiesAsync(default))
                  .ReturnsAsync(1)
                  .Verifiable();

        //Act
        _ = await sut.Handle(command, default);

        //Assert
        repository.VerifyAll();
    }
}
