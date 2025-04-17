namespace ASM_NhomSugar_SD19311.DTO
{
    public class CartDto
    {
        public int Id { get; set; } // Mã giỏ hàng (PK)
        public int Quantity { get; set; } // Số lượng
        public int Size { get; set; } // Kích cỡ
        public int CheckoutCount { get; set; } // Số lượng bán
        public decimal Price { get; set; } // Giá
        public string? PaymentMode { get; set; } // Phương thức thanh toán
        public DateTime DateCreated { get; set; } // Ngày tạo
        public decimal TotalPrice { get; set; } // Tổng giá
        public float PayPalPayment { get; set; } // Tổng tiền thanh toán PayPal
        public int AccountId { get; set; } // Mã người dùng (FK)
        public int AddressId { get; set; } // Mã địa chỉ (FK)
        public int SizeId { get; set; } // Mã kích cỡ (FK
        public int ProductId { get; set; } // Mã sản phẩm (FK)
        public List<CartItemDto> CartItems { get; set; } = new List<CartItemDto>();
    }
}
