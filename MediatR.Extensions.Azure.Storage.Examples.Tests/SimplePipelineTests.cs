using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    public class SimplePipelineTests
    {
        private readonly IServiceProvider serviceProvider;

        public SimplePipelineTests(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddOptions<TestOutputLoggerOptions>().Configure(opt => opt.MinimumLogLevel = LogLevel.Warning)
                .Services

                .AddCoreDependencies(log)
                .AddSimplePipeline()
                .AddContosoErrorPipeline()
                .AddFabrikamErrorPipeline()

                .BuildServiceProvider();
        }

        [Theory(DisplayName = "Contoso pipeline is executed")]
        [InlineData("cbefba6e-e51e-4b3a-9d4f-4eae5824d06a")]
        [InlineData(null)]
        public async Task Test01(string messageId)
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new ContosoCustomerRequest
            {
                MessageId = messageId,
                ContosoCustomer = new ContosoCustomer
                {
                    FirstName = "Fabio",
                    LastName = "Marini",
                    Email = "fm@example.com"
                }
            };

            try
            {
                var res = await med.Send(req);

                res.MessageId.Should().Be(req.MessageId);

                res.CanonicalCustomer.Should().NotBeNull();
                res.CanonicalCustomer.FullName.Should().Be("Fabio Marini");
                res.CanonicalCustomer.Email.Should().Be("fm@example.com");
            }
            catch (Exception ex)
            {
                var err = new ContosoExceptionRequest
                {
                    Exception = ex,
                    Request = req
                };

                _ = await med.Send(err);
            }
        }

        [Theory(DisplayName = "Fabrikam pipeline is executed")]
        [InlineData("cbefba6e-e51e-4b3a-9d4f-4eae5824d06a")]
        [InlineData(null)]
        public async Task Test02(string messageId)
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new FabrikamCustomerRequest
            {
                MessageId = messageId,
                CanonicalCustomer = new CanonicalCustomer
                {
                    FullName = "Fabio Marini",
                    Email = "fm@example.com"
                }
            };

            try
            {
                var res = await med.Send(req);

                res.MessageId.Should().Be(req.MessageId);

                res.FabrikamCustomer.Should().NotBeNull();
                res.FabrikamCustomer.FullName.Should().Be("Fabio Marini");
                res.FabrikamCustomer.Email.Should().Be("fm@example.com");
                res.FabrikamCustomer.DateOfBirth.Should().Be(new DateTime(1970, 10, 26));
            }
            catch (Exception ex)
            {
                var err = new FabrikamExceptionRequest
                {
                    Exception = ex,
                    Request = req
                };

                _ = await med.Send(err);
            }
        }
    }
}
