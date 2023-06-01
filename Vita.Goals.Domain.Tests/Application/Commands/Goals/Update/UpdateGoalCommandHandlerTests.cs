using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vita.Goals.Application.Commands.Goals;
using Vita.Goals.UnitTests.Attributes;

namespace Vita.Goals.UnitTests.Application.Commands.Goals.Update;
public class UpdateGoalCommandHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task GivenUpdateGoalCommand_WhenHandle_Then(
        UpdateGoalCommand command,
        UpdateGoalCommandHandler sut)
    {
        await sut.Handle(command, default);
    }
}
