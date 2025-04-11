namespace ASM_NhomSugar_SD19311.DTO
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Chỉ dùng khi tạo/sửa
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
    }
}
