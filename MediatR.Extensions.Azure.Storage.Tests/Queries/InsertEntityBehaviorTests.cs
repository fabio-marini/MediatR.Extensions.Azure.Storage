using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Queries
{
    public class InsertEntityBehaviorTests
    {
        private readonly InsertEntityBehaviorFixture<TestQuery, TestResult> ctx;
        private readonly TestQuery qry = new TestQuery { Message = "Hello!" };

        public InsertEntityBehaviorTests()
        {
            this.ctx = new InsertEntityBehaviorFixture<TestQuery, TestResult>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1() => _ = await ctx.Test1(qry);

        [Fact(DisplayName = "CloudTable is not specified")]
        public async Task Test2() => _ = await ctx.Test2(qry);

        [Fact(DisplayName = "Behavior uses default TableEntity")]
        public async Task Test3() => _ = await ctx.Test3(qry);

        [Fact(DisplayName = "Behavior uses specified TableEntity")]
        public async Task Test4() => _ = await ctx.Test4(qry);

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test5() => _ = await ctx.Test5(qry);
    }
}
