using Microsoft.EntityFrameworkCore;
using SSKJ.RoadManageSystem.Models.ProjectModel;

namespace SSKJ.RoadManageSystem.Repository.MySQL.Project
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }
        public virtual DbSet<Authorize> Authorize { get; set; }
        public virtual DbSet<ProjectInfo> ProjectInfo { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Route> Route { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRelation> UserRelation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Authorize>(entity =>
            {
                entity.ToTable("authorize");

                entity.Property(e => e.AuthorizeId).HasColumnType("varchar(50)");

                entity.Property(e => e.Category).HasColumnType("int(11)");

                entity.Property(e => e.IsHalf).HasColumnType("int(1)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateUserId).HasColumnType("varchar(50)");

                entity.Property(e => e.ItemId).HasColumnType("varchar(50)");

                entity.Property(e => e.ItemType).HasColumnType("int(11)");

                entity.Property(e => e.ObjectId).HasColumnType("varchar(50)");

                entity.Property(e => e.SortCode).HasColumnType("int(11)");
            });

            modelBuilder.Entity<ProjectInfo>(entity =>
            {
                entity.HasKey(e => e.ProjectId);

                entity.ToTable("projectinfo");

                entity.Property(e => e.ProjectId).HasColumnType("varchar(50)");

                entity.Property(e => e.ConstructionUnit).HasColumnType("varchar(50)");

                entity.Property(e => e.Description).HasColumnType("varchar(200)");

                entity.Property(e => e.DesignUnit).HasColumnType("varchar(50)");

                entity.Property(e => e.Identification).HasColumnType("varchar(50)");

                entity.Property(e => e.PrjName).HasColumnType("varchar(50)");

                entity.Property(e => e.OwnerUnit).HasColumnType("varchar(50)");

                entity.Property(e => e.SupervisoryUnit).HasColumnType("varchar(50)");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.SerialNumber).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnType("varchar(50)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateUserId).HasColumnType("varchar(50)");

                entity.Property(e => e.DeleteMark).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("varchar(200)");

                entity.Property(e => e.EnabledMark).HasColumnType("int(11)");

                entity.Property(e => e.FullName).HasColumnType("varchar(50)");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyUserId).HasColumnType("varchar(50)");

                entity.Property(e => e.SortCode).HasColumnType("int(11)");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.ToTable("route");

                entity.Property(e => e.RouteId).HasColumnType("varchar(50)");

                entity.Property(e => e.Description).HasColumnType("varchar(200)");

                entity.Property(e => e.DesignSpeed).HasColumnType("varchar(50)");

                entity.Property(e => e.EndStake).HasColumnType("double(18,4)");

                entity.Property(e => e.ParentId).HasColumnType("varchar(50)");

                entity.Property(e => e.ProjectId).HasColumnType("varchar(50)");

                entity.Property(e => e.RouteLength).HasColumnType("double(18,4)");

                entity.Property(e => e.RouteName).HasColumnType("varchar(50)");

                entity.Property(e => e.RouteType).HasColumnType("int(11)");

                entity.Property(e => e.StartStake).HasColumnType("double(18,4)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateUserId).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId).HasColumnType("varchar(50)");

                entity.Property(e => e.Account).HasColumnType("varchar(50)");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateUserId).HasColumnType("varchar(50)");

                entity.Property(e => e.DeleteMark).HasColumnType("int(11)");

                entity.Property(e => e.Description).HasColumnType("varchar(200)");

                entity.Property(e => e.Email).HasColumnType("varchar(50)");

                entity.Property(e => e.EnabledMark).HasColumnType("int(11)");

                entity.Property(e => e.FirstVisit).HasColumnType("datetime");

                entity.Property(e => e.Gender).HasColumnType("int(11)");

                entity.Property(e => e.HeadIcon).HasColumnType("varchar(200)");

                entity.Property(e => e.LastVisit).HasColumnType("datetime");

                entity.Property(e => e.LogOnCount).HasColumnType("int(11)");

                entity.Property(e => e.ManagerId).HasColumnType("varchar(50)");

                entity.Property(e => e.Mobile).HasColumnType("varchar(50)");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyUserId).HasColumnType("varchar(50)");

                entity.Property(e => e.Password).HasColumnType("varchar(50)");

                entity.Property(e => e.RealName).HasColumnType("varchar(50)");

                entity.Property(e => e.RoleId).HasColumnType("varchar(50)");

                entity.Property(e => e.Secretkey).HasColumnType("varchar(50)");

                entity.Property(e => e.ProvinceId).HasColumnType("varchar(50)");

                entity.Property(e => e.CityId).HasColumnType("varchar(50)");

                entity.Property(e => e.CountyId).HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<UserRelation>(entity =>
            {
                entity.ToTable("userrelation");

                entity.Property(e => e.UserRelationId).HasColumnType("varchar(50)");

                entity.Property(e => e.Category).HasColumnType("int(11)");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.CreateUserId).HasColumnType("varchar(50)");

                entity.Property(e => e.IsDefault).HasColumnType("int(11)");

                entity.Property(e => e.ObjectId).HasColumnType("varchar(50)");

                entity.Property(e => e.SortCode).HasColumnType("int(11)");

                entity.Property(e => e.UserId).HasColumnType("varchar(50)");
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
