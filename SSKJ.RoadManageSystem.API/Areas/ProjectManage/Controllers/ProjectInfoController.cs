using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSKJ.RoadManageSystem.API.Controllers;
using SSKJ.RoadManageSystem.IBusines.Project;
using SSKJ.RoadManageSystem.IBusines.System;
using SSKJ.RoadManageSystem.Models.SystemModel;

namespace SSKJ.RoadManageSystem.API.Areas.ProjectManage.Controllers
{
    [Route("api/ProjectInfo/[action]")]
    [Area("ProjectManage")]
    public class ProjectInfoController : BaseController
    {
        private readonly IUserProjectBusines userProjectBll;
        private readonly IProjectInfoBusines prjInfoBll;
        private readonly IBusines.Project.IUserBusines userBll;

        public ProjectInfoController(IUserProjectBusines userProjectBll, IProjectInfoBusines prjInfoBll, IBusines.Project.IUserBusines userBll)
        {
            this.userProjectBll = userProjectBll;
            this.prjInfoBll = prjInfoBll;
            this.userBll = userBll;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(int pageSize, int pageIndex)
        {
            try
            {
                var userInfo = UserInfo;

                if (userInfo.TokenExpiration <= DateTime.Now)
                    return BadRequest(new { message = "登录超时，请重新登录!" });

                var result = new Tuple<IEnumerable<UserProject>, int>(null, 0);

                if (userInfo.RoleId == "PrjManager")
                    result = await userProjectBll.GetListAsync(e => e.UserId == userInfo.UserId, e => e.SerialNumber, true, pageSize, pageIndex);
                else if (userInfo.RoleId == "System")
                    result = new Tuple<IEnumerable<UserProject>, int>(null, 0);
                else
                {
                    var entity = await userProjectBll.GetListAsync(e => e.PrjDataBase == userInfo.DataBaseName);
                    result = new Tuple<IEnumerable<UserProject>, int>(entity, entity.Count());
                }

                return Success(new
                {
                    data = result.Item1,
                    count = result.Item2
                });
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(Models.ProjectInfoModel entity)
        {
            var dbName = "";
            var upId = "";
            try
            {
                var userInfo = UserInfo;

                if (userInfo.RoleId != "PrjManager")
                    return Fail("权限不足，操作失败!");

                if (!string.IsNullOrEmpty(entity.PrjInfo.ProjectId))
                {
                    var up = await userProjectBll.GetEntityAsync(u => u.ProjectId == entity.PrjInfo.ProjectId);
                    entity.PrjInfo.ModifyDate = DateTime.Now;
                    var result = await prjInfoBll.UpdateAsync(entity.PrjInfo, up.PrjDataBase);


                    up.PrjName = entity.PrjInfo.PrjName;
                    up.Description = entity.PrjInfo.Description;
                    up.DesignUnit = entity.PrjInfo.DesignUnit;
                    up.OwnerUnit = entity.PrjInfo.OwnerUnit;
                    up.SupervisoryUnit = entity.PrjInfo.SupervisoryUnit;
                    up.ConstructionUnit = entity.PrjInfo.ConstructionUnit;
                    up.ModifyDate = DateTime.Now;
                    await userProjectBll.UpdateAsync(up);

                    if (result)
                        return Success();
                    return Fail();
                }
                else
                {
                    var projects = await userProjectBll.GetListAsync();
                    int? prjSerialNumber = 0;
                    if (projects.Count() > 0) prjSerialNumber = projects.Select(p => p.SerialNumber).Max() + 1;
                    else prjSerialNumber++;

                    dbName = "road_project_00" + prjSerialNumber;
                    var db = await Utility.Tools.DataBaseUtils.CreateDataBase(dbName);
                    if (!db) return Fail("初始化数据库失败!");

                    entity.PrjInfo.ProjectId = Guid.NewGuid().ToString();
                    entity.PrjInfo.SerialNumber = prjSerialNumber;
                    var result = await prjInfoBll.CreateAsync(entity.PrjInfo, dbName);

                    var resultU = false;
                    if (string.IsNullOrEmpty(entity.UserInfo.UserId))
                    {
                        entity.UserInfo.UserId = Guid.NewGuid().ToString();
                        entity.UserInfo.Secretkey = Guid.NewGuid().ToString();
                        entity.UserInfo.Password = Utility.Tools.MD5Utils.Sign(entity.UserInfo.Password, entity.UserInfo.Secretkey);
                        entity.UserInfo.CreateDate = DateTime.Now;
                        entity.UserInfo.CreateUserId = userInfo.UserId;
                        entity.UserInfo.RoleId = "PrjAdmin";

                        resultU = await userBll.CreateAsync(entity.UserInfo, dbName);
                    }

                    if (!result && !resultU)
                    {
                        await Utility.Tools.DataBaseUtils.DeleteDataBase(dbName);
                        return Fail();
                    }

                    var userProject = new UserProject
                    {
                        UserPrjId = Guid.NewGuid().ToString(),
                        UserId = userInfo.UserId,
                        ProjectId = entity.PrjInfo.ProjectId,
                        SerialNumber = prjSerialNumber,
                        PrjIdentification = "",
                        PrjDataBase = dbName,
                        PrjName = entity.PrjInfo.PrjName,
                        Description = entity.PrjInfo.Description,
                        OwnerUnit = entity.PrjInfo.OwnerUnit,
                        DesignUnit = entity.PrjInfo.DesignUnit,
                        SupervisoryUnit = entity.PrjInfo.SupervisoryUnit,
                        ConstructionUnit = entity.PrjInfo.ConstructionUnit,
                        ModifyDate = DateTime.Now
                    };
                    var resultUP = await userProjectBll.CreateAsync(userProject);
                    upId = userProject.UserPrjId;
                    if (!resultUP)
                    {
                        var a = await prjInfoBll.DeleteAsync(entity.PrjInfo, dbName);
                        if (a) await Utility.Tools.DataBaseUtils.DeleteDataBase(dbName);
                    }
                    return Success();
                }
            }
            catch (Exception ex)
            {
                await Utility.Tools.DataBaseUtils.DeleteDataBase(dbName);
                if (!string.IsNullOrEmpty(upId)) await userProjectBll.DeleteAsync(upId);

                return Error(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(List<UserProject> list)
        {
            try
            {
                var userInfo = UserInfo;

                list.ForEach(async p =>
                {
                    await Utility.Tools.DataBaseUtils.DeleteDataBase(p.PrjDataBase);
                });
                var result = await userProjectBll.DeleteAsync(list);
                if (result)
                    return Success();
                return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
    }
}