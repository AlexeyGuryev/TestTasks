namespace StoragePersistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        RemoveDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoomStateEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomId = c.Int(nullable: false),
                        StateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomEntities", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId);
            
            CreateTable(
                "dbo.FurnitureEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(nullable: false),
                        Count = c.Int(nullable: false),
                        RoomStateEntity_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomStateEntities", t => t.RoomStateEntity_Id)
                .Index(t => t.RoomStateEntity_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomStateEntities", "RoomId", "dbo.RoomEntities");
            DropForeignKey("dbo.FurnitureEntities", "RoomStateEntity_Id", "dbo.RoomStateEntities");
            DropIndex("dbo.FurnitureEntities", new[] { "RoomStateEntity_Id" });
            DropIndex("dbo.RoomStateEntities", new[] { "RoomId" });
            DropTable("dbo.FurnitureEntities");
            DropTable("dbo.RoomStateEntities");
            DropTable("dbo.RoomEntities");
        }
    }
}
