using ClinicalKnowledgeControl.BLL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicalKnowledgeControl.DAL.Repositories
{
    public class ClinicalGuidelinesRepository
    {
        public DataTable GetAll()
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "SELECT Id, Name, ICDCode, UpdateDate, EffectiveDate, FileLink, Description FROM ClinicalGuidelines WHERE IsActive = 1 ORDER BY Name";
                using (var adapter = new SqlDataAdapter(query, conn))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// Получить клиническую рекомендацию по Id
        /// </summary>
        /// <param name="id">Идентификатор клинической рекомендации</param>
        /// <returns>Объект ClinicalGuideline или null, если не найдено</returns>
        public ClinicalGuideline GetById(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    SELECT Id, Name, ICDCode, UpdateDate, EffectiveDate, 
                           FileLink, Description, IsActive, CreatedDate
                    FROM ClinicalGuidelines
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReaderToClinicalGuideline(reader);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Маппинг данных из SqlDataReader в объект ClinicalGuideline
        /// </summary>
        private ClinicalGuideline MapReaderToClinicalGuideline(SqlDataReader reader)
        {
            return new ClinicalGuideline
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                ICDCode = reader.IsDBNull(reader.GetOrdinal("ICDCode"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("ICDCode")),
                UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdateDate"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("UpdateDate")),
                EffectiveDate = reader.IsDBNull(reader.GetOrdinal("EffectiveDate"))
                    ? (DateTime?)null
                    : reader.GetDateTime(reader.GetOrdinal("EffectiveDate")),
                FileLink = reader.IsDBNull(reader.GetOrdinal("FileLink"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("FileLink")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("Description")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
            };
        }

        public int Insert(string name, string icdCode, DateTime? updateDate, DateTime? effectiveDate, string fileLink, string description)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    INSERT INTO ClinicalGuidelines (Name, ICDCode, UpdateDate, EffectiveDate, FileLink, Description)
                    VALUES (@Name, @ICDCode, @UpdateDate, @EffectiveDate, @FileLink, @Description);
                    SELECT SCOPE_IDENTITY();";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ICDCode", string.IsNullOrEmpty(icdCode) ? (object)DBNull.Value : icdCode);
                    cmd.Parameters.AddWithValue("@UpdateDate", updateDate.HasValue ? (object)updateDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@EffectiveDate", effectiveDate.HasValue ? (object)effectiveDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FileLink", string.IsNullOrEmpty(fileLink) ? (object)DBNull.Value : fileLink);
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public void Update(int id, string name, string icdCode, DateTime? updateDate, DateTime? effectiveDate, string fileLink, string description)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = @"
                    UPDATE ClinicalGuidelines
                    SET Name = @Name, ICDCode = @ICDCode, UpdateDate = @UpdateDate,
                        EffectiveDate = @EffectiveDate, FileLink = @FileLink, Description = @Description
                    WHERE Id = @Id";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ICDCode", string.IsNullOrEmpty(icdCode) ? (object)DBNull.Value : icdCode);
                    cmd.Parameters.AddWithValue("@UpdateDate", updateDate.HasValue ? (object)updateDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@EffectiveDate", effectiveDate.HasValue ? (object)effectiveDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FileLink", string.IsNullOrEmpty(fileLink) ? (object)DBNull.Value : fileLink);
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = ConnectionManager.GetConnection())
            {
                string query = "UPDATE ClinicalGuidelines SET IsActive = 0 WHERE Id = @Id";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
