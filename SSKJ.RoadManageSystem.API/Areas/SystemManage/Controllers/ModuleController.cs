using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSKJ.RoadManageSystem.API.Controllers;
using SSKJ.RoadManageSystem.IBusines.System;
using SSKJ.RoadManageSystem.Models.SystemModel;

namespace SSKJ.RoadManageSystem.API.Areas.SystemManage.Controllers
{
    [Route("api/Module/[action]")]
    [Area("SystemManage")]
    public class ModuleController : BaseController
    {
        private readonly IModuleBusines moduleBll;
        private readonly IButtonBusines buttonBll;
        private readonly IColumnBusines columnBll;

        public ModuleController(IModuleBusines moduleBll, IButtonBusines buttonBll, IColumnBusines columnBll)
        {
            this.moduleBll = moduleBll;
            this.buttonBll = buttonBll;
            this.columnBll = columnBll;
        }

        [HttpGet]
        public async Task<IActionResult> GetModuleTreeGrid(string keyword)
        {
            try
            {
                var a = UserInfo;

                var data = "";
                if (!string.IsNullOrEmpty(keyword))
                    data = await moduleBll.GetTreeListAsync(f => f.FullName.Contains(keyword));
                else
                    data = await moduleBll.GetTreeListAsync(f => true);

                return SuccessData(data);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetButtonTreeGrid(string moduleId)
        {
            var data = "";
            if (!string.IsNullOrEmpty(moduleId))
                data = await buttonBll.GetTreeListAsync(f => f.ModuleId.Equals(moduleId));
            else
                data = await buttonBll.GetTreeListAsync(f => true);

            return SuccessData(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetColumnTreeGrid(string moduleId)
        {
            IEnumerable<ModuleColumn> data = null;
            if (!string.IsNullOrEmpty(moduleId))
                data = await columnBll.GetListAsync(f => f.ModuleId.Equals(moduleId));
            else
                data = await columnBll.GetListAsync();

            return SuccessData(data.OrderBy(o => o.SortCode).ToList());
        }

        [HttpPost]
        public IActionResult ButtonListToTree(List<ModuleButton> list)
        {
            return SuccessData(buttonBll.ButtonListToTree(list));
        }

        [HttpPost]
        public async Task<IActionResult> SaveEntity(Module module, List<ModuleButton> buttons, List<ModuleColumn> columns)
        {
            try
            {
                var result = false;
                var entity = await moduleBll.GetEntityAsync(module.ModuleId);
                if (entity != null)
                {
                    var _buttons = await buttonBll.GetListAsync(b => b.ModuleId == entity.ModuleId);
                    var _columns = await columnBll.GetListAsync(c => c.ModuleId == entity.ModuleId);

                    entity = Utility.Tools.MapperUtils.MapTo<Module, Module>(module);
                    entity.ModifyDate = DateTime.Now;
                    entity.ModifyUserId = UserInfo.UserId;
                    result = await moduleBll.UpdateAsync(entity);

                    if (_buttons.Count() > 0)
                        await buttonBll.DeleteAsync(_buttons);
                    if (_columns.Count() > 0)
                        await columnBll.DeleteAsync(_columns);
                }
                else
                {
                    module.CreateUserId = UserInfo.UserId;
                    module.CreateDate = DateTime.Now;
                    result = await moduleBll.CreateAsync(module);
                }
                if (result)
                {
                    if (buttons.Count > 0)
                    {
                        var result_btn = await buttonBll.CreateAsync(buttons);
                        if (!result_btn)
                        {
                            await moduleBll.DeleteAsync(module);
                            return Fail();
                        }
                    }
                    if (columns.Count > 0)
                    {
                        var result_col = await columnBll.CreateAsync(columns);
                        if (!result_col)
                        {
                            await moduleBll.DeleteAsync(module);
                            await buttonBll.DeleteAsync(buttons);
                            return Fail();
                        }
                    }
                    return SuccessMes();
                }
                return Fail();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string moduleId)
        {
            try
            {
                var list = await moduleBll.GetListAsync(m => m.EnabledMark == 1);
                var modules = GetModules(list.ToList(), moduleId);
                modules.Add(list.Single(m => m.ModuleId == moduleId));

                var buttons = await buttonBll.GetListAsync(b => modules.Any(m => m.ModuleId == b.ModuleId));
                var columns = await columnBll.GetListAsync(c => modules.Any(m => m.ModuleId == c.ModuleId));

                await buttonBll.DeleteAsync(buttons);
                await columnBll.DeleteAsync(columns);
                await moduleBll.DeleteAsync(modules);

                return SuccessMes();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public List<Module> GetModules(List<Module> list, string pId)
        {
            var _list = list.Where(f => f.ParentId == pId).ToList();

            return _list.Concat(_list.SelectMany(t => GetModules(list, t.ModuleId))).ToList();
        }
    }
}