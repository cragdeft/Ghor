using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class VersionViewModel:Version
    {
        public VersionViewModel()
        {
        }

        public VersionViewModel(Version model)
        {
            this.VersionId = model.VersionId;
            this.Id = model.Id;
            this.AppName = model.AppName;
            this.AppVersion = model.AppVersion;
            this.AuthCode = model.AuthCode;
            this.PassPhrase = model.PassPhrase;
            this.MAC = model.MAC;
        }
    }
}