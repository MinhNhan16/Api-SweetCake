namespace ASM_NhomSugar_SD19311.DTO
{
    public class AddProductToCartDto
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
