using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Monshi.Domain;
using Monshi.Domain.Customers;
using Monshi.Domain.Logs;
using Monshi.Domain.Orders;
using Monshi.Domain.Products.Entities;
using Monshi.Domain.Users.Entities;

namespace Monshi.Data.SqlServer;

public class ApplicationDbContext:DbContext,IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }

    

    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Factor> Factors { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<FactorQuery> FactorQuery { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FactorQuery>()
            .HasNoKey()
            .ToView("MyView");
        
        /*modelBuilder.Entity<Factor>().HasOne(x => x.Customer)
            .WithMany(x => x.Factors)
            .HasForeignKey(x => x.CustomerId);
        
        modelBuilder.Entity<Factor>().HasOne(x => x.User)
            .WithMany(x => x.Factors)
            .HasForeignKey(x => x.UserId);
        
        modelBuilder.Entity<OrderItem>().HasOne(x => x.Factor)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.FactorId);
        
        modelBuilder.Entity<OrderItem>().HasOne(x => x.Product)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId);
        
        
        modelBuilder.Entity<Product>().HasOne(x => x.User)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.UserId);


        modelBuilder.Entity<Customer>().HasOne(x => x.User)
            .WithMany(x => x.Customers)
            .HasForeignKey(x => x.UserId);*/
                
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(UserConfig)));
//        modelBuilder.Entity<User>().HasQueryFilter(x => x.IsRemoved == false);

        var entities = modelBuilder.Model.GetEntityTypes()
            .Select(x => x.ClrType)
            .Where(x => x.BaseType ==typeof(BaseEntity))
            .ToList();

        foreach (var entity in entities)
        {
            var mymethod = method.MakeGenericMethod(new[] {entity});
            mymethod.Invoke(this,new []{modelBuilder});
        }
        base.OnModelCreating(modelBuilder);
    }

     public MethodInfo method = typeof(ApplicationDbContext).GetMethod("SetQueryFilter");
    public void SetQueryFilter<T>(ModelBuilder modelBuilder) where T:BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(x => x.IsRemoved == false);
    }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entries=ChangeTracker
            .Entries()
            .Where(x => x.Entity is BaseEntity)
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                ((BaseEntity)entry.Entity).CreationDate=DateTime.Now;
            }
            else  if (entry.State == EntityState.Modified)
            {
                ((BaseEntity)entry.Entity).ModificationDate=DateTime.Now;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().HaveMaxLength(100);
        configurationBuilder.Properties<TimeSpan>().HaveConversion<TimeSpanConverter>();
        base.ConfigureConventions(configurationBuilder);
    }
}