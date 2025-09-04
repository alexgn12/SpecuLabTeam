using GammaAI.Core.Models;
using GammaAI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GammaAI.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(GammaDbContext context)
        {
            // Asegurar que la base de datos está creada
            await context.Database.EnsureCreatedAsync();

            // Si ya hay datos, no hacer nada
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Electrónicos", Description = "Dispositivos y gadgets electrónicos" },
                new Category { Name = "Ropa", Description = "Vestimenta y accesorios" },
                new Category { Name = "Hogar", Description = "Artículos para el hogar" },
                new Category { Name = "Deportes", Description = "Equipamiento deportivo" },
                new Category { Name = "Libros", Description = "Literatura y material educativo" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                // Electrónicos
                new Product { Name = "iPhone 15", Description = "Smartphone de última generación", Price = 999.99m, Stock = 50, CategoryId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "MacBook Pro", Description = "Laptop profesional", Price = 2499.99m, Stock = 25, CategoryId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "iPad Air", Description = "Tablet versátil", Price = 599.99m, Stock = 40, CategoryId = 1, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                
                // Ropa
                new Product { Name = "Camisa Polo", Description = "Camisa casual de algodón", Price = 49.99m, Stock = 100, CategoryId = 2, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Jeans Clásicos", Description = "Pantalones de mezclilla", Price = 79.99m, Stock = 75, CategoryId = 2, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Chaqueta de Cuero", Description = "Chaqueta elegante", Price = 199.99m, Stock = 30, CategoryId = 2, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                
                // Hogar
                new Product { Name = "Cafetera Express", Description = "Cafetera automática", Price = 149.99m, Stock = 60, CategoryId = 3, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Aspiradora Robot", Description = "Limpieza automática", Price = 299.99m, Stock = 35, CategoryId = 3, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Lámpara LED", Description = "Iluminación eficiente", Price = 39.99m, Stock = 120, CategoryId = 3, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                
                // Deportes
                new Product { Name = "Bicicleta Montaña", Description = "Bicicleta todo terreno", Price = 449.99m, Stock = 20, CategoryId = 4, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Pelota de Fútbol", Description = "Pelota oficial", Price = 29.99m, Stock = 200, CategoryId = 4, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Raqueta de Tenis", Description = "Raqueta profesional", Price = 129.99m, Stock = 45, CategoryId = 4, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                
                // Libros
                new Product { Name = "Cien Años de Soledad", Description = "Novela de García Márquez", Price = 19.99m, Stock = 80, CategoryId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "El Quijote", Description = "Clásico de Cervantes", Price = 24.99m, Stock = 65, CategoryId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                new Product { Name = "Manual de .NET", Description = "Guía de programación", Price = 59.99m, Stock = 40, CategoryId = 5, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
