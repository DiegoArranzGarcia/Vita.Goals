using AutoFixture;
using Vita.Goals.Application.Commands.Goals.Delete;
using Vita.Goals.Application.Commands.Goals.Delete;
using Vita.Goals.Domain.Aggregates.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals;

public class DeleteCommandHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GivenDeleteGoalCommand_ButNotAllowedUser_WhenHandle_ThenThrowsUnauthorizedAccessException(
        [Frozen] Mock<IGoalRepository> goalRepository,
        Goal goal,
        DeleteGoalCommand command,
        DeleteGoalCommandHandler sut)
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
    public async Task GivenDeleteGoalCommand_WhenHandle_ThenMakesAppropiateRepositoryCalls(
       [Frozen] Mock<IGoalRepository> goalRepository,
       Fixture fixture,
       DeleteGoalCommand command,
       DeleteGoalCommandHandler sut)
    {
        //Arrange
        fixture.Inject(command.User.Id);
        Goal goal = fixture.Create<Goal>();

        goalRepository.Setup(x => x.FindById(command.Id, default))
                      .ReturnsAsync(goal)
                      .Verifiable();

        goalRepository.Setup(x => x.Delete(command.Id))
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
