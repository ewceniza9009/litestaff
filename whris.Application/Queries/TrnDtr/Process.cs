using Dapper;
using Microsoft.Data.SqlClient;
using whris.Data.Data;

namespace whris.Application.Queries.TrnDtr
{
    public class GetEmployees
    {
        public int PayrollGroupId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        
        public List<Employee> Result() 
        {
            var result = new List<Employee>();
            //string sql = $@"SELECT ID FROM MstEmployee WHERE IsLocked=1 AND Id={EmployeeId}";
            string sql = $@"SELECT ID, IsLocked FROM MstEmployee WHERE Id={EmployeeId}";

            if (EmployeeId is null)
            {
                if (DepartmentId is null)
                {
                    //sql = $@"SELECT ID FROM MstEmployee WHERE IsLocked=1 AND PayrollGroupId={PayrollGroupId}";
                    sql = $@"SELECT ID, IsLocked FROM MstEmployee WHERE PayrollGroupId={PayrollGroupId}";
                }
                else
                {
                    //sql = $@"SELECT ID FROM MstEmployee WHERE IsLocked=1 AND DepartmentId={DepartmentId} AND PayrollGroupId={PayrollGroupId}";
                    sql = $@"SELECT ID, IsLocked FROM MstEmployee WHERE DepartmentId={DepartmentId} AND PayrollGroupId={PayrollGroupId}";
                }
            }

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Employee>(sql).ToList();
            };

            return result;
        }

        public class Employee
        {
            public int Id { get; set; }
            public bool IsLocked { get; set; }
        }
    }
}
