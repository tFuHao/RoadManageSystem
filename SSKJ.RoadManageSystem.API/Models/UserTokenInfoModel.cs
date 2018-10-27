using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.API.Models
{
    public class UserTokenInfoModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string DataBaseName { get; set; }
        public DateTime? TokenExpiration { get; set; }
    }
}
