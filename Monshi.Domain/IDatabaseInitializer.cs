namespace Monshi.Domain;

public interface IDatabaseInitializer
{
    Task SeedAsync();
}