using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks; // Thêm dòng này để hỗ trợ Task
using WebBanHang_2692.Models;
using WebBanHang_2692.Repositories;

namespace WebBanHang_2692.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _productRepository;

        public HomeController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // 1. THÊM ASYNC VÀ TASK VÀO ĐÂY
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 12; // Số sản phẩm trên 1 trang (3 hàng x 4 cột = 12)

            // 2. THÊM AWAIT VÀ ĐỔI THÀNH GetAllAsync()
            var allProducts = await _productRepository.GetAllAsync();

            // Đếm tổng số sản phẩm hiện có
            int totalProducts = allProducts.Count();

            // Tính tổng số trang (Ví dụ: 14 / 12 = 1.16 làm tròn lên là 2 trang)
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            // Cắt lấy dữ liệu của trang hiện tại bằng Skip và Take
            var productsOnPage = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền dữ liệu phân trang ra View bằng ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Gửi danh sách đã cắt (productsOnPage) thay vì toàn bộ (allProducts)
            return View(productsOnPage);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}