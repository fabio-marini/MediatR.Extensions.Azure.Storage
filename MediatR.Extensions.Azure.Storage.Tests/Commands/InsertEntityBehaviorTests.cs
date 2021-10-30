using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Commands
{
    public class InsertEntityBehaviorTests
    {
        private readonly InsertEntityBehaviorFixture<TestCommand> ctx;
        private readonly TestCommand cmd = new TestCommand { Message = "Hello!" };

        public InsertEntityBehaviorTests()
        {
            this.ctx = new InsertEntityBehaviorFixture<TestCommand>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1() => _ = await ctx.Test1(cmd);

        [Fact(DisplayName = "CloudTable is not specified")]
        public async Task Test2() => _ = await ctx.Test2(cmd);

        [Fact(DisplayName = "Behavior uses default TableEntity")]
        public async Task Test3() => _ = await ctx.Test3(cmd);

        [Fact(DisplayName = "Behavior uses specified TableEntity")]
        public async Task Test4() => _ = await ctx.Test4(cmd);

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test5() => _ = await ctx.Test5(cmd);
    }
}
