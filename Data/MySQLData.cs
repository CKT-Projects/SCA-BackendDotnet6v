//using scabackend.Models;
//using Microsoft.EntityFrameworkCore;

//namespace scabackend.Data
//{
//    public class MySQLData : DbContext
//    {
//        public MySQLData (DbContextOptions<MySQLData> options) : base(options)
//        {}

//        public DbSet<UserDataModel> users { get; set; }
//    }
//}


//UserDataModel newUser = new UserDataModel();
//newUser.public_uuid = Guid.NewGuid().ToString();
//newUser.username = "a";
//newUser.email = "kpa.ph@aol.com";
//newUser.mobile = "123456";
//newUser.password = "122121";
//newUser.hint = "122121";
//newUser.firstname = "KIng";
//newUser.middlename = "Pau";
//newUser.lastname = "asa";
//newUser.role = 1;
//newUser.worker_of = 0;
//newUser.is_active = 1;
//newUser.created_at = DateTime.Now;
//newUser.updated_at = DateTime.Now;

//this._mySQLData.users.Add(newUser);
//this._mySQLData.SaveChanges();
