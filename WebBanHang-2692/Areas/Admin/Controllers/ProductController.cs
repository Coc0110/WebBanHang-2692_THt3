using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebBanHang_2692.Models;
using WebBanHang_2692.Repositories; // Đổi lại thành namespace chứa Repository của bạn

namespace WebBanHang_2692.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)] // Khóa chặt, chỉ Admin mới được vào
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Tiêm (Inject) các Repository vào
        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Action 1: Hiển thị danh sách sản phẩm (Dạng bảng)
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // Action 2: Hiển thị Form Thêm mới
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // (Bạn có thể tự copy các hàm Edit, Delete, và hàm Add [HttpPost] từ bài cũ sang đây)
    }
}