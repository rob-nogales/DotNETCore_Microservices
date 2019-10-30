using Convey;
using Convey.CQRS.Commands;
using Convey.CQRS.Events;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.LoadBalancing.Fabio;
using Convey.Logging;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Metrics.AppMetrics;
using Convey.Persistence.Redis;
using Convey.Tracing.Jaeger;
using Convey.Tracing.Jaeger.RabbitMQ;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using MessagesService.Commands;
using MessagesService.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using static MessagesService.Events.UserCreated;

namespace MessagesService
{
    public class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services => services
                    .AddOpenTracing()
                    .AddConvey()
                    .AddConsul()
                    .AddFabio()
                    .AddJaeger()
                    .AddCommandHandlers()
                    .AddEventHandlers()
                    .AddQueryHandlers()
                    .AddInMemoryCommandDispatcher()
                    .AddInMemoryEventDispatcher()
                    .AddInMemoryQueryDispatcher()
                    .AddRedis()
                    .AddRabbitMq(plugins: p => p.AddJaegerRabbitMqPlugin())
                    .AddMetrics()
                    .AddWebApi()
                    .Build())
                .Configure(app => app
                    .UseErrorHandler()
                    .UseInitializers()
                    .UseRouting()
                    .UseDispatcherEndpoints(endpoints => endpoints
                        .Get("ping", ctx => ctx.Response.WriteAsync("OK"))
                        .Get<GetUserMessages.Query, GetUserMessages.Result>("users/{userId}/messages")
                        .Post<CreateUserMessage.Command>("/users/{userId}/messages", afterDispatch: (cmd, ctx) => ctx.Response.Created($"messages/{cmd.MessageId}")))
                    .UseJaeger()
                    .UseInitializers()
                    .UseMetrics()
                    .UseRabbitMq()
                    .SubscribeEvent<UserCreatedEvent>())
                .UseLogging();
            });
    }
}
