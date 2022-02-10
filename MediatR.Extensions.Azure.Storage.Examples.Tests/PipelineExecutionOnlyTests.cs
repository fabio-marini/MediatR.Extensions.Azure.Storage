﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace MediatR.Extensions.Azure.Storage.Examples
{
    [Trait("TestCategory", "Integration"), Collection("Examples")]
    public class PipelineExecutionOnlyTests
    {
        private readonly IServiceProvider serviceProvider;

        public PipelineExecutionOnlyTests(ITestOutputHelper log)
        {
            serviceProvider = new ServiceCollection()

                .AddCoreDependencies(log)

                .AddContosoRequestPipeline()
                .AddContosoExceptionPipeline()

                .AddFabrikamRequestPipeline()
                .AddFabrikamExceptionPipeline()

                .BuildServiceProvider();
        }

        [Fact(DisplayName = "All pipelines have no errors")]
        public async Task Test00()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req1 = new ContosoCustomerRequest
            {
                MessageId = Guid.NewGuid().ToString(),
                ContosoCustomer = new ContosoCustomer
                {
                    FirstName = "Fabio",
                    LastName = "Marini",
                    Email = "fm@example.com"
                }
            };

            var res1 = await med.Send(req1);

            res1.MessageId.Should().Be(req1.MessageId);

            res1.CanonicalCustomer.Should().NotBeNull();
            res1.CanonicalCustomer.FullName.Should().Be("Fabio Marini");
            res1.CanonicalCustomer.Email.Should().Be("fm@example.com");

            var req2 = new FabrikamCustomerRequest
            {
                MessageId = req1.MessageId,
                CanonicalCustomer = res1.CanonicalCustomer
            };

            var res2 = await med.Send(req2);

            res2.MessageId.Should().Be(req2.MessageId);

            res2.FabrikamCustomer.Should().NotBeNull();
            res2.FabrikamCustomer.FullName.Should().Be("Fabio Marini");
            res2.FabrikamCustomer.Email.Should().Be("fm@example.com");
            res2.FabrikamCustomer.DateOfBirth.Should().Be(new DateTime(1970, 10, 26));
        }

        [Fact(DisplayName = "Contoso pipeline has errors")]
        public async Task Test01()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new ContosoCustomerRequest
            {
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

        [Fact(DisplayName = "Fabrikam pipeline has errors")]
        public async Task Test02()
        {
            var med = serviceProvider.GetRequiredService<IMediator>();

            var req = new FabrikamCustomerRequest
            {
                CanonicalCustomer = new CanonicalCustomer
                {
                    FullName = "Fabio Marini",
                    Email = "fm@example.com"
                }
            };

            try
            {
                var res = await med.Send(req);
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
