using Dapper;
using Microsoft.Data.SqlClient;
using whris.Application.Library;
using whris.Data.Data;

namespace whris.Application.Common
{
    public class Utilities
    {
        static HRISContext _context
        {
            get => new HRISContext();
        }

        public static void UpdateEntityAuditFields(dynamic entity) 
        {
            var entityName = entity.GetType().Name
                .Replace("Detail", string.Empty)
                .Replace("Dto", string.Empty);

            var entityFromDb = new Entity();
            var currentUserId = Security.GetCurrentDBUserId();

            using (var connection = new SqlConnection(Config.ConnectionString))
            {
                if (entityName == "TrnOTApplication")
                {
                    entityName = "TrnOverTime";
                }

                if (entityName == "TrnLoan")
                {
                    entityName = "MstEmployeeLoan";
                }

                if (entityName == "TrnChangeShiftCode")
                {
                    entityName = "TrnChangeShift";
                }

                var query = $"SELECT * FROM {entityName} WHERE Id={entity.Id}";

                entityFromDb = connection.Query<Entity>(query).FirstOrDefault();
            };

            var entryUserId = entityFromDb?.EntryUserId ?? 0;
            var entryDateTime = entityFromDb?.EntryDateTime ?? DateTime.Now;

            entity.EntryUserId = entryUserId;
            entity.EntryDateTime = entryDateTime;

            entity.UpdateUserId = currentUserId;
            entity.UpdateDateTime = DateTime.Now;

            if (entryUserId == 0)
            {
                entity.EntryUserId = currentUserId;
                entity.EntryDateTime = DateTime.Now;
            }
        }

        public class Entity
        {
            public int? EntryUserId { get; set; }

            public DateTime EntryDateTime { get; set; }

            public int UpdateUserId { get; set; }

            public DateTime UpdateDateTime { get; set; }
        }
    }
}
