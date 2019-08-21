using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFHelper.Models
{
    public class Request
    {
        public class OrderProduct
        {
            public string Logo { get; set; }
            public string ProductInfo { get; set; }
            public decimal Price { get; set; }
            public decimal Count { get; set; }
            public decimal Amount { get; set; }
            public string Remark { get; set; }
        }

        public class OrderPayingToPDFInfo
        {
            public S_Order Order { get; set; }
            public List<OrderProduct> OrderProductList { get; set; }
            public S_OrderPaying OrderPaying { get; set; }
        }
        public class S_Order
        {
            /// <summary>
            /// 
            /// </summary>

            public Guid OrderID { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid OrgOrderID { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public string Code { set; get; }
            /// <summary>
            /// 项目ID
            /// </summary>
            public Guid? MallId { set; get; }
            /// <summary>
            /// 
            /// </summary>
            public Guid StoreID { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime Bizdate { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid StorePersonID { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? IsPromotion { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? PromotionCost { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? TotalAmt { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? NetAmt { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? PaidAmt { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? PayingType { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? PaidStatus { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? DeliverType { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? PhoneFirst { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime? DeliverDate { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? DeliverStatus { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public string FullName { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public string MobileNo { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public string AddressInfo { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? DiscountType { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? DiscountAmt { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public string Remark { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? Enabled { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime AddOn { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid Addby { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime EditOn { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid Editby { set; get; }
            /// <summary>
            /// 新增字段 同步CRM交易的会员ID
            /// </summary>
            public Guid CustomerId { set; get; }
            /// <summary>
            /// 新增字段 同步CRM交易的会员卡号
            /// </summary>
            public string CardCode { set; get; }


            public S_Order()
            {
            }

        }

        public class S_OrderPaying
        {
            /// <summary>
            /// 
            /// </summary>
            public Guid ID { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid OrderID { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public string Code { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public int? PayingType { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? PayingAmt { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? PaidAmt { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? CashDiscount { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime? PayingDate { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? Status { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime? ActualPayDate { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public byte? Enabled { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime AddOn { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid Addby { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public DateTime EditOn { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public Guid Editby { set; get; }

            /// <summary>
            /// 
            /// </summary>
            public int? Flag { set; get; }
            /// <summary>
            ///     
            /// </summary>
            public string Autograph { set; get; }
            /// <summary>
            /// 新增PDF生成
            /// </summary>
            public string FilePath { set; get; }

            public S_OrderPaying()
            {
            }

        }
    }
}