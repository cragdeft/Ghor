using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using SmartHome.Entity;
using SmartHome.Model.ModelDataContext;
using SmartHome.Model.Models;
using SmartHome.Service;
using SmartHome.Service.Interfaces;
using SmartHome.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.Controllers
{
    public class VersionController : Controller
    {
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        private readonly IVersionService _versionService;
        public VersionController(IUnitOfWorkAsync unitOfWorkAsync, IVersionService versionService)
        {
            this._unitOfWorkAsync = unitOfWorkAsync;
            this._versionService = versionService;
        }

        public async Task<ActionResult> Index()
        {
            var versions = await _versionService.GetsAsync();
            return View(versions.ToList());
        }

        public ActionResult Index2()
        {
            var versions = _versionService.Queryable();
            return View(versions.ToList());
        }

        #region Create
        public ActionResult Create()
        {
            return View(new VersionEntity());
        }

        [HttpPost]
        public async  Task<ActionResult> Create(VersionEntity entity)
        {
            if (ModelState.IsValid)
            {
                #region MyRegion
                //_unitOfWorkAsync.BeginTransaction();

                //try
                //{
                //    _versionService.Add(entity);
                //    var changes = await _unitOfWorkAsync.SaveChangesAsync();
                //    _unitOfWorkAsync.Commit();
                //    return RedirectToAction("Index");
                //}
                //catch (Exception ex)
                //{
                //    _unitOfWorkAsync.Rollback();
                //} 
                #endregion


                #region MyRegion
                using (IDataContextAsync context = new SmartHomeDataContext())
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    IRepositoryAsync<SmartHome.Model.Models.Version> versionRepository = new Repository<SmartHome.Model.Models.Version>(context, unitOfWork);
                    IVersionService versionService = new VersionService(versionRepository);

                    try
                    {
                        unitOfWork.BeginTransaction();


                        //versionService.Add(entity);
                        versionService.Add(entity);
                        var changes =await unitOfWork.SaveChangesAsync();
                        unitOfWork.Commit();
                        return RedirectToAction("Index");




                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                    }
                }
                #endregion
            }
            return View(entity);
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit(int VersionId)
        {
            var application = await _versionService.GetAsync(VersionId);

            if (application == null)
            {
                return RedirectToAction("Index");
            }
            return View(application);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(VersionEntity entity)
        {
            if (ModelState.IsValid)
            {
               
                _unitOfWorkAsync.BeginTransaction();

                try
                {
                    _versionService.Modify(entity);
                    var changes = await _unitOfWorkAsync.SaveChangesAsync();
                    _unitOfWorkAsync.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    _unitOfWorkAsync.Rollback();
                }
            }
            return View(entity);
        }

        #endregion

        #region Delete
        public async Task<ActionResult> Delete(int VersionId)
        {
            var entity = await _versionService.GetAsync(VersionId);
            if (entity == null)
            {
                return RedirectToAction("Index");
            }
            return View(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(VersionEntity entity)
        {



            try
            {
                _versionService.Remove(entity);
                await _unitOfWorkAsync.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // base.SetErrorMessage("Whoops! Couldn't delete the application. The error was [{0}]", ex.Message);
            }

            return View(entity);
        }
        #endregion


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _unitOfWorkAsync.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

    }

}