using Vita.Goals.Application.Commands.Goals.Delete;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals.Delete;
public class DeleteCommandHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GivenDeleteGoalCommand_WhenHandle_ThenCallsTheAppropiateRepositoryMethods(
        [Frozen] Mock<IGoalRepository> goalsRepository,
        DeleteGoalCommand command,
        DeleteGoalCommandHandler sut)
    {
        //Arrange
        goalsRepository.Setup(x => x.Delete(command.Id))
                       .Returns(Task.CompletedTask)
                       .Verifiable();

        goalsRepository.Setup(x => x.UnitOfWork.SaveEntitiesAsync(default))
                       .ReturnsAsync(1)
                       .Verifiable();

        //Act
        await sut.Handle(command, default);

        //Assert
        goalsRepository.VerifyAll();
    }
}
