using Examen.Models;
using Examen.Models.DTOS;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Examen.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Actor> Actors => Set<Actor>();
        public DbSet<MovieActor> MovieActors => Set<MovieActor>();
        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Screening> Screenings => Set<Screening>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<LoyaltyCard> LoyaltyCards => Set<LoyaltyCard>();
        public DbSet<Ticket> Tickets => Set<Ticket>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // MOVIE
            b.Entity<Movie>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Title).IsRequired().HasMaxLength(200);
                e.Property(x => x.DurationMin).IsRequired();
                e.HasIndex(x => x.Title); // búsqueda rápida
                e.HasMany(x => x.Screenings)
                 .WithOne(s => s.Movie)
                 .HasForeignKey(s => s.MovieId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ACTOR
            b.Entity<Actor>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.FullName).IsRequired().HasMaxLength(200);
                e.HasIndex(x => x.FullName);
            });

            // MOVIE-ACTOR (N:M)
            b.Entity<MovieActor>(e =>
            {
                e.HasKey(ma => new { ma.MovieId, ma.ActorId });
                e.HasOne(ma => ma.Movie)
                 .WithMany(m => m.MovieActors)
                 .HasForeignKey(ma => ma.MovieId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(ma => ma.Actor)
                 .WithMany(a => a.MovieActors)
                 .HasForeignKey(ma => ma.ActorId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // HALL
            b.Entity<Hall>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Capacity).IsRequired();
                e.HasIndex(x => x.Name).IsUnique(); // nombre de sala único
                e.HasMany(x => x.Screenings)
                 .WithOne(s => s.Hall)
                 .HasForeignKey(s => s.HallId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // SCREENING
            b.Entity<Screening>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.StartsAt).IsRequired();
                e.HasIndex(x => new { x.HallId, x.StartsAt }); // ayuda a validar solapamiento
                                                               // Relaciones ya configuradas desde Movie/Hall
                e.HasMany(s => s.Tickets)
                 .WithOne(t => t.Screening)
                 .HasForeignKey(t => t.ScreeningId)
                 .OnDelete(DeleteBehavior.Cascade); // si borras la función, se van los tickets
            });

            // CUSTOMER
            b.Entity<Customer>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Email).IsRequired().HasMaxLength(200);
                e.Property(x => x.FullName).IsRequired().HasMaxLength(200);
                e.Property(x => x.Active).HasDefaultValue(true);
                e.HasIndex(x => x.Email).IsUnique(); // email único
                e.HasMany(x => x.Tickets)
                 .WithOne(t => t.Customer)
                 .HasForeignKey(t => t.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // LOYALTY CARD (1:1 base del examen)
            b.Entity<LoyaltyCard>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Code).IsRequired().HasMaxLength(50);
                e.Property(x => x.Points).HasDefaultValue(0);
                e.HasIndex(x => x.Code).IsUnique(); // código de tarjeta único

                /* e.HasOne(x => x.Customer)
                  .WithOne(c => c.LoyaltyCard)
                  .HasForeignKey<LoyaltyCard>(x => x.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
                */

                e.HasOne(x => x.Customer)
                 .WithMany(c => c.LoyaltyCards)
                 .HasForeignKey(x => x.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict);

            });

            // TICKET
            b.Entity<Ticket>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.SeatNumber).IsRequired();
                e.Property(x => x.Price).IsRequired().HasColumnType("decimal(10,2)");
                e.HasIndex(x => new { x.ScreeningId, x.SeatNumber }).IsUnique(); // asiento único por función
            });
        }
    }
}
