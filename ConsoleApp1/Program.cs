using ClassLibrary1;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    #region 1. Command and Handler

    public class EchoCommand : IRequest
    {
        public string Message { get; set; }
    }

    public class EchoCommandHandler : IRequestHandler<EchoCommand>
    {
        public Task<Unit> Handle(EchoCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine(request.Message);

            return Unit.Task;
        }
    }

    //public class OtherHandler : IRequestHandler<EchoCommand>
    //{
    //    public Task<Unit> Handle(EchoCommand request, CancellationToken cancellationToken)
    //    {
    //        Console.WriteLine(request.Message + " from other handler");

    //        return Unit.Task;
    //    }
    //}

    #endregion

    #region 2. Query and Handler

    public class EchoQuery : IRequest<string>
    {
        public string Message { get; set; }
    }

    public class EchoQueryHandler : IRequestHandler<EchoQuery, string>
    {
        public Task<string> Handle(EchoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.Message);
        }
    }

    #endregion

    #region 3. Pre and Post Processors

    public class EchoPreProcess : IRequestPreProcessor<EchoCommand>
    {
        public Task Process(EchoCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{request.Message} from {nameof(EchoPreProcess)}");

            return Task.CompletedTask;
        }
    }

    public class EchoPostProcess : IRequestPostProcessor<EchoCommand, Unit>
    {
        public Task Process(EchoCommand request, Unit response, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{request.Message} from {nameof(EchoPostProcess)}");

            return Task.CompletedTask;
        }
    }

    #endregion

    #region 4. Notifications

    public class EchoNotification : INotification
    {
        public string Message { get; set; }
    }

    public class EchoNotificationHandler1 : INotificationHandler<EchoNotification>
    {
        public Task Handle(EchoNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{notification.Message} from {nameof(EchoNotificationHandler1)}");

            return Task.CompletedTask;
        }
    }

    public class EchoNotificationHandler2 : INotificationHandler<EchoNotification>
    {
        public Task Handle(EchoNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{notification.Message} from {nameof(EchoNotificationHandler2)}");

            return Task.CompletedTask;
        }
    }

    public class EchoNotificationHandler3 : INotificationHandler<EchoNotification>
    {
        public Task Handle(EchoNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"{notification.Message} from {nameof(EchoNotificationHandler3)}");

            return Task.CompletedTask;
        }
    }

    #endregion

    #region 5. Pipelines

    public class EchoBehavior1 : IPipelineBehavior<EchoCommand, Unit>
    {
        public Task<Unit> Handle(EchoCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            Console.WriteLine($"{request.Message} from {nameof(EchoBehavior1)}");

            return next();
        }
    }

    public class EchoBehavior2 : IPipelineBehavior<EchoCommand, Unit>
    {
        public Task<Unit> Handle(EchoCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            Console.WriteLine($"{request.Message} from {nameof(EchoBehavior2)}");

            return next();
        }
    }

    public class EchoBehavior3 : IPipelineBehavior<EchoCommand, Unit>
    {
        public Task<Unit> Handle(EchoCommand request, CancellationToken cancellationToken, RequestHandlerDelegate<Unit> next)
        {
            Console.WriteLine($"{request.Message} from {nameof(EchoBehavior3)}");

            return next();
        }
    }

    #endregion

    class Program
    {
        private static async Task RunDemo1()
        {
            var serviceProvider = new ServiceCollection()

                .AddMediatR(typeof(Program))

            #region Behaviors

                .AddTransient(typeof(IPipelineBehavior<EchoCommand, Unit>), typeof(EchoBehavior3))
                .AddTransient(typeof(IPipelineBehavior<EchoCommand, Unit>), typeof(EchoBehavior2))
                .AddTransient(typeof(IPipelineBehavior<EchoCommand, Unit>), typeof(EchoBehavior1))

            #endregion

                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            // 1. command and handler
            // 3. pre and post process
            // 5. pipeline behaviors
            var cmd = new EchoCommand { Message = "Hello command! :)" };

            _ = await mediator.Send(cmd);

            // 2. query and handler
            var qry = new EchoQuery { Message = "Hello query! :)" };

            var res = await mediator.Send<string>(qry);

            Console.WriteLine(res);

            // 4. notifications
            var notification = new EchoNotification { Message = "Hello notification! :)" };

            await mediator.Publish(notification);
        }

        private static async Task RunDemo2(IMediator mediator)
        {
            var activityId = Guid.NewGuid().ToString();

            var cmd1 = new SourceCustomerCommand
            {
                MessageId = activityId,
                SourceCustomer = new SourceCustomer
                {
                    FirstName = "Fabio",
                    LastName = "Marini",
                    Email = "fm@example.com"
                }
            };

            _ = await mediator.Send(cmd1);

            // pretend this is coming from the customers queue...
            var cmd2 = new TargetCustomerCommand
            {
                MessageId = activityId,
                CanonicalCustomer = new CanonicalCustomer
                {
                    FullName = "Fabio Marini",
                    Email = "fm@example.com"
                }
            };

            _ = await mediator.Send(cmd2);
        }

        private static async Task RunDemo3(IMediator mediator)
        {
            var activityId = "c6fcc080-d812-4822-97ce-1bfb5e883158";

            var qry = new RetrieveCustomerQuery { MessageId = activityId };

            var res = await mediator.Send(qry);

            Console.WriteLine(JsonConvert.SerializeObject(res, Formatting.Indented));
        }

        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()

                .AddSingleton<IConfiguration>(sp =>
                {
                    var appSettings = new Dictionary<string, string>
                    {
                        { "TrackingEnabled", "true" }
                    };

                    return new ConfigurationBuilder()

                        .AddInMemoryCollection(appSettings)
                        .Build();
                })
                .AddCore()

                //.AddSimplePipeline()

                //.AddTableTrackingPipeline()
                //.AddBlobTrackingPipeline()
                //.AddQueueRoutingPipeline()

                //.AddActivityTrackingPipeline()
                //.AddMultiTrackingPipeline()

                // run query
                //.AddBlobTrackingProcessors()

                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            await RunDemo2(mediator);

            //await RunDemo3(mediator);

            Console.Read();
        }
    }
}
