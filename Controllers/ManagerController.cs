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
        private static string SealImgUrl = System.Configuration.ConfigurationManager.AppSettings["SealImgUrl"];
        private static string root = AppDomain.CurrentDomain.BaseDirectory;
        //private static string root = @"E:\Vincent\SelfGit储存库\PDFHelper\PDFHelper\";
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
            TempHtml = TempHtml.Replace("@Phone", model.Order.MobileNo);
            TempHtml = TempHtml.Replace("@OrderNo", model.Order.Code);

            TempHtml = TempHtml.Replace("@DeliverDate", model.Order.DeliverDate.Value.ToString("yyyy-MM-dd"));
            TempHtml = TempHtml.Replace("@DeliverAddress", model.Order.AddressInfo);
            TempHtml = TempHtml.Replace("@SignDate", model.Order.Bizdate.ToString("yyyy-MM-dd hh:mm:ss"));

            TempHtml = TempHtml.Replace("@BrandName", model.StoreCustomerInfo?.BrandName ?? "");
            TempHtml = TempHtml.Replace("@StoreCode", model.StoreCustomerInfo?.StoreCode ?? "");
            TempHtml = TempHtml.Replace("@CompanyPhone", model.StoreCustomerInfo?.ContactNumber ?? "");

            #region 商品
            var ProListStr = string.Empty;
            foreach (var item in model.OrderProductList)
            {
                ProListStr += $@"
                     <tr class='HaveBorder'>
                        <td>{item.ProductName}</td>
                        <td>{item.ProductTypeName}</td>
                        <td>{item.Specifications}</td>
                        <td>&nbsp;</td>
                        <td>{item.UnitOfMeasure}</td>
                        <td>{item.Price}</td>
                        <td>{item.Count}</td>
                        <td>{item.Amount}</td>
                        <td>{item.Remark}</td>
                    </tr>";
            }

            TempHtml = TempHtml.Replace("@ProductList", ProListStr);
            #endregion

            #region 支付方式
            bool xjcheck = model.PayType.Contains("现金") ? true : false;
            bool skcheck = model.PayType.Contains("卡") ? true : false;
            bool ydcheck = model.PayType.Contains("移动支付") ? true : false;
            bool qtcheck = false;
            var paytypeArr = model.PayType.Split(',');
            foreach (var item in paytypeArr)
            {
                if (!item.Contains("现金") && !item.Contains("卡") && !item.Contains("移动支付"))
                    qtcheck = true;
            }
            var bingoHtml = "<div class='bingo' style='float:left'>√</div>";
            var nobingoHtml = "<div class='bingo' style='float:left'></div>";
            var PayTypeHtml = $@" 
                        <div class='divpaytype'> {(xjcheck ? bingoHtml : nobingoHtml)} <div style='float:left'>现金</div></div>
                        <div class='divpaytype'> {(skcheck ? bingoHtml : nobingoHtml)} <div style='float:left'>刷卡</div></div>
                        <div class='divpaytype'> {(ydcheck ? bingoHtml : nobingoHtml)} <div style='float:left'>移动支付</div></div>
                        <div class='divpaytype'> {(qtcheck ? bingoHtml : nobingoHtml)} <div style='float:left'>其他</div></div>";
            TempHtml = TempHtml.Replace("@PayType", PayTypeHtml);
            #endregion


            var TotalAmt = model.Order.TotalAmt.ToString();
            var ChineseTotalAmt = Util.MoneyToChinese(TotalAmt);
            if (ChineseTotalAmt.IndexOf("圆") == 0)
                ChineseTotalAmt = " 零" + ChineseTotalAmt;
            if (ChineseTotalAmt.IndexOf("拾") == -1)
                ChineseTotalAmt = " 零拾" + ChineseTotalAmt;
            if (ChineseTotalAmt.IndexOf("佰") == -1)
                ChineseTotalAmt = " 零佰" + ChineseTotalAmt;
            if (ChineseTotalAmt.IndexOf("仟") == -1)
                ChineseTotalAmt = " 零仟" + ChineseTotalAmt;
            if (ChineseTotalAmt.IndexOf("万") == -1)
                ChineseTotalAmt = " 零万" + ChineseTotalAmt;
            //捌拾叁万,肆仟,玖佰,叁拾,柒圆壹角整,
            TempHtml = TempHtml.Replace("@ChineseTotalAmt", ChineseTotalAmt);
            TempHtml = TempHtml.Replace("@TotalAmt", model.Order.TotalAmt.ToString());
            TempHtml = TempHtml.Replace("@OrderRemark", model.Order.Remark);

            TempHtml = TempHtml.Replace("@DownPayment", model.DownPayment.ToString()); //首款
            TempHtml = TempHtml.Replace("@PaidAmt", model.OrderPaying.PaidAmt.ToString());//本次付款
            var Balance = 0M;
            if (model.OrderPaying.PayingType == 3)//如果此订单是首款
                Balance = (model.Order.TotalAmt ?? 0) - model.DownPayment;
            else if (model.OrderPaying.PayingType == 1)//如果此订单是尾款
                Balance = (model.Order.TotalAmt ?? 0) - model.DownPayment - (model.OrderPaying.PaidAmt ?? 0);

            TempHtml = TempHtml.Replace("@Balance", Balance.ToString());//欠款

            TempHtml = TempHtml.Replace("@Cashier", model.CashierName.ToString()); //收银员
            TempHtml = TempHtml.Replace("@Autograph", $"<img src='{UploadUrl + model.OrderPaying.Autograph}' style='width:100px' />");

            TempHtml = TempHtml.Replace("@seal", $"<img src='{UploadUrl + SealImgUrl}' style='width: 176px;height: 140px;'/>");
            return TempHtml;
        }
    }
}