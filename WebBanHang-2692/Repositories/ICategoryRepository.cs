using WebBanHang_2692.Models;

namespace WebBanHang_2692.Repositories
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}
