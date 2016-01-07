
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class VersionManageViewModel:VersionViewModel
    {
        public VersionManageViewModel()
            : base()
        {
        }
        public VersionManageViewModel(Version model)
            : base(model)
        {
        }

        public Version ToDalEntity()
        {
            return ToDalEntity(new Version());
        }

        public Version ToDalEntity(Version model)
        {
            model.VersionId = this.VersionId;
            model.Id = this.Id;
            model.AppName = this.AppName;
            model.AppVersion = this.AppVersion;
            model.AuthCode = this.AuthCode;
            model.PassPhrase = this.PassPhrase;
            model.MAC = this.MAC;
            return model;
        }

    }
}