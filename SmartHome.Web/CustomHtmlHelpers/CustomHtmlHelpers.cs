using SmartHome.Model.Enums;
using SmartHome.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SmartHome.Web.CustomHtmlHelpers
{
    public static class CustomHtmlHelpers
    {
        #region Channel Load
        public static IHtmlString ChannelLoad(this HtmlHelper helper, IEnumerable<SmartHome.Model.Models.Channel> channels, int channelNo)
        {

            var tempChannel = channels.FirstOrDefault(p => p.ChannelNo == channelNo);

            TagBuilder mainRow = new TagBuilder("div");
            if (tempChannel == null)
            {
                var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                string imgUrl = urlHelper.Content("~/Images/device/" + "default_on.png");
                string defaultChannel= "<div><div class=\"row\"><img alt=\"Dimlight\" src="+ imgUrl + " /></div><div class=\"row\"><span class=\"font-size-10 text-grey\"></span></div></div>";
                return new MvcHtmlString(defaultChannel);
            }

            TagBuilder rowPic = new TagBuilder("div");
            rowPic.AddCssClass("row");

            TagBuilder rowMsg = new TagBuilder("div");
            rowMsg.AddCssClass("row");

            TagBuilder pic = new TagBuilder("img");

            TagBuilder msg = new TagBuilder("span");

            LoadChannel(helper, tempChannel, pic, msg);

            rowPic.InnerHtml = pic.ToString(TagRenderMode.SelfClosing);
            rowMsg.InnerHtml = msg.ToString();

            mainRow.InnerHtml = rowPic.ToString();
            mainRow.InnerHtml += rowMsg;


            return new MvcHtmlString(mainRow.ToString());

        }

        

        private static void LoadChannel(HtmlHelper helper, Channel channel, TagBuilder pic, TagBuilder msg)
        {
            string cLoadPicture = string.Empty;
            string cLoadDim = string.Empty;
            string cLoadHardwareDim = string.Empty;
            string cLoadName = channel.LoadName;
            cLoadName = GetLoadName(channel, cLoadName);

            foreach (var nextCStatus in channel.ChannelStatuses)
            {
                switch (nextCStatus.StatusType)
                {
                    case ChannelStatusType.Switchable:
                        cLoadPicture = nextCStatus.StatusValue == 0 ? cLoadName + "_off" : cLoadName + "_on";
                        break;
                    case ChannelStatusType.Dimmable:
                        cLoadDim = nextCStatus.StatusValue == 0 ? nextCStatus.StatusValue.ToString() : nextCStatus.StatusValue.ToString();
                        if (channel.LoadType != LoadType.DimmableBulb)
                        {
                            cLoadDim = cLoadName;
                        }

                        break;
                    case ChannelStatusType.HardwareDimSwitchable:
                        cLoadHardwareDim = nextCStatus.StatusValue == 0 ? nextCStatus.StatusValue.ToString() : nextCStatus.StatusValue.ToString();
                        break;
                }
            }
            //pic.Attributes.Add("src", "/Images/device/" + cLoadPicture + ".png");

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            string imgUrl = urlHelper.Content("~/Images/device/" + cLoadPicture + ".png");

            pic.Attributes.Add("src", imgUrl);
            pic.Attributes.Add("alt", channel.LoadName);
            msg.AddCssClass("font-size-10 text-grey");
            msg.SetInnerText(cLoadDim);
        }

        private static string GetLoadName(Channel channel, string cLoadName)
        {
            switch (channel.LoadType)
            {
                case LoadType.NoLoad:
                    cLoadName = "default";
                    break;
                case LoadType.NonDimmableBulb:
                    cLoadName = "bulb";

                    break;
                case LoadType.DimmableBulb:
                    cLoadName = "dimmable";

                    break;
                case LoadType.Fan:
                    cLoadName = "fan";

                    break;
                case LoadType.Tubelight:
                    cLoadName = "tube";

                    break;
                case LoadType.Cfl:
                    cLoadName = "cfl";
                    break;
            }

            return cLoadName;
        }
        #endregion


        #region Remove

        public static IHtmlString SmartSwitch6g(this HtmlHelper helper, SmartDevice device)
        {
            StringBuilder sb = new StringBuilder();
            //string data = "shamim<br/>123";
            sb.Append("<div class=\"col-sm-6 col-centered switch-panel-height\">");
            sb.Append("<div class=\"panel panel-default\">");
            SwitchPanelHeader(sb, device.DeviceName);
            SwitchPanelBody(sb, device);
            sb.Append("");
            sb.Append("</div>");
            sb.Append("</div>");



            return new MvcHtmlString(sb.ToString());
        }

        private static void SwitchPanelBody(StringBuilder sb, SmartDevice device)
        {
            sb.Append("<div class=\"panel-body switch-body\">");
            sb.Append("<div class=\"text-center\">");
            sb.Append("<div class=\"container margin-20 padding-20\">");


            SwitchColumns(sb, device);


            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
        }

        private static void SwitchColumns(StringBuilder sb, SmartDevice device)
        {
            string columnContent = string.Empty;
            //foreach (var nextChannel in Enumerable.Range(1, 4))
            foreach (var nextChannel in ((SmartSwitch) device).Channels.OrderBy(p => p.ChannelNo))
            {

                //var tempChannel = device.Channels.FirstOrDefault(p => p.ChannelNo == nextChannel);

                switch (nextChannel.LoadType)
                {
                    case LoadType.NoLoad:
                        columnContent = LoadChannel(nextChannel, "default");
                        break;
                    case LoadType.NonDimmableBulb:
                        columnContent = LoadChannel(nextChannel, "bulb");
                        break;
                    case LoadType.DimmableBulb:
                        columnContent = LoadChannel(nextChannel, "dimmable");
                        break;
                    case LoadType.Fan:
                        columnContent = LoadChannel(nextChannel, "fan");
                        break;
                    case LoadType.Tubelight:
                        columnContent = LoadChannel(nextChannel, "tube");
                        break;
                    case LoadType.Cfl:
                        columnContent = LoadChannel(nextChannel, "cfl");
                        break;

                }


                switch (nextChannel.ChannelNo)
                {
                    case 1:
                        sb.Append("<div class=\"row\">");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append(columnContent);
                        sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");

                        SwitchIndicator(sb);

                        sb.Append("</div>");
                        sb.Append("</div>");
                        break;

                    case 2:
                        sb.Append("<div class=\"row\">");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append(columnContent);
                        sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append("</div>");
                        sb.Append("</div>");
                        break;


                    case 3:
                        sb.Append("<div class=\"row\">");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append(columnContent);
                        sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append("</div>");
                        sb.Append("</div>");
                        break;

                    case 4:
                        sb.Append("<div class=\"row\">");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append(columnContent);
                        sb.Append("</div>");

                        //sb.Append("<div class=\"col-sm-4\">");
                        //sb.Append("</div>");

                        //sb.Append("<div class=\"col-sm-4\">");
                        //sb.Append("</div>");
                        //sb.Append("</div>");
                        break;

                    case 5:
                        //sb.Append("<div class=\"row\">");

                        //sb.Append("<div class=\"col-sm-4\">");

                        //sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append(columnContent);
                        sb.Append("</div>");

                        //sb.Append("<div class=\"col-sm-4\">");
                        //sb.Append("</div>");
                        //sb.Append("</div>");
                        break;


                    case 6:
                        //sb.Append("<div class=\"row\">");

                        //sb.Append("<div class=\"col-sm-4\">");

                        //sb.Append("</div>");

                        //sb.Append("<div class=\"col-sm-4\">");
                        //sb.Append(columnContent);
                        //sb.Append("</div>");

                        sb.Append("<div class=\"col-sm-4\">");
                        sb.Append(columnContent);
                        sb.Append("</div>");
                        sb.Append("</div>");
                        break;
                }


            }


        }

        private static void SwitchIndicator(StringBuilder sb)
        {
            sb.Append("<div class=\"row\">");
            sb.Append("<img src=\"/Images/device/indicator_on.png\" alt=\"indicator on\" />");
            sb.Append("</div>");
            sb.Append("<div class=\"row\">");
            sb.Append("<span class=\"font-size-10 text-grey\">indicator</span>");
            sb.Append("</div>");
        }





        private static string LoadChannel(Channel channel, string channelLoad)
        {
            StringBuilder sbColumn = new StringBuilder();
            string cLoadPicture = string.Empty;
            string cLoadDim = string.Empty;
            string cLoadHardwareDim = string.Empty;
            foreach (var nextCStatus in channel.ChannelStatuses)
            {
                switch (nextCStatus.StatusType)
                {
                    case ChannelStatusType.Switchable:
                        cLoadPicture = nextCStatus.StatusValue == 0 ? channelLoad + "_off" : channelLoad + "_on";
                        break;
                    case ChannelStatusType.Dimmable:
                        cLoadDim = nextCStatus.StatusValue == 0 ? nextCStatus.StatusValue.ToString() : nextCStatus.StatusValue.ToString();
                        if (channel.LoadType != LoadType.DimmableBulb)
                        {
                            cLoadDim = channel.LoadName;
                        }

                        break;
                    case ChannelStatusType.HardwareDimSwitchable:
                        cLoadHardwareDim = nextCStatus.StatusValue == 0 ? nextCStatus.StatusValue.ToString() : nextCStatus.StatusValue.ToString();
                        break;
                }
            }


            //sbColumn.Append("<div class=\"col-sm-4\">");
            sbColumn.Append("<div class=\"row\">");
            sbColumn.Append("<img src=\"/Images/device/" + cLoadPicture + ".png\" alt=\"" + channelLoad + "\" />");
            sbColumn.Append("</div>");
            sbColumn.Append("<div class=\"row\">");
            sbColumn.Append("<span class=\"font-size-10 text-grey\">" + cLoadDim + "</span>");
            sbColumn.Append("</div>");
            //sbColumn.Append("</div>");
            return sbColumn.ToString();
        }

        private static void SwitchPanelHeader(StringBuilder sb, string deviceName)
        {
            sb.Append("<div class=\"panel-heading\">");
            sb.Append("<div class=\"row\">");
            sb.Append("<div class=\"col-sm-6\">");
            sb.Append("<h4 class=\"panel-title panel-header-label-color\"><strong><span>" + deviceName + "</span></strong></h4>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
        } 
        #endregion
    }
}