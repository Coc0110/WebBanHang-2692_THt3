using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebBanHang_2692.Models;
using WebBanHang_2692.Repositories;

namespace WebBanHang_2692.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // Hiển thị form thêm sản phẩm
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        // Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
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
                return RedirectToAction("Index");
            }

            // Nếu form lỗi, phải nạp lại danh sách Category để không bị lỗi View
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
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

        // Display a list of products
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // Display a single product
        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
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

            // Cần nạp danh sách Category để hiển thị lên Dropdown list trong View
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        // Process the product update
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile mainImage)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Images");

            if (ModelState.IsValid)
            {
                if (mainImage != null)
                {
                    product.ImageUrl = await SaveImage(mainImage);
                }

                await _productRepository.UpdateAsync(product);
                return RedirectToAction("Index");
            }

            // Nếu form lỗi, phải nạp lại danh sách Category
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
            return RedirectToAction("Index");
        }

        // Lọc sản phẩm theo danh mục
        // Lọc sản phẩm theo danh mục (Có phân trang)
        public async Task<IActionResult> ByCategory(int id, int page = 1)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();

            var products = await _productRepository.GetAllAsync();
            var filteredProducts = products.Where(p => p.CategoryId == id).ToList();

            // Logic phân trang y hệt Trang chủ
            int pageSize = 12;
            int totalProducts = filteredProducts.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var productsOnPage = filteredProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CategoryName = category.Name;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Các biến báo cho giao diện biết nó đang ở trang Danh mục
            ViewBag.ControllerName = "Product";
            ViewBag.ActionName = "ByCategory";
            ViewBag.CategoryId = id;

            return View("~/Views/Home/Index.cshtml", productsOnPage);
        }
    }
}