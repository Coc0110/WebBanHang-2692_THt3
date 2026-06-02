using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebBanHang_2692.Models;

namespace WebBanHang_2692.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Sau này sẽ hiển thị danh sách hóa đơn
        }
    }
}