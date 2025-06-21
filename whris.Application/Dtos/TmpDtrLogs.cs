namespace whris.Application.Dtos
{
    public class TmpDtrLogs
    {
        public int EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? LogType { get; set; }
    }
}
