namespace IT_SegmentApi.DTOs
{
    public class CustomerDto
    {
        public class RegisterCustomerDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Postcode { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string HashPassword { get; set; } // will be hashed
        }

        public class LoginDto
        {
            public string Email { get; set; }
            public string HashPassword { get; set; }
        }
    }
}
