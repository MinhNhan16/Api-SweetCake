using ASM_NhomSugar_SD19311.Model;

namespace ASM_NhomSugar_SD19311.DTO
{
    public class OrderDto
    {
        public string Id { get; set; } // Mã đơn hàng (PK)
        public DateTime OrderDate { get; set; } // Ngày đặt hàng
        public decimal TotalPrice { get; set; } // Tổng giá
        public string PaymentMode { get; set; } // Phương thức thanh toán
        public int OrderStatus { get; set; } // Trạng thái đơn hàng
        public int AccountId { get; set; } // Mã người dùng (FK)
        public int AddressId { get; set; } // Mã địa chỉ (FK)


        public List<OrderDetails>? OrderDetails { get; set; }

    }
}
