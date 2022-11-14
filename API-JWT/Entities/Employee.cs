namespace API_JWT.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Surname { get; set; }
        public int Age { get; set; }
        public Department? Department { get; set; }
        public int? DepartmentId { get; set; }
    }
}
