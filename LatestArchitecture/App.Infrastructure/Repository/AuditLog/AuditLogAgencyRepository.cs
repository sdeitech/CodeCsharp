using App.Application.Dto.AuditLog;
using App.Application.Interfaces.Repositories.AuditLogs;
using App.Application.Interfaces;
using App.Common.Constant;
using App.Domain.Entities.AuditLog;
using App.Domain.Entities.AuditLogs.AuditLog;
using Dapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using static App.Common.Constant.Constants;
using App.Application.Dto.Common;
using App.Infrastructure.DBContext;

namespace App.Infrastructure.Repository.AuditLog
{
    public class AuditLogAgencyRepository : DbConnectionRepositoryBase, IAuditLogAgencyRepository
    {
        public AuditLogAgencyRepository(ApplicationDbContext context, IDbConnectionFactory dbConnectionFactory)
        : base(context, dbConnectionFactory)
        {
            _dbContext = context;
        }
        private readonly ApplicationDbContext _dbContext;
        private readonly ConcurrentDictionary<string, int> _tableIdCache = new();
        private readonly ConcurrentDictionary<string, int> _columnIdCache = new();
        private readonly ConcurrentDictionary<string, AuditLogMasterColumn> _columnCache = new();

        public int GetTableId(string tableName)
        {
            _tableIdCache.TryGetValue(tableName, out int tableId);
            return tableId;
        }

        public void InsertAuditLogs(List<AuditLogs> auditLogs)
        {
            _dbContext.AuditLogs.AddRange(auditLogs);
            _dbContext.SaveChanges();
        }

        public void PreloadMasterData()
        {
            // Load all tables and store in cache
            var tables = _dbContext.AuditLogMasterTables
                .Where(t => t.IsActive == true && t.IsDeleted == false)
                .ToDictionary(t => t.TableName, t => t.TableID);

            foreach (var table in tables)
            {
                _tableIdCache.TryAdd(table.Key, table.Value);
            }

            // Load all columns and store in cache
            var columns = _dbContext.AuditLogMasterColumn
                .Where(c => c.IsActive == true && c.IsDeleted == false)?
                .ToList();

            foreach (var column in columns)
            {
                string key = $"{column.TableID}_{column.ColumnName}";
                _columnIdCache.TryAdd(key, column.ColumnID);
                _columnCache.TryAdd(key, column);
            }
        }

        public void SaveChangesWithAuditLogs(string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId, double? latitude, double? longitude, int? PatientId)
        {
            try
            {
                // Pre-load all master tables and columns to avoid repeated queries
                PreloadMasterData();

                List<AuditLogs> auditLogs = GetChanges(screenName, action, userId, organizationId, ipAddress, portalId);

                if (auditLogs.Any())
                {
                    // Batch assignment of common properties
                    foreach (var log in auditLogs)
                    {
                        log.OrganizationID = organizationId;
                        log.IPAddress = ipAddress;
                        log.IsActive = true;
                        //log.Latitude = latitude;
                        //log.Longitude = longitude;
                        //log.PatientId = PatientId;
                    }

                    // Use bulk insert if available, or add range for better performance
                    _dbContext.AuditLogs.AddRange(auditLogs);
                }

                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error in SaveChangesWithAuditLogs: {ex.Message}");
                throw;
            }
        }

        public List<AuditLogs> GetChanges(string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId)
        {
            var entities = _dbContext.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified)
                .ToList();

            return TrackChanges(entities, screenName, action, userId, organizationId, ipAddress, portalId);
        }

        private List<AuditLogs> TrackChanges(List<EntityEntry> entities, string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId)
        {
            var auditInfoList = new List<AuditLogs>();

            // Create a lookup dictionary for entities
            var tablesDict = entities.GroupBy(e => GetTableName(e))
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var tableGroup in tablesDict)
            {
                string tableName = tableGroup.Key;

                if (!_tableIdCache.TryGetValue(tableName, out int tableId))
                {
                    // Skip if table is not configured for auditing
                    continue;
                }

                foreach (var entry in tableGroup.Value)
                {
                    try
                    {
                        var entryLogs = ApplyAuditLog(entry, screenName, action, userId, organizationId, ipAddress, portalId);
                        auditInfoList.AddRange(entryLogs);
                    }
                    catch (Exception ex)
                    {
                        // Log the error but continue processing other entries
                        Console.WriteLine($"Error processing audit log for {tableName}: {ex.Message}");
                    }
                }
            }

            return auditInfoList;
        }

        private string GetTableName(EntityEntry dbEntry)
        {
            TableAttribute tableAttr = dbEntry.Entity.GetType()
                .GetCustomAttributes(typeof(TableAttribute), false)
                .SingleOrDefault() as TableAttribute;

            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

            /*     if (tableName.Contains("_"))
                 {
                     string[] parts = tableName.Split('_');
                     return parts.Length > 0 ? parts[0] : tableName;
                 }*/

            return tableName;
        }

        private List<AuditLogs> ApplyAuditLog(EntityEntry entry, string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    return GetAddedProperties(entry, screenName, action, userId, organizationId, ipAddress, portalId);
                case EntityState.Deleted:
                    return GetDeletedProperties(entry, screenName, action, userId, organizationId, ipAddress, portalId);
                case EntityState.Modified:
                    return GetModifiedProperties(entry, screenName, action, userId, ipAddress, portalId);
                default:
                    return new List<AuditLogs>();
            }
        }

        private List<AuditLogs> GetAddedProperties(EntityEntry entry, string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId)
        {
            var auditInfoList = new List<AuditLogs>();
            string tableName = GetTableName(entry);
            /*    string[] entityName = tableName.Split('_');

                if (entityName.Length == 2)
                    tableName = entityName[0];*/

            if (!_tableIdCache.TryGetValue(tableName, out int tableId))
            {
                return auditInfoList;
            }
            /*
                        var primaryKey = entry.Metadata.FindPrimaryKey();
                        var keys = primaryKey.Properties.ToDictionary(x => x.Name);
                        int primaryKeyValue = 0;

                        if (keys != null && keys.Count > 0)
                        {
                            var keyValue = entry.CurrentValues[keys.First().Key];
                            primaryKeyValue = Convert.ToInt32(keyValue);
                        }*/

            var primaryKey = entry.Metadata.FindPrimaryKey();
            var primaryKeyNames = primaryKey?.Properties.Select(p => p.Name).ToHashSet() ?? new HashSet<string>();
            int primaryKeyValue = 0;

            if (primaryKey != null && primaryKey.Properties.Any())
            {
                var keyValue = entry.CurrentValues[primaryKey.Properties.First().Name];
                primaryKeyValue = Convert.ToInt32(keyValue);
            }

            // Get all properties to audit in one go
            var propertiesToAudit = new List<string>();
            foreach (var property in entry.CurrentValues.Properties)
            {
                if (AuditLogSkipColumns.SkipColumns(property.Name) || primaryKeyNames.Contains(property.Name))
                {
                    continue;
                }


                string columnKey = $"{tableId}_{property.Name}";
                string newVal = Convert.ToString(entry.CurrentValues[property])?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(newVal))
                {
                    propertiesToAudit.Add(property.Name);
                }
            }

            // Bulk process properties
            foreach (var propertyName in propertiesToAudit)
            {
                string columnKey = $"{tableId}_{propertyName}";
                int masterEntityId = 0;
                if (!_columnIdCache.TryGetValue(columnKey, out int columnId))
                {
                    continue;
                }

                var property = entry.CurrentValues.Properties.First(p => p.Name == propertyName);
                string newVal = Convert.ToString(entry.CurrentValues[property])?.Trim() ?? string.Empty;

                var auditInfo = new AuditLogs
                {
                    ActionID = (int)MasterActions.Add,
                    ColumnID = columnId,
                    ChangedBy = userId,
                    IPAddress = ipAddress,
                    OrganizationID = organizationId,
                    PortalID = portalId,
                    TableID = tableId,
                    ScreenName = screenName,
                    StatusID = (int)Auditlogstatus.CreatedSuccessfully,
                    LogDate = DateTime.UtcNow
                };


                // Process master entity if needed
                //if (_columnCache.TryGetValue(columnKey, out var columnInfo) && !string.IsNullOrEmpty(columnInfo.MasterEntityName))
                //{
                //    if (int.TryParse(newVal, out int masterEntitesId) && masterEntitesId > 0)
                //    {
                //        //if (tableName == TableNames.MessageRecepient && property.Name == ColumnNames.ToUserID)
                //        //{
                //        //    masterEntityId = masterEntitesId;
                //        //}

                //        string cacheKey = $"{columnInfo.MasterEntityName}_{masterEntitesId}";
                //        if (_entityValueCache.TryGetValue(cacheKey, out var cachedValue))
                //        {
                //            auditInfo.NewValue = Convert.ToString(cachedValue.Value);
                //        }
                //        else
                //        {
                //            var newData = GetMasterEntityValue(columnInfo.MasterEntityName, masterEntitesId, portalId);
                //            auditInfo.NewValue = newData != null ? Convert.ToString(newData.Value) : null;

                //            if (newData != null)
                //            {
                //                _entityValueCache.TryAdd(cacheKey, newData);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                auditInfo.NewValue = newVal;
                //}


                auditInfoList.Add(auditInfo);
            }

            return auditInfoList;
        }

        private List<AuditLogs> GetDeletedProperties(EntityEntry entry, string screenName, int action, int? userId, int? organizationId, string ipAddress, int portalId)
        {
            // Similar optimizations as GetModifiedProperties
            var auditInfoList = new List<AuditLogs>();
            string tableName = GetTableName(entry);
            string[] entityName = tableName.Split('_');

            if (entityName.Length == 2)
                tableName = entityName[0];

            if (!_tableIdCache.TryGetValue(tableName, out int tableId))
            {
                return auditInfoList;
            }

            var primaryKey = entry.Metadata.FindPrimaryKey();
            var keys = primaryKey.Properties.ToDictionary(x => x.Name);
            PropertyValues dbValues = entry.GetDatabaseValues();

            if (dbValues == null)
            {
                return auditInfoList;
            }

            int primaryKeyValue = 0;
            if (keys != null && keys.Count > 0)
            {
                var keyValue = entry.CurrentValues[keys.First().Key];
                primaryKeyValue = Convert.ToInt32(keyValue);
            }

            // Process all properties in a batched way
            var relevantProperties = new List<(string Name, string OldVal, string NewVal)>();
            foreach (var property in entry.OriginalValues.Properties)
            {
                string columnKey = $"{tableId}_{property.Name}";
                if (!_columnIdCache.ContainsKey(columnKey))
                {
                    continue;
                }

                string newVal = Convert.ToString(entry.CurrentValues[property])?.Trim() ?? string.Empty;
                string oldVal = Convert.ToString(dbValues[property])?.Trim() ?? string.Empty;

                if (oldVal != newVal)
                {
                    relevantProperties.Add((property.Name, oldVal, newVal));
                }
            }

            // Process each property with pre-fetched data
            foreach (var (propertyName, oldVal, newVal) in relevantProperties)
            {
                string columnKey = $"{tableId}_{propertyName}";
                if (!_columnIdCache.TryGetValue(columnKey, out int columnId))
                {
                    continue;
                }

                var auditInfo = new AuditLogs
                {
                    ActionID = (int)MasterActions.Delete,
                    TableID = tableId,
                    ColumnID = columnId,
                    ScreenName = screenName,
                    LogDate = DateTime.UtcNow,
                    ChangedBy = userId,
                    PortalID = portalId,
                    IPAddress = ipAddress,
                    OrganizationID = organizationId,
                    StatusID = (int)Auditlogstatus.DeletedSuccessfully,
                    OldValue = oldVal,
                    NewValue = newVal
                };

                // Process master entity if needed
                //if (_columnCache.TryGetValue(columnKey, out var columnInfo) && !string.IsNullOrEmpty(columnInfo.MasterEntityName))
                //{
                //    ProcessMasterEntityValues(columnInfo, newVal, oldVal, auditInfo, portalId);
                //}
                //else
                //{
                //    auditInfo.OldValue = oldVal;
                //    auditInfo.NewValue = newVal;
                //}

                auditInfoList.Add(auditInfo);
            }

            return auditInfoList;
        }

        private List<AuditLogs> GetModifiedProperties(EntityEntry entry, string screenName, int action, int? userId, string ipAddress, int portalId)
        {
            var auditInfoList = new List<AuditLogs>();
            string tableName = GetTableName(entry);
            /* string[] entityName = tableName.Split('_');

             if (entityName.Length == 2)
                 tableName = entityName[0];*/

            if (!_tableIdCache.TryGetValue(tableName, out int tableId))
            {
                return auditInfoList;
            }

            /*    var primaryKey = entry.Metadata.FindPrimaryKey();
                var keys = primaryKey.Properties.ToDictionary(x => x.Name);
                PropertyValues? dbValues = entry.GetDatabaseValues();

                if (dbValues == null)
                {
                    return auditInfoList;
                }

                int primaryKeyValue = 0;
                if (keys != null && keys.Count > 0)
                {
                    var keyValue = entry.CurrentValues[keys.First().Key];
                    primaryKeyValue = Convert.ToInt32(keyValue);
                }
    */
            var primaryKey = entry.Metadata.FindPrimaryKey();
            var primaryKeyNames = primaryKey?.Properties.Select(p => p.Name).ToHashSet() ?? new HashSet<string>();
            PropertyValues? dbValues = entry.GetDatabaseValues();

            if (dbValues == null)
            {
                return auditInfoList;
            }

            int primaryKeyValue = 0;
            if (primaryKey != null && primaryKey.Properties.Any())
            {
                var keyValue = entry.CurrentValues[primaryKey.Properties.First().Name];
                primaryKeyValue = Convert.ToInt32(keyValue);
            }


            // Get all changed properties in one pass
            var changedProperties = new List<string>();
            foreach (var property in entry.OriginalValues.Properties)
            {
                if (AuditLogSkipColumns.SkipColumns(property.Name) || primaryKeyNames.Contains(property.Name))
                {
                    continue;
                }


                string newVal = Convert.ToString(entry.CurrentValues[property])?.Trim() ?? string.Empty;
                string oldVal = Convert.ToString(dbValues[property])?.Trim() ?? string.Empty;

                if (oldVal != newVal)
                {
                    changedProperties.Add(property.Name);
                }
            }

            // Bulk fetch column info for all changed properties
            var columnKeys = changedProperties.Select(prop => $"{tableId}_{prop}").ToList();
            var relevantColumns = new Dictionary<string, AuditLogMasterColumn>();

            foreach (var key in columnKeys)
            {

                if (_columnCache.TryGetValue(key, out var column))
                {
                    string propName = key.Split('_')[1];
                    relevantColumns.Add(propName, column);
                }
            }

            // Now process each property with the pre-fetched data
            foreach (var property in entry.OriginalValues.Properties)
            {

                if (!changedProperties.Contains(property.Name))
                {
                    continue;
                }

                if (!relevantColumns.TryGetValue(property.Name, out var columnInfo))
                {
                    continue;
                }

                string newVal = Convert.ToString(entry.CurrentValues[property])?.Trim() ?? string.Empty;
                string oldVal = Convert.ToString(dbValues[property])?.Trim() ?? string.Empty;



                int columnId = columnInfo.ColumnID;

                var auditInfo = new AuditLogs
                {
                    ActionID = action != (int)MasterActions.Delete ? (int)MasterActions.Update : (int)MasterActions.Delete,
                    TableID = tableId,
                    ColumnID = columnId,
                    ScreenName = screenName,
                    LogDate = DateTime.UtcNow,
                    ChangedBy = userId,
                    PortalID = portalId,
                    StatusID = action != (int)MasterActions.Delete ? (int)Auditlogstatus.UpdatedSuccessfully : (int)Auditlogstatus.DeletedSuccessfully,
                    OldValue = oldVal,
                    NewValue = newVal
                };


                //if (!string.IsNullOrEmpty(columnInfo.MasterEntityName))
                //{
                //    ProcessMasterEntityValues(columnInfo, newVal, oldVal, auditInfo, portalId);
                //}
                //else
                //{
                //auditInfo.OldValue = oldVal;
                //auditInfo.NewValue = newVal;
                //}

                auditInfoList.Add(auditInfo);
            }

            return auditInfoList;
        }

        public async Task<List<AuditLogResponseDto>> GetAuditLogList(FilterDto filter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@SearchKey", filter.SearchTerm, DbType.String);
            parameters.Add("@PageNumber", filter.PageNumber, DbType.Int32);
            parameters.Add("@PageSize", filter.PageSize, DbType.Int32);
            parameters.Add("@SortColumn", filter.SortColumn, DbType.String);
            parameters.Add("@SortOrder", filter.SortOrder, DbType.String);

            return (List<AuditLogResponseDto>)await _dbConnection.QueryAsync<AuditLogResponseDto>(SqlMethod.ADT_GetAuditLogs, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
