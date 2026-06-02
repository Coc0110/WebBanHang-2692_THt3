namespace WebBanHang_2692.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        public string Url { get; set; }

        // Khóa ngoại liên kết với bảng Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}