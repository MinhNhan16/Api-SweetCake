namespace ASM_NhomSugar_SD19311.DTO
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderStatus { get; set; }
        public string CustomerName { get; set; }
        public string ShipperName { get; set; }
        public string ProductNames { get; set; } // Danh sách sản phẩm trong đơn hàng


    }
}
