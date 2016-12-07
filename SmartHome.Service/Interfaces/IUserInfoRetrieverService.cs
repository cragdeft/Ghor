using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Service.Interfaces
{
  public interface IUserInfoRetrieverService<T>
  {
    T RetriveUserData();
  }
}
