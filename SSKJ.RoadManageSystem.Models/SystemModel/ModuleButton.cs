﻿namespace SSKJ.RoadManageSystem.Models.SystemModel
{
    public partial class ModuleButton
    {
        public string ModuleButtonId { get; set; }
        public string ModuleId { get; set; }
        public string ParentId { get; set; }
        public string Icon { get; set; }
        public string FullName { get; set; }
        public string ActionAddress { get; set; }
        public int? SortCode { get; set; }
        public string Description { get; set; }
        public string EnCode { get; set; }
    }
}
