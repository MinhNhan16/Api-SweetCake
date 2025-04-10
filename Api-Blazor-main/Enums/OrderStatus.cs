namespace ASM_NhomSugar_SD19311.Enums
{
    public enum OrderStatus
    {
        Placed = 0,     // Đặt (Order placed) -> Đang xử lý
        Confirmed = 1,  // Đã xác nhận (Order confirmed)
        Shipping = 2,   // Đang giao (Order shipping)
        Delivered = 3,  // Đã giao (Order delivered)
        Cancelled = 4   // Hủy (Order cancelled)
    }
}
