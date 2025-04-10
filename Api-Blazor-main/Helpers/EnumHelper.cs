namespace ASM_NhomSugar_SD19311.Helpers
{
    public static class EnumHelper
    {
        public static string GetOrderStatusDisplayName(string status)
        {
            return status switch
            {
                "Placed" => "Đang xử lý", // Map "Placed" to "Đang xử lý" as per the screenshot
                "Delivered" => "Đã giao",
                "Cancelled" => "Hủy",
                _ => status
            };
        }
    }
}
