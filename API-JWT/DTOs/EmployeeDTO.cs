namespace API_JWT.DTOs
{
    public class EmployeeDTO
    {
        public string Name { get; set; } = null!;
        public string? Surname { get; set; }
        public int Age { get; set; }
        public int? DepartmentId { get; set; }
    }
}
