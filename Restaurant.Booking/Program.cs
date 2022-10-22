using Microsoft.EntityFrameworkCore;
using MassTransit;
using Restaurant.Booking;
using Restaurant.Booking.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

Console.OutputEncoding = System.Text.Encoding.UTF8; 
Console.Title = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>().ConsoleTitle;

builder.Services.AddDbContext<RestaurantBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(ConnectionStringsKeys.SqlServer)));

builder.Services.AddMassTransit((x =>
{
    x.AddConsumer<BookingRequestedConsumer>().Endpoint(config => config.Temporary = true);
    x.AddConsumer<BookingApprovedConsumer>().Endpoint(config => config.Temporary = true);
    x.AddConsumer<BookingFaultedConsumer>().Endpoint(config => config.Temporary = true);

    x.AddSagaStateMachine<BookingStateMachine, BookingState>()
            .Endpoint(e => e.Temporary = true)
            .InMemoryRepository();

    x.AddDelayedMessageScheduler();

    x.UsingRabbitMq((context, config) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

        config.Host(
            host: rabbitMqConfig.Host,
            virtualHost: rabbitMqConfig.VirtualHost,
            hostSettings =>
            {
                hostSettings.Username(rabbitMqConfig.User);
                hostSettings.Password(rabbitMqConfig.Password);
            });

        config.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(10),
                                                       TimeSpan.FromMinutes(20),
                                                       TimeSpan.FromMinutes(30)));

        config.UseMessageRetry(r => r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3)));

        config.UseDelayedMessageScheduler();
        config.UseInMemoryOutbox();
        config.ConfigureEndpoints(context);
    });
}));

builder.Services.AddSingleton<Restaurant.Booking.Restaurant>();
builder.Services.AddTransient<BookingState>();
builder.Services.AddTransient<BookingStateMachine>();
builder.Services.AddScoped<IProcessedMessagesRepository, ProcessedMessagesRepository>();
builder.Services.AddHostedService<WorkerBackgroundService>();

var app = builder.Build();
app.Run();