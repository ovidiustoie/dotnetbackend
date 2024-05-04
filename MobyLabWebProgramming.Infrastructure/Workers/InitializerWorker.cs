using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Workers;

/// <summary>
/// This is an example of a worker service, this service is called on the applications start to do some asynchronous work.
/// </summary>
public class InitializerWorker : BackgroundService
{
    private readonly ILogger<InitializerWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InitializerWorker(ILogger<InitializerWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger; // The logger instance is injected here.
        _serviceProvider = serviceProvider; // Here the service provider is injected to request other components on runtime at request.
    }
    private async void CreateAuthors(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope(); 
            var authorService = scope.ServiceProvider.GetService<IAuthorService>(); 
                                                                                

            if (authorService == null)
            {
                _logger.LogInformation("Couldn't create the author service!");

                return;
            }

            var count = await authorService.GetAuthorCount(cancellationToken);

            if (count.Result == 0)
            {
                await authorService.AddAuthor(new()
                {
                    FirstName = "Mihai",
                    LastName = "Eminescu",
                    Description = "Poet, prozator și jurnalist român, considerat, în general, ca fiind cea mai cunoscută și influentă personalitate din literatura română."
                }, cancellationToken: cancellationToken);
                await authorService.AddAuthor(new()
                {
                    FirstName = "Eugen",
                    LastName = "Ionescu",
                    Description = "Scriitor de limbă franceză originar din România, protagonist al teatrului absurdului."
                }, cancellationToken: cancellationToken);
                await authorService.AddAuthor(new()
                {
                    FirstName = "Mircea",
                    LastName = "Eliade",
                    Description = "Istoric al religiilor, scriitor de ficțiune, filozof și profesor de origine română."
                }, cancellationToken: cancellationToken);

                await authorService.AddAuthor(new()
                {
                    FirstName = "Albert",
                    LastName = "Camus",
                    Description = "Romancier, dramaturg și filozof algerian, francez, reprezentant al existențialismului."
                }, cancellationToken: cancellationToken);
                await authorService.AddAuthor(new()
                {
                    FirstName = "James",
                    LastName = "Joyce",
                    Description = "Prozator și poet irlandez, considerat unul dintre cei mai importanți scriitori ai secolului al XX-lea."
                }, cancellationToken: cancellationToken);

                await authorService.AddAuthor(new()
                {
                    FirstName = "Feodor",
                    LastName = "Dostoievski",
                    Description = "Unul din cei mai importanți scriitori ruși."
                }, cancellationToken: cancellationToken);
                await authorService.AddAuthor(new()
                {
                    FirstName = "Franz",
                    LastName = "Kafka",
                    Description = "Scriitor de limbă germană, evreu originar din Praga."
                }, cancellationToken: cancellationToken);

                await authorService.AddAuthor(new()
                {
                    FirstName = "Charles",
                    LastName = "Baudelaire",
                    Description = "Poet francez care a produs, de asemenea, lucrări notabile ca eseist."
                }, cancellationToken: cancellationToken);

                await authorService.AddAuthor(new()
                {
                    FirstName = "Edgar Allan",
                    LastName = "Poe",
                    Description = "Scriitor, poet, redactor și critic literar american."
                }, cancellationToken: cancellationToken);

                await authorService.AddAuthor(new()
                {
                    FirstName = "Victor",
                    LastName = "Hugo",
                    Description = "Poet, dramaturg și romancier francez."
                }, cancellationToken: cancellationToken);

                await authorService.AddAuthor(new()
                {
                    FirstName = "Knut",
                    LastName = "Hamsun",
                    Description = "Scriitor norvegian. A avut un rol important în modernizarea romanului european."
                }, cancellationToken: cancellationToken);
            }
        
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing database!");
        }
       

    }
    private async void CreateLibrarians(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var librarianService = scope.ServiceProvider.GetService<ILibrarianService>();


            if (librarianService == null)
            {
                _logger.LogInformation("Couldn't create the librarian service!");

                return;
            }

            var count = await librarianService.GetLibrarianCount(cancellationToken);

            if (count.Result == 0)
            {
                await librarianService.AddLibrarian(new()
                {
                    FirstName = "Mihai",
                    LastName = "Ionescu",
                    Position = "Director",
                    Email = "miion@bjb.com",
                    Description = "Cel mai bun director."
                }, cancellationToken: cancellationToken);
                await librarianService.AddLibrarian(new()
                {
                    FirstName = "Alexandra",
                    LastName = "Georgescu",
                    Position = "Director adjunct",
                    Email = "algeo@bjb.com",
                    Description = "Cel mai buna directoare adjuncta."
                }, cancellationToken: cancellationToken);
                await librarianService.AddLibrarian(new()
                {
                    FirstName = "Vasile",
                    LastName = "Popescu",
                    Position = "Biblotecar",
                    Email = "vapop@bjb.com",
                    Description = "Nu doarme in timpul orelor de lucru."
                }, cancellationToken: cancellationToken);

            }
       
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing database!");
        }


    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope(); // Here a new scope is created, this is useful to get new scoped instances.
            var userService = scope.ServiceProvider.GetService<IUserService>(); // Here an instance for a service is requested, it may fail if the component is not declared or
                                                                                // an exception is thrown on it’s creation.

            if (userService == null)
            {
                _logger.LogInformation("Couldn't create the user service!");

                return;
            }

            var count = await userService.GetUserCount(cancellationToken);

            if (count.Result == 0)
            {
                _logger.LogInformation("No user found, adding default user!");

                await userService.AddUser(new()
                {
                    Email = "admin@default.com",
                    Name = "Admin",
                    Role = UserRoleEnum.Admin,
                    Password = PasswordUtils.HashPassword("default")
                }, cancellationToken: cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing database!");
        }
        CreateAuthors(cancellationToken);
        CreateLibrarians(cancellationToken);
    }
}