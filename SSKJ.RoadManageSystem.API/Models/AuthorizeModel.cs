﻿using SSKJ.RoadManageSystem.Models.SystemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSKJ.RoadManageSystem.API.Models
{
    public class AuthorizeModel
    {
        public UserInfoModel UserInfo { get; set; }
        public IEnumerable<Module> ModuleAuthorizes { get; set; }
        public IEnumerable<ModuleButton> ButtonAuthorizes { get; set; }
        public IEnumerable<ModuleColumn> ColumnAuthorizes { get; set; }
        public string RouteAuthorizes { get; set; }
    }
}
