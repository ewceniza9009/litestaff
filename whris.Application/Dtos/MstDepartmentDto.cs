namespace whris.Application.Dtos
{
    public class MstDepartmentDto
    {
        public int Id { get; set; }

        public string Department { get; set; } = null!;

        public bool IsDeleted { get; set; }
    }
}
