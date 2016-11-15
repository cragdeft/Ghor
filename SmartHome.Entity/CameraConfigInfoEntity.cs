using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Entity
{
  public class CameraConfigInfoEntity
  {
    [JsonProperty("Id")]
    public int AppsCameraConfigInfoId { get; set; }
    [JsonProperty("Ip")]
    public string CameraIp { get; set; }
    [JsonProperty("Port")]
    public int CameraPort { get; set; }
    [JsonProperty("UserName")]
    public string CameraUsername { get; set; }
    [JsonProperty("Password")]
    public string CameraPassword { get; set; }
    public int IsSynced { get; set; }
  }
}
