using Vita.Goals.Application.Commands.Tasks.Create;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

using DomainTask = Vita.Goals.Domain.Aggregates.Tasks.Task;
using DomainTaskStatus = Vita.Goals.Domain.Aggregates.Tasks.TaskStatus;

namespace Vita.Goals.UnitTests.Application.Commands.Tasks.Create;

public class CreateTaskCommandHandlerTests
{

    [Theory]
    [AutoMoqData]
    internal async Task GivenCreateTaskCommand_WhenHandle_ThenReturnsGuidOfCreatedTask(
        [Frozen] Mock<Vita.Goals.Domain.Aggregates.Tasks.ITaskRepository> goalRepository,
        [Frozen] Mock<IGoalRepository> taskRepository,
        Goal goal,
        CreateTaskCommand command,
        CreateTaskCommandHandler sut)
    {
        //Arrange
        ICollection<DomainTask> tasksCaptured = new List<DomainTask>();
        taskRepository.Setup(x => x.Add(Capture.In(tasksCaptured)))
                  .Returns(Task.CompletedTask);

        //Act
        Guid createdTaskId = await sut.Handle(command, default);

        //Assert
        tasksCaptured.Should().ContainSingle();
        DomainTask capturedTask = tasksCaptured.First();

        capturedTask.PlannedDate.Should().Be(command);
        capturedTask.Title.Should().Be(command.Title);
        capturedTask.Status.Should().Be(DomainTaskStatus.Ready);
        capturedTask.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        createdTaskId.Should().Be(capturedTask.Id);
    }

    [Theory]
    [AutoMoqData]
    internal async Task GivenCreateTaskCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
        [Frozen] Mock<Vita.Goals.Domain.Aggregates.Tasks.ITaskRepository> repository,
        CreateTaskCommand command,
        CreateTaskCommandHandler sut)
    {
        //Arrange
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
