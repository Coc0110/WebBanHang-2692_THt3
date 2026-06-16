using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang_2692.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebBanHang_2692.Controllers
{
    [Authorize] // Bắt buộc đăng nhập mới được xem
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy ID của User đang đăng nhập
            var user = await _userManager.GetUserAsync(User);

            // Tìm tất cả đơn hàng của User này, kéo theo chi tiết đơn hàng và thông tin sản phẩm
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }
}