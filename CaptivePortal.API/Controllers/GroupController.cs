using CaptivePortal.API.Models;
using CaptivePortal.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CaptivePortal.API.Controllers
{
    public class GroupController : Controller
    {
        Group objGroup = new Group();
        Context.DbContext db = new Context.DbContext();
        // GET: Group
        public ActionResult Index()
        {
            GrouplistViewModel list = new GrouplistViewModel();
            list.GroupViewlist = new List<GroupViewModel>();
            var result = db.Group.ToList();

            var groupDetails = (from item in result
                                select new GroupViewModel()
                                {
                                    GroupName = item.GroupName,
                                    Rule = item.Rule,
                                    GroupId = item.GroupId,
                                }
                             ).ToList();
            list.GroupViewlist.AddRange(groupDetails);
            return View(list);
        }

        public ActionResult CreateGroup(Group model)
        {
            objGroup.GroupName = model.GroupName;
            objGroup.Rule = model.Rule;
            db.Group.Add(objGroup);
            db.SaveChanges();
            return RedirectToAction("Index", "Group");
        }
        [HttpPost]
        public ActionResult DeleteGroup(int GroupId)
        {
            var group = db.Group.Find(GroupId);
            db.Group.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index", "Group");
        }

        [HttpPost]
        public ActionResult UpdateUserGroup(UserGroup groupModel)
        {
            try
            {


                ApplicationUser user = new ApplicationUser();
                for (int i = 0; i < groupModel.UserIdList.Count; i++)
                {
                    user = db.Users.Find(groupModel.UserIdList[i].UserId);
                    user.GroupId = groupModel.GroupId;
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }
    }
}