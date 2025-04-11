using ASM_NhomSugar_SD19311.Enums;

namespace ASM_NhomSugar_SD19311.Helpers
{
    public static class EnumHelper
    {
        public static string GetOrderStatusDisplayName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Placed => "Đang xử lý",
                OrderStatus.Confirmed => "Đã xác nhận",
                OrderStatus.Shipping => "Đang giao",
                OrderStatus.Delivered => "Đã giao",
                OrderStatus.Cancelled => "Đã hủy",
                _ => status.ToString()
            };
        }
    }
}
