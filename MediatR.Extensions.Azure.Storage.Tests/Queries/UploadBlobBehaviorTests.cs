using System.Threading.Tasks;
using Xunit;

namespace MediatR.Extensions.Azure.Storage.Tests.Queries
{
    public class UploadBlobBehaviorTests
    {
        private readonly UploadBlobBehaviorFixture<TestQuery, TestResult> ctx;
        private readonly TestQuery qry = new TestQuery { Message = "Hello!" };

        public UploadBlobBehaviorTests()
        {
            this.ctx = new UploadBlobBehaviorFixture<TestQuery, TestResult>();
        }

        [Fact(DisplayName = "Behavior is disabled")]
        public async Task Test1() => _ = await ctx.Test1(qry);

        [Fact(DisplayName = "BlobClient is not specified")]
        public async Task Test2() => _ = await ctx.Test2(qry);

        [Fact(DisplayName = "Behavior uses default BlobContent and BlobHeaders")]
        public async Task Test3() => _ = await ctx.Test3(qry);

        [Fact(DisplayName = "Behavior uses specified BlobContent")]
        public async Task Test4() => _ = await ctx.Test4(qry);

        [Fact(DisplayName = "Behavior uses specified BlobHeaders")]
        public async Task Test5() => _ = await ctx.Test5(qry);

        [Fact(DisplayName = "Behavior uses specified BlobContent and BlobHeaders")]
        public async Task Test6() => _ = await ctx.Test6(qry);

        [Fact(DisplayName = "Behavior uses specified Metadata")]
        public async Task Test7() => _ = await ctx.Test7(qry);

        [Fact(DisplayName = "Exceptions are logged")]
        public async Task Test8() => _ = await ctx.Test8(qry);
    }
}
