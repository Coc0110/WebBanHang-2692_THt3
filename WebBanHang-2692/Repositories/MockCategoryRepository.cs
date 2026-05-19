using WebBanHang_2692.Models;

namespace WebBanHang_2692.Repositories
{
    public class MockCategoryRepository : ICategoryRepository
    {
        private List<Category> _categoryList;

        public MockCategoryRepository()
        {
            _categoryList = new List<Category>
        {
            new Category { Id = 1, Name = "Pokemon" },
            new Category { Id = 2, Name = "Pokeball" },
            new Category { Id = 3, Name = "PokeItem"},
            // Thêm các category khác
        };
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _categoryList;
        }
    }
}
