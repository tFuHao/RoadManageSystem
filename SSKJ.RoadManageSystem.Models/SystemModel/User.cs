﻿using System;

namespace SSKJ.RoadManageSystem.Models.SystemModel
{
    public partial class User
    {
        public string UserId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Secretkey { get; set; }
        public string RealName { get; set; }
        public string HeadIcon { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public DateTime? FirstVisit { get; set; }
        public DateTime? LastVisit { get; set; }
        public int? LogOnCount { get; set; }
        public int? DeleteMark { get; set; }
        public int? EnabledMark { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string CountyId { get; set; }
    }
}
