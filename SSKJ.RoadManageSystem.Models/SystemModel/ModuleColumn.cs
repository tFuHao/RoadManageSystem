namespace SSKJ.RoadManageSystem.Models.SystemModel
{
    public partial class ModuleColumn
    {
        public string ModuleColumnId { get; set; }
        public string ModuleId { get; set; }
        public string ParentId { get; set; }
        public string FullName { get; set; }
        public int? SortCode { get; set; }
        public string Description { get; set; }
        public string EnCode { get; set; }
        public string Align { get; set; }
        public double? Width { get; set; }
        public int? Target { get; set; }
    }
}
