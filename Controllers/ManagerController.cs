using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Commom;
using Newtonsoft.Json;
using PDFHelper.Models;
using static PDFHelper.Models.Request;

namespace PDFHelper.Controllers
{
    public class ManagerController : ApiController
    {
        private IronPdfHelper IronPdf;

        private static string UploadUrl = System.Configuration.ConfigurationManager.AppSettings["FileUploadUrl"];
        //private static string root = AppDomain.CurrentDomain.BaseDirectory;
        private static string root = @"E:\Vincent\SelfGIT储存库\PDFHelper\PDFHelper\";
        [HttpPost]
        public Result<string> ConvertHTMLToPDF([FromBody]OrderPayingToPDFInfo model)
        {
            var temp = $@"{root}Content\PDFHtmlTemp\SaleOrderTemplate.html";
            var Upload = $@"{root}Content\UploadFile\";

            using (StreamReader reader = new StreamReader(temp))
            {
                var html = reader.ReadToEnd();
                var AfterHtml = ReplaceHtml(html, model);
                IronPdf = new IronPdfHelper(AfterHtml, Upload, root);
                var R = IronPdf.ConvertHtmlToPdf();
                return new Result<string> { Success = true, Message = "", Data = R };
            }
        }
        /// <summary>
        /// 替换HTML
        /// </summary>
        /// <param name="TempHtml"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ReplaceHtml(string TempHtml, OrderPayingToPDFInfo model)
        {
            TempHtml = TempHtml.Replace("@Name", model.Order.FullName);
            TempHtml = TempHtml.Replace("@OrderNo", model.Order.Code);
            TempHtml = TempHtml.Replace("@Phone", model.Order.MobileNo);
            TempHtml = TempHtml.Replace("@SignDate", model.Order.Bizdate.ToString());
            TempHtml = TempHtml.Replace("@DeliverAddress", model.Order.AddressInfo);
            //0 未配送；1 部分配送；2 已配送；3 已确认收货
            var DeliverType = string.Empty;
            switch (model.Order.DeliverStatus)
            {
                case 0: DeliverType = "未配送"; break;
                case 1: DeliverType = "部分配送"; break;
                case 2: DeliverType = "已配送"; break;
                case 3: DeliverType = "已确认收货"; break;
                default: DeliverType = "未知状态"; break;
            }
            TempHtml = TempHtml.Replace("@DeliverType", DeliverType);
            TempHtml = TempHtml.Replace("@DeliverDate", model.Order.DeliverDate.ToString());

            var ProListStr = string.Empty;
            for (int i = 0; i < model.OrderProductList.Count(); i++)
            {
                ProListStr += $@"
                     <tr class='HaveBorder'>
                        <td>{i + 1}</td>
                        <td><img src='{UploadUrl + model.OrderProductList[i].Logo}' style='width:100px;' /></td>
                        <td>{model.OrderProductList[i].ProductInfo}</td>
                        <td>{model.OrderProductList[i].Price}</td>
                        <td>{model.OrderProductList[i].Count}</td>
                        <td>{model.OrderProductList[i].Amount}</td>
                        <td>{model.OrderProductList[i].Remark}</td>
                    </tr>";
            }
            TempHtml = TempHtml.Replace("@ProductList", ProListStr);

            TempHtml = TempHtml.Replace("@TotalAmt", model.Order.TotalAmt.ToString());
            TempHtml = TempHtml.Replace("@NetAmt", model.OrderPaying.PayingAmt.ToString());
            TempHtml = TempHtml.Replace("@PaidAmt", model.OrderPaying.PaidAmt.ToString());
            TempHtml = TempHtml.Replace("@Balance", (model.OrderPaying.PayingAmt - model.OrderPaying.PaidAmt).ToString());

            TempHtml = TempHtml.Replace("@Autograph", $"<img src='{UploadUrl + model.OrderPaying.Autograph}' style='width:100px' />");
            TempHtml = TempHtml.Replace("@OrderDate", model.OrderPaying.ActualPayDate.ToString());

            TempHtml = TempHtml.Replace("@BgImg", root);
            TempHtml = TempHtml.Replace("@TopImg", root);
            TempHtml = TempHtml.Replace("@BtnImg", root);
            return TempHtml;
        }
    }
}