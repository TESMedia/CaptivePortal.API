using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.Context
{
    public class AdminManagementDbOperation
    {
        public void PerformDatabaseOperations()
        {
            try
            {
                using (var db = new CPDBContext())
                {
                    var admin = db.Users.Where(i => i.UserName == "captive@loc8.com").ToList();
                    if (admin.Count != 1)
                    {
                        var user = new Users
                        {
                            UserName = "captive@loc8.com",
                            UserPassword = "Tes@123"
                        };

                        db.Users.Add(user);
                        db.SaveChanges();

                        UserRole objUserRole = new UserRole();
                        var adminInfo = db.Users.Where(i => i.UserName == "captive@loc8.com").FirstOrDefault();
                        objUserRole.UserId = user.UserId;
                        var role = db.Role.Where(i => i.RoleName == "GAdmin").FirstOrDefault();
                        objUserRole.RoleId = role.RoleId;
                        db.UserRole.Add(objUserRole);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}