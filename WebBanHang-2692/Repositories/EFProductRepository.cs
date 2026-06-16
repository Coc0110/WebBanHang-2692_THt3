using Microsoft.EntityFrameworkCore;
using WebBanHang_2692.Models;

namespace WebBanHang_2692.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // Code lấy danh sách (bao gồm cả Category để hiển thị tên danh mục)
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            // Lấy sản phẩm kèm theo cả danh sách Hình ảnh phụ
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Images)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}