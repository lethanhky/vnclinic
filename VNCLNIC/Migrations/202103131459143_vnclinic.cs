namespace VNCLNIC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vnclinic : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Departments", "Clinic_Id", "dbo.Clinics");
            DropIndex("dbo.Departments", new[] { "Clinic_Id" });
            CreateTable(
                "dbo.DoctorSchedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        MorningWork = c.String(),
                        NoonWork = c.String(),
                        EveningWork = c.String(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.DepartmentClinics",
                c => new
                    {
                        Department_Id = c.Int(nullable: false),
                        Clinic_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Department_Id, t.Clinic_Id })
                .ForeignKey("dbo.Departments", t => t.Department_Id, cascadeDelete: true)
                .ForeignKey("dbo.Clinics", t => t.Clinic_Id, cascadeDelete: true)
                .Index(t => t.Department_Id)
                .Index(t => t.Clinic_Id);
            
            DropColumn("dbo.Departments", "Clinic_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "Clinic_Id", c => c.Int());
            DropForeignKey("dbo.DoctorSchedules", "User_Id", "dbo.Users");
            DropForeignKey("dbo.DepartmentClinics", "Clinic_Id", "dbo.Clinics");
            DropForeignKey("dbo.DepartmentClinics", "Department_Id", "dbo.Departments");
            DropIndex("dbo.DepartmentClinics", new[] { "Clinic_Id" });
            DropIndex("dbo.DepartmentClinics", new[] { "Department_Id" });
            DropIndex("dbo.DoctorSchedules", new[] { "User_Id" });
            DropTable("dbo.DepartmentClinics");
            DropTable("dbo.DoctorSchedules");
            CreateIndex("dbo.Departments", "Clinic_Id");
            AddForeignKey("dbo.Departments", "Clinic_Id", "dbo.Clinics", "Id");
        }
    }
}
