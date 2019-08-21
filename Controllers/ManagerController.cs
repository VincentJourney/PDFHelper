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
        private HtmlToPdfHelper pdfHelper;
        private IronPdfHelper IronPdf;

        private static string UploadUrl = System.Configuration.ConfigurationManager.AppSettings["FileUploadUrl"];
        //private static string root = AppDomain.CurrentDomain.BaseDirectory;
        private static string root = @"E:\Vincent\SelfGIT储存库\PDFHelper\PDFHelper\";
        [HttpPost]
        public Result<string> ConvertHTMLToPDF([FromBody]OrderPayingToPDFInfo model)
        {
            var temp = $@"{root}Content\PDFHtmlTemp\SaleOrderTemplate2.html";
            var Upload = $@"{root}Content\UploadFile\";

            using (StreamReader reader = new StreamReader(temp))
            {
                var html = reader.ReadToEnd();
                var AfterHtml = ReplaceHtml(html, model);

                string[] htmlTextArr = Regex.Split(AfterHtml, "##", RegexOptions.IgnoreCase);

                var outurl = $"{Upload}{Guid.NewGuid()}.png";
                string htmlfile = $"{Upload}HtmlTemp/{Guid.NewGuid()}.html";
                CreateHtml.Create(htmlTextArr[0], htmlfile);
                CreateHtml.Append(htmlTextArr[1], htmlfile);
                WebSnapshotsHelper.ConvertStart(htmlfile, outurl, 800, 1000, 800, 1000);

                reader.Close();

                //var outurl = $"{Upload}{Guid.NewGuid()}.png";
                //WebSnapshotsHelper.ConvertStart(htmlfile, outurl, 800, 5000, 800, 5000);

                //IronPdf = new IronPdfHelper(AfterHtml, Upload, root);
                //var R = IronPdf.ConvertHtmlToPdf();

                //由于Spire.Pdf组件生成的时候会在第一页产生水印 插入新页到第一页后删除即可
                //var instance = new CreateHeaderFooter(R.Url);
                //CreateHeaderFooter.Add();
                //var path = CreateHeaderFooter.Remove();

                #region 因为上传到文件服务器的可读性问题。暂时取消上传文件服务器 
                //var base64 = FileHelper.FileToBase64(path);
                //var FileUploadResult = FileUpload($"{UploadUrl}/api/file/UploadFile", base64);
                //var file = JsonConvert.DeserializeObject<ImageResponse>(FileUploadResult);
                #endregion

                return new Result<string> { Success = true, Message = "", Data = "" };
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
            //var UploadUrl = "http://10.0.8.6:1848";
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

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="url"></param>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        private string FileUpload(string url, string base64Str)
        {
            try
            {
                var request = new
                {
                    fileName = Guid.NewGuid().ToString(),
                    base64Str = base64Str,
                    sourceSystem = "crm-ocr",
                    fileDescription = "资源上传图片"
                };
                return HttpHelper.HttpPost(url, JsonConvert.SerializeObject(request));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}