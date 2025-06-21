using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using whris.Data.Data;

namespace whris.Application.Queries.TrnDtr
{
    public class MissingPunches
    {
        public int DTRId { get; set; }
        public List<Record> Result()
        {
            var result = new List<Record>();
			string sql = $@"SELECT QueryMissingSwipesQ004.Id, 
				QueryMissingSwipesQ004.DTRId, 
				QueryMissingSwipesQ004.EmployeeId, 
				QueryMissingSwipesQ004.Employee, 
				QueryMissingSwipesQ004.Date, 
				QueryMissingSwipesQ004.Day, 
				QueryMissingSwipesQ004.TimeIn1, 
				QueryMissingSwipesQ004.TimeOut1, 
				QueryMissingSwipesQ004.TimeIn2, 
				QueryMissingSwipesQ004.TimeOut2, 
				QueryMissingSwipesQ004.DTRRemarks, 
				QueryMissingSwipesQ004.Is4Swipes, 
				QueryMissingSwipesQ004.HasMissingPunches, 
				QueryMissingSwipesQ004.IsAbsent
			FROM 
			(
				SELECT QueryMissingSwipesQ002.Id, 
							QueryMissingSwipesQ002.DTRId, 
							QueryMissingSwipesQ002.EmployeeId, 
							QueryMissingSwipesQ002.Employee, 
							QueryMissingSwipesQ002.Date, 
							QueryMissingSwipesQ002.Day, 
							QueryMissingSwipesQ002.TimeIn1, 
							QueryMissingSwipesQ002.TimeOut1, 
							QueryMissingSwipesQ002.TimeIn2, 
							QueryMissingSwipesQ002.TimeOut2,
							QueryMissingSwipesQ002.DTRRemarks,
							IIf([MstShiftCodeDay].[TimeIn1] Is Not Null And [MstShiftCodeDay].[TimeOut1] Is Not Null And [MstShiftCodeDay].[TimeIn2] Is Not Null And [MstShiftCodeDay].[TimeOut2] Is Not Null,1,0) AS Is4Swipes,
							IIf(IIf([MstShiftCodeDay].[TimeIn1] Is Not Null And [MstShiftCodeDay].[TimeOut1] Is Not Null And [MstShiftCodeDay].[TimeIn2] Is Not Null And [MstShiftCodeDay].[TimeOut2] Is Not Null, 1, 0) = 1, IIF([QueryMissingSwipesQ002].[TimeIn1] Is Null, 1, 0) | IIF([QueryMissingSwipesQ002].[TimeOut1] Is Null, 1, 0) | IIF([QueryMissingSwipesQ002].[TimeIn2] Is Null, 1, 0) | IIF([QueryMissingSwipesQ002].[TimeOut2] Is Null, 1, 0), IIF([QueryMissingSwipesQ002].[TimeIn1] Is Null, 1, 0) | IIF([QueryMissingSwipesQ002].[TimeOut2] Is Null, 1, 0)) AS HasMissingPunches,
							IIF([QueryMissingSwipesQ002].[TimeIn1] Is Null And [QueryMissingSwipesQ002].[TimeOut1] Is Null And [QueryMissingSwipesQ002].[TimeIn2] Is Null And [QueryMissingSwipesQ002].[TimeOut2] Is Null, 1, 0) IsAbsent
						FROM 
						(
							SELECT TrnDTRLine.Id,
								TrnDTRLine.DTRId, 
								TrnDTRLine.EmployeeId, 
								MstEmployee.FullName AS Employee, 
								TrnDTRLine.ShiftCodeId, 
								TrnDTRLine.Date,
								Format([Date],'dddd') AS [Day], 
								TrnDTRLine.TimeIn1, 
								TrnDTRLine.TimeOut1, 
								TrnDTRLine.TimeIn2, 
								TrnDTRLine.TimeOut2,
								TrnDTRLine.DTRRemarks
							FROM TrnDTRLine LEFT JOIN MstEmployee ON TrnDTRLine.EmployeeId = MstEmployee.Id
							WHERE dbo.MstEmployee.IsLocked = 1
						) QueryMissingSwipesQ002 INNER JOIN MstShiftCodeDay ON ([QueryMissingSwipesQ002].[Day] = [MstShiftCodeDay].[Day]) AND (QueryMissingSwipesQ002.ShiftCodeId = MstShiftCodeDay.ShiftCodeId)
			) QueryMissingSwipesQ004
			WHERE QueryMissingSwipesQ004.HasMissingPunches = 1 AND QueryMissingSwipesQ004.IsAbsent = 0 AND DTRId={DTRId}
			ORDER BY QueryMissingSwipesQ004.Employee;";

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                result = connection.Query<Record>(sql).ToList();
            };

            return result;
        }

        public class Record
        {
			public int Id { get ; set; } 
			public int DTRId { get ; set; } 
			public int EmployeeId { get ; set; } 
			public string? Employee { get ; set; }
			public int ShiftCodeId => new HRISContext().TrnDtrlines.FirstOrDefault(x => x.Id == Id)?.ShiftCodeId ?? 0;
			public string ShiftCode => new HRISContext().MstShiftCodes.FirstOrDefault(x => x.Id == ShiftCodeId)?.ShiftCode ?? "NA";
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime Date { get; set; }
            public string? Day { get ; set; }
            [Column(TypeName = "datetime")]
            [DataType(DataType.DateTime)]
            public DateTime? TimeIn1 { get ; set; }
            [Column(TypeName = "datetime")]
            [DataType(DataType.DateTime)]
            public DateTime? TimeOut1 { get ; set; }
            [Column(TypeName = "datetime")]
            [DataType(DataType.DateTime)]
            public DateTime? TimeIn2 { get ; set; }
            [Column(TypeName = "datetime")]
            [DataType(DataType.DateTime)]
            public DateTime? TimeOut2 { get ; set; } 
            public string? Dtrremarks { get ; set; }
            public bool Is4Swipes { get ; set; } 
			public bool HasMissingPunches { get ; set; } 
			public bool IsAbsent { get; set; }
        }
    }
}
