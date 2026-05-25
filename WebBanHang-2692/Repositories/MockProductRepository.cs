using System.Collections.Generic;
using System.Linq;
using WebBanHang_2692.Models;

public class MockProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    public MockProductRepository()
    {
        // Khởi tạo trước danh mục để dùng chung
        var catPokemon = new Category { Id = 1, Name = "Pokemon" };
        var catPokeball = new Category { Id = 2, Name = "Pokeball" };

        _products = new List<Product>
        {
            new Product { Id = 1, Name = "Bulbasaur", Price = 1000, Description = "Grass / Poison", ImageUrl = "/images/Pokemon/0001_bulbasaur.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 2, Name = "Ivysaur", Price = 1500, Description = "Grass / Poison", ImageUrl = "/images/Pokemon/0002_ivysaur.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 3, Name = "Venusaur", Price = 3000, Description = "Grass / Poison", ImageUrl = "/images/Pokemon/0003_venusaur.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 4, Name = "Charmander", Price = 1000, Description = "Fire", ImageUrl = "/images/Pokemon/0004_charmander.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 5, Name = "Charmeleon", Price = 1500, Description = "Fire", ImageUrl = "/images/Pokemon/0005_charmeleon.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 6, Name = "Charizard", Price = 3500, Description = "Fire / Flying", ImageUrl = "/images/Pokemon/0006_charizard.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 7, Name = "Squirtle", Price = 1000, Description = "Water", ImageUrl = "/images/Pokemon/0007_squirtle.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 8, Name = "Wartortle", Price = 1500, Description = "Water", ImageUrl = "/images/Pokemon/0008_wartortle.jpg", CategoryId = 1, Category = catPokemon },
            new Product { Id = 9, Name = "Blastoise", Price = 3000, Description = "Water", ImageUrl = "/images/Pokemon/0009_blastoise.jpg", CategoryId = 1, Category = catPokemon },
            
            new Product { Id = 10, Name = "Poke Ball", Price = 200, Description = "Standard ball", ImageUrl = "/images/PokeBall/Poke_Ball.webp", CategoryId = 2, Category = catPokeball },
            new Product { Id = 11, Name = "Great Ball", Price = 600, Description = "Higher catch rate", ImageUrl = "/images/PokeBall/Great_Ball.webp", CategoryId = 2, Category = catPokeball },
            new Product { Id = 12, Name = "Ultra Ball", Price = 1200, Description = "Very high catch rate", ImageUrl = "/images/PokeBall/Ultra_Ball.webp", CategoryId = 2, Category = catPokeball },
            new Product { Id = 13, Name = "Master Ball", Price = 50000, Description = "100% catch rate", ImageUrl = "/images/PokeBall/Master_Ball.webp", CategoryId = 2, Category = catPokeball },
            new Product { Id = 14, Name = "Beast Ball", Price = 10000, Description = "For Ultra Beasts", ImageUrl = "/images/PokeBall/Beast_Ball.webp", CategoryId = 2, Category = catPokeball }
        };
    }

    public IEnumerable<Product> GetAll()
    {
        return _products;
    }

    public Product GetById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
    }

    public void Update(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            _products[index] = product;
        }
    }

    public void Delete(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
        }
    }
}