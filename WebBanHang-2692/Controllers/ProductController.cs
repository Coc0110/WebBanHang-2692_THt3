using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http; // Thêm thư viện để dùng IFormFile
using System.IO; // Thêm thư viện để xử lý đường dẫn và luồng file (Path, FileStream)
using System.Threading.Tasks; // Thêm thư viện để dùng bất đồng bộ (Task, async/await)
using System.Collections.Generic; // Thêm thư viện để dùng List<>
using WebBanHang_2692.Models;
using WebBanHang_2692.Repositories;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public IActionResult Add()
    {
        var categories = _categoryRepository.GetAllCategories();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    // --- ĐOẠN CODE ĐƯỢC CẬP NHẬT TỪ HÌNH ẢNH ---
    [HttpPost]
    public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
    {
        if (ModelState.IsValid)
        {
            if (imageUrl != null)
            {
                // Lưu hình ảnh đại diện
                product.ImageUrl = await SaveImage(imageUrl);
            }
            if (imageUrls != null)
            {
                product.ImageUrls = new List<string>();
                foreach (var file in imageUrls)
                {
                    // Lưu các hình ảnh khác
                    product.ImageUrls.Add(await SaveImage(file));
                }
            }
            _productRepository.Add(product);
            return RedirectToAction("Index");
        }
        return View(product);
    }
    private async Task<string> SaveImage(IFormFile image)
    {
        // Thay đổi đường dẫn theo cấu hình của bạn
        var savePath = Path.Combine("wwwroot/images", image.FileName);
        using (var fileStream = new FileStream(savePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }
        return "/images/" + image.FileName; // Trả về đường dẫn tương đối
    }
    // --- KẾT THÚC ĐOẠN CẬP NHẬT ---

    // Các actions khác như Display, Update, Delete

    // Display a list of products
    public IActionResult Index()
    {
        var products = _productRepository.GetAll();
        return View(products);
    }

    // Display a single product
    public IActionResult Display(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // Show the product update form
    public IActionResult Update(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // Process the product update
    [HttpPost]
    public async Task<IActionResult> Update(Product product, IFormFile mainImage)
    {
        // Bỏ qua lỗi validation cho các trường hình ảnh vì ta tự xử lý
        ModelState.Remove("ImageUrl");
        ModelState.Remove("ImageUrls");

        if (ModelState.IsValid)
        {
            if (mainImage != null)
            {
                // Nếu người dùng tải ảnh mới lên, lưu ảnh và cập nhật lại đường dẫn mới
                product.ImageUrl = await SaveImage(mainImage);
            }
            // Nếu mainImage == null (người dùng không tải ảnh mới), 
            // product.ImageUrl sẽ tự động giữ lại giá trị đường dẫn cũ nhờ thẻ input hidden bên View.

            _productRepository.Update(product);
            return RedirectToAction("Index");
        }
        return View(product);
    }

    // Show the product delete confirmation
    public IActionResult Delete(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // Process the product deletion
    [HttpPost, ActionName("DeleteConfirmed")]
    public IActionResult DeleteConfirmed(int id)
    {
        _productRepository.Delete(id);
        return RedirectToAction("Index");
    }
}