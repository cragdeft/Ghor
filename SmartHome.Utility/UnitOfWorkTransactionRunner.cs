using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Utility
{
    public class UnitOfWorkTransactionRunner : ITransactionRunner
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkTransactionRunner(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void RunTransaction(Action action)
        {
            if (action == null) return;

            _unitOfWork.BeginTransaction();
            try
            {
                action?.Invoke();
                _unitOfWork.SaveChanges();
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public T RunTransaction<T>(Func<T> action)
        {
            if (action == null) return default(T);

            _unitOfWork.BeginTransaction();
            try
            {
                var result = action.Invoke();
                _unitOfWork.SaveChanges();
                _unitOfWork.Commit();
                return result;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public T RunSelectTransaction<T>(Func<T> action)
        {
            if (action == null) return default(T);

            
            try
            {
                var result = action.Invoke();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
