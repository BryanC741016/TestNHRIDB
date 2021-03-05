﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NHRIDB_DAL.DbModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class NHRIDBEntitiesDB : DbContext
    {
        public NHRIDBEntitiesDB()
            : base("name=NHRIDBEntitiesDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GroupUser> GroupUser { get; set; }
        public virtual DbSet<MenuName> MenuName { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<LogLogin> LogLogin { get; set; }
        public virtual DbSet<Hospital> Hospital { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Diagnosis> Diagnosis { get; set; }
        public virtual DbSet<Region> Region { get; set; }
        public virtual DbSet<TubeData> TubeData { get; set; }
        public virtual DbSet<TubeDataLog> TubeDataLog { get; set; }
    
        public virtual ObjectResult<GetTotal_Result> GetTotal(string hospitalId)
        {
            var hospitalIdParameter = hospitalId != null ?
                new ObjectParameter("hospitalId", hospitalId) :
                new ObjectParameter("hospitalId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetTotal_Result>("GetTotal", hospitalIdParameter);
        }
    
        public virtual ObjectResult<GetDifferentTotal_Result> GetDifferentTotal(Nullable<System.Guid> hospitalId)
        {
            var hospitalIdParameter = hospitalId.HasValue ?
                new ObjectParameter("hospitalId", hospitalId) :
                new ObjectParameter("hospitalId", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetDifferentTotal_Result>("GetDifferentTotal", hospitalIdParameter);
        }
    }
}
