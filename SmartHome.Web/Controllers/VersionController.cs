using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
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

        public ActionResult Index()
        {
            var viewModel = new VersionListViewModel();
            var versions = _versionService.Queryable();
            return View(new VersionListViewModel(versions));
        }

        #region Create
        public ActionResult Create()
        {

            return View(new VersionManageViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(VersionManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkAsync.BeginTransaction();

                try
                {
                    var application = model.ToDalEntity();
                    application.ObjectState = ObjectState.Added;
                    _versionService.Insert(application);
                    var changes = await _unitOfWorkAsync.SaveChangesAsync();
                    _unitOfWorkAsync.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    // Rollback transaction
                    _unitOfWorkAsync.Rollback();
                }
            }
            return View(model);
        }
        #endregion

        #region Edit
        public async Task<ActionResult> Edit(int VersionId)
        {
            var application = await _versionService.FindAsync(VersionId);

            if (application == null)
            {
                //base.SetErrorMessage("Application with Id [{0}] does not exist", id.ToString());
                return RedirectToAction("Index");
            }
            return View(new VersionManageViewModel(application));
        }

        [HttpPost]
        public async Task<ActionResult> Edit(VersionManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var application = await _versionService.FindAsync(model.VersionId);
                if (application == null) { throw new ArgumentException(string.Format("Application with Id [{0}] does not exist", model.VersionId)); }

                _unitOfWorkAsync.BeginTransaction();

                try
                {
                    model.ToDalEntity(application);
                    application.ObjectState = ObjectState.Modified;
                    _versionService.Update(application);
                    var changes = await _unitOfWorkAsync.SaveChangesAsync();
                    // Commit Transaction
                    _unitOfWorkAsync.Commit();
                    return RedirectToAction("Index");
                }
                catch
                {
                    // Rollback transaction
                    _unitOfWorkAsync.Rollback();
                }
            }
            return View(model);
        }

        #endregion

        #region Delete
        public async Task<ActionResult> Delete(int VersionId)
        {
            var application = await _versionService.FindAsync(VersionId);

            if (application == null)
            {
                // base.SetErrorMessage("Application with Id [{0}] does not exist", id.ToString());
                return RedirectToAction("Index");
            }

            return View(new VersionViewModel(application));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(VersionViewModel model)
        {
            var application = await _versionService.FindAsync(model.VersionId);
            if (application == null) { throw new ArgumentException(string.Format("Application with Id [{0}] does not exist", model.VersionId)); }

            try
            {
                application.ObjectState = ObjectState.Deleted;
                _versionService.Delete(application);
                await _unitOfWorkAsync.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // base.SetErrorMessage("Whoops! Couldn't delete the application. The error was [{0}]", ex.Message);
            }

            return View(model);
        }
        #endregion


    }

}