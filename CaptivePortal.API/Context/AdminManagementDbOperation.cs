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
                            Password = "Tes@123",
                            CreationDate = DateTime.Now,
                            UpdateDate=DateTime.Now
                        };

                        List<Age> listAge = new List<Age>()
                        {
                             new Age { Value = "0-17" }, new Age { Value = "18-24" }, new Age { Value = "25-43" }, new Age { Value = "35-44" }, new Age { Value = "45-54" }, new Age { Value = "55-64" }, new Age { Value = "65++" }
                        };

                        db.Age.AddRange(listAge);

                        List<Gender> listGender = new List<Gender>()
                        {
                            new Gender { Value="Male"},new Gender {Value="Female" },new Gender { Value="Not Answered"} 
                        };
                        db.Gender.AddRange(listGender);
                        

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