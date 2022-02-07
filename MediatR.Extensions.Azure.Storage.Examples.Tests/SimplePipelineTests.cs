using ClassLibrary1;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    public class SimplePipelineTests
    {
        private readonly IServiceProvider serviceProvider;

        public SimplePipelineTests(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCoreDependencies(log)
                .AddSimplePipeline()

                .BuildServiceProvider();
        }

        [Fact(DisplayName = "Contoso pipeline is executed")]
        public async Task Step01()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new ContosoCustomerRequest
            {
                MessageId = Guid.NewGuid().ToString(),
                ContosoCustomer = new ContosoCustomer
                {
                    FirstName = "Fabio",
                    LastName = "Marini",
                    Email = "fm@example.com"
                }
            };

            var res = await med.Send(req);

            res.MessageId.Should().Be(req.MessageId);

            res.CanonicalCustomer.Should().NotBeNull();
            res.CanonicalCustomer.FullName.Should().Be("Fabio Marini");
            res.CanonicalCustomer.Email.Should().Be("fm@example.com");
        }

        [Fact(DisplayName = "Fabrikam pipeline is executed")]
        public async Task Step02()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new FabrikamCustomerRequest
            {
                MessageId = Guid.NewGuid().ToString(),
                CanonicalCustomer = new CanonicalCustomer
                {
                    FullName = "Fabio Marini",
                    Email = "fm@example.com"
                }
            };

            var res = await med.Send(req);

            res.MessageId.Should().Be(req.MessageId);

            res.FabrikamCustomer.Should().NotBeNull();
            res.FabrikamCustomer.FullName.Should().Be("Fabio Marini");
            res.FabrikamCustomer.Email.Should().Be("fm@example.com");
            res.FabrikamCustomer.DateOfBirth.Should().Be(new DateTime(1970, 10, 26));
        }
    }
}
