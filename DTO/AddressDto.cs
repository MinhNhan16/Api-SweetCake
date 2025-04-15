namespace ASM_NhomSugar_SD19311.DTO
{
    public class AddressDto
    {
        public int Id { get; set; } // Mã địa chỉ (PK)
        public string Street { get; set; } // Đường phố
        public string City { get; set; } // Thành phố
        public string State { get; set; } // Tỉnh
        public string Country { get; set; } // Quốc gia
        public int AccountId { get; set; } // Mã người dùng (FK)
    }
}
