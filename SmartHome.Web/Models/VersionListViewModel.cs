
using SmartHome.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartHome.Web.Models
{
    public class VersionListViewModel
    {
        public VersionListViewModel()
        {
            this.Versions = new List<VersionViewModel>();
        }


        public VersionListViewModel(IEnumerable<Version> version)
        {
            this.Versions = new List<VersionViewModel>();

            foreach (var modelItem in version)
            {
                this.Versions.Add(new VersionViewModel(modelItem));
            }
        }

        public List<VersionViewModel> Versions { get; set; }
    }
}