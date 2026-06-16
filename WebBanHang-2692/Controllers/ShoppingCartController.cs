using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebBanHang_2692.Models;
using WebBanHang_2692.Repositories;
using WebBanHang_2692.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace WebBanHang_2692.Controllers
{
    // Cờ này cực kỳ quan trọng: Ép buộc phải Đăng nhập mới được dùng giỏ hàng
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(IProductRepository productRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        // HÀM CHÌA KHÓA: Trả về tên Session gắn liền với ID của User hiện tại
        private string GetCartSessionKey()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return $"Cart_{userId}";
        }

        // 1. Hiển thị Giỏ hàng
        public IActionResult Index()
        {
            var cartKey = GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            return View(cart);
        }

        // 2. Thêm vào Giỏ hàng
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cartKey = GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            HttpContext.Session.SetObjectAsJson(cartKey, cart);
            return RedirectToAction("Index");
        }

        // 3. Xóa một món khỏi giỏ
        public IActionResult RemoveFromCart(int productId)
        {
            var cartKey = GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                HttpContext.Session.SetObjectAsJson(cartKey, cart);
            }

            return RedirectToAction("Index");
        }

        // 4. Xóa sạch giỏ hàng
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(GetCartSessionKey());
            return RedirectToAction("Index");
        }

        // Hiển thị form thanh toán
        public IActionResult Checkout()
        {
            var cartKey = GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            if (!cart.Any())
            {
                return RedirectToAction("Index");
            }
            return View(new Order());
        }

        // Xử lý khi bấm Đặt hàng (Đồng bộ ngược)
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cartKey = GetCartSessionKey();
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(cartKey) ?? new List<CartItem>();
            if (!cart.Any()) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            order.UserId = user.Id;
            order.OrderDate = DateTime.Now;
            order.TotalPrice = cart.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = new List<OrderDetail>();

            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            order.Notes = order.Notes ?? "";

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Lưu Đơn hàng

            // TÍNH NĂNG ĐỒNG BỘ NGƯỢC: Cập nhật lại Profile nếu có thông tin mới
            bool profileChanged = false;

            // Cập nhật tên nếu chưa có hoặc có sự thay đổi
            if (string.IsNullOrEmpty(user.FullName) || user.FullName != order.ReceiverName)
            {
                user.FullName = order.ReceiverName;
                profileChanged = true;
            }
            // Cập nhật SĐT
            if (string.IsNullOrEmpty(user.PhoneNumber) || user.PhoneNumber != order.PhoneNumber)
            {
                user.PhoneNumber = order.PhoneNumber;
                profileChanged = true;
            }
            // Cập nhật Địa chỉ
            if (string.IsNullOrEmpty(user.Address) || user.Address != order.ShippingAddress)
            {
                user.Address = order.ShippingAddress;
                profileChanged = true;
            }

            // Lưu vào Identity Database nếu có bất kỳ sự thay đổi nào
            if (profileChanged)
            {
                await _userManager.UpdateAsync(user);
            }

            HttpContext.Session.Remove(cartKey); // Xóa giỏ hàng
            return View("OrderSuccess", order.Id);
        }
    }
}