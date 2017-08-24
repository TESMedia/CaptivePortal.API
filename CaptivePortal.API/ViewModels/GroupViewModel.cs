using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CaptivePortal.API.ViewModels
{
    public class GroupViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Rule { get; set; }
    }

    public class GrouplistViewModel
    {
        public GrouplistViewModel()
        {
            GroupViewlist = new List<GroupViewModel>();

        }
        public List<GroupViewModel> GroupViewlist { get; set; }

    }

    public class UserIds
    {
        public int UserId { get; set; }
    }
    public class UserGroup
    {
        public UserGroup()
        {
            UserIdList = new List<UserIds>();
        }
        public int GroupId { get; set; }
        public List<UserIds> UserIdList { get; set; }
    }
}