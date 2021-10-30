using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands
{
    public class SendMessageBehaviorTests
    {
        private readonly SendMessageBehaviorFixture<TestCommand> ctx;
        private readonly TestCommand cmd = new TestCommand { Message = "Hello!" };

        public SendMessageBehaviorTests()
        {
            this.ctx = new SendMessageBehaviorFixture<TestCommand>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1() => _ = await ctx.Test1(cmd);

        [Fact(DisplayName = "QueueClient is not specified")]
        public async Task Test2() => _ = await ctx.Test2(cmd);

        [Fact(DisplayName = "Behavior uses default QueueMessage")]
        public async Task Test3() => _ = await ctx.Test3(cmd);

        [Fact(DisplayName = "Behavior uses specified QueueMessage")]
        public async Task Test4() => _ = await ctx.Test4(cmd);

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test5() => _ = await ctx.Test5(cmd);
    }
}
