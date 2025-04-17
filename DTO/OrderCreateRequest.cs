namespace ASM_NhomSugar_SD19311.DTO
{
    public class OrderCreateRequest
    {
        public int AccountId { get; set; }
        public int AddressId { get; set; }
        public string PaymentMode { get; set; }
        public List<CartDto> Carts { get; set; }
    }
}
