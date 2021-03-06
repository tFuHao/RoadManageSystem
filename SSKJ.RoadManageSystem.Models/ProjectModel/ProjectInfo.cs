﻿using System;

namespace SSKJ.RoadManageSystem.Models.ProjectModel
{
    public partial class ProjectInfo
    {
        public string ProjectId { get; set; }
        public string PrjName { get; set; }
        public string Identification { get; set; }
        public string Description { get; set; }
        public string OwnerUnit { get; set; }
        public string DesignUnit { get; set; }
        public string SupervisoryUnit { get; set; }
        public string ConstructionUnit { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? SerialNumber { get; set; }
    }
}
