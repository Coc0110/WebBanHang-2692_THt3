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

        // Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile? imageUrl, List<IFormFile>? imageUrls)
        {
            ModelState.Remove("Category"); // Loại bỏ kiểm tra Navigation property

            if (imageUrl == null)
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng chọn ảnh đại diện cho sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    product.ImageUrl = await SaveImage(imageUrl);
                }
                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.Images = new List<ProductImage>();
                    foreach (var file in imageUrls)
                    {
                        product.Images.Add(new ProductImage { Url = await SaveImage(file) });
                    }
                }

                await _productRepository.AddAsync(product);
                TempData["Message"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Show the product update form
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        // Process the product update
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile? mainImage, List<IFormFile>? imageUrls)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Images");
            ModelState.Remove("Category"); // Loại bỏ kiểm tra Navigation property

            if (ModelState.IsValid)
            {
                if (mainImage != null)
                {
                    product.ImageUrl = await SaveImage(mainImage);
                }

                if (imageUrls != null && imageUrls.Count > 0)
                {
                    product.Images = new List<ProductImage>();
                    foreach (var file in imageUrls)
                    {
                        product.Images.Add(new ProductImage { Url = await SaveImage(file), ProductId = product.Id });
                    }
                }

                await _productRepository.UpdateAsync(product);
                TempData["Message"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Show the product delete confirmation
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Process the product deletion
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            TempData["Message"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        // Delete secondary image
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            await _productRepository.DeleteImageAsync(imageId);
            TempData["Message"] = "Đã xóa ảnh phụ thành công!";
            return RedirectToAction("Update", new { id = productId });
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }
    }
}