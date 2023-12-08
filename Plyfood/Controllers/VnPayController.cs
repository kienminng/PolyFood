using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto.SendMail;
using Plyfood.Entity;
using Plyfood.Service.IService;
using Plyfood.VnPay;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/VnPay")]
public class VnPayController : Controller
{
    private readonly Helper.VnPay _vnPay;
    private readonly AppDbContext _context;
    private readonly IMailSender _mailSender;

    public VnPayController(Helper.VnPay vnPay,AppDbContext context,IMailSender sender)
    {
        _vnPay = vnPay;
        _context = context;
        _mailSender = sender;
    }

    [HttpPost("VnPay")]
    public IActionResult Pay(long amount , int orderId)
    {
        string vnp_Returnurl = _vnPay.ReturnUrl; //URL nhan ket qua tra ve 
        string vnp_Url = _vnPay.BaseUrl; //URL thanh toan cua VNPAY 
        string vnp_TmnCode = _vnPay.TmnCode; //Ma định danh merchant kết nối (Terminal Id)
        string vnp_HashSecret = _vnPay.HashSecret; //Secret Key

        OrderInfo order = new OrderInfo();
        order.OrderId = orderId;
        order.Amount = amount;
        order.Status = "0";
        order.CreatedDate = DateTime.Now;

        VnPayLibrary vnpay = new VnPayLibrary();
        
        vnpay.AddRequestData("vnp_Version",_vnPay.Version);
        vnpay.AddRequestData("vnp_Command",_vnPay.Command);
        vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        vnpay.AddRequestData("vnp_Amount",(order.Amount*100).ToString());
        vnpay.AddRequestData("vnp_CreateDate",order.CreatedDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode","VND");
        vnpay.AddRequestData("vnp_IpAddr",Utils.GetIpAddress(HttpContext));
        vnpay.AddRequestData("vnp_Locale","vn");
        vnpay.AddRequestData("vnp_OrderInfo","Thanh toán đơn hàng: "+ order.OrderId);
        vnpay.AddRequestData("vnp_OrderType","other");
        vnpay.AddRequestData("vnp_ReturnUrl",vnp_Returnurl);
        vnpay.AddRequestData("vnp_TxnRef",order.OrderId.ToString());
        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        return Ok(paymentUrl);
    }

    [HttpGet("callBack")]
    public IActionResult VnPay_CallBack()
    {
        string vnp_TmnCode = _vnPay.TmnCode; //Ma định danh merchant kết nối (Terminal Id)
        string vnp_HashSecret = _vnPay.HashSecret; //Secret Key
        var vnpayData =  HttpContext.Request.Query;
        VnPayLibrary vnPayLibrary = new VnPayLibrary();
        foreach (var (key, value)  in vnpayData)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnPayLibrary.AddResponseData(key, value);
            }
        }

        long orderId = Int64.Parse(vnPayLibrary.GetResponseData("vnp_TxnRef"));
        string vnp_ResponseCode = vnPayLibrary.GetResponseData("vnp_ResponseCode");
        string vnp_TransactionStatus = vnPayLibrary.GetResponseData("vnp_TransactionStatus");
        String vnp_SecureHash = vnPayLibrary.GetResponseData("vnp_SecureHash");
        long vnp_Amount = Int64.Parse(vnPayLibrary.GetResponseData("vnp_Amount")) / 100;
        bool check = vnPayLibrary.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
        if (check)
        {
            if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
            {
                Console.WriteLine($"Thanh toán giao dich thành công OrderId = {orderId} VNPAY transaction = {vnp_TransactionStatus} ");
                var order = _context.Orders
                    .Include(x=> x.User)
                    .Include(x=> x.OrderDetails)
                    .ThenInclude(x=> x.Product)
                    .FirstOrDefault(x => x.Order_Id == orderId);
                order.Order_Status_Id = 3;
                order.Updated = DateTime.Now;
                _context.Orders.Update(order);
                _context.SaveChanges();
                _mailSender.SendMail(SendMailForm(order.User.Email,order));
                return Ok("Giao dịch thành công");
            }
            else
            {
                return BadRequest($"Lỗi trong khi thực hiện giao dịch Mã lỗi : {vnp_ResponseCode}");
            }
            
        }
        else
        {
            return BadRequest("có lỗi trong quá trình xử lí ");
        }
    }

    private MailForm SendMailForm(string email,Order order)
    {
        Dto.SendMail.MailForm mailForm = new MailForm();
        mailForm.To = email;
        mailForm.Subject = "Hoá Đơn Thanh Toán ";
        
        string OrderItems = "";
        foreach (var s in order.OrderDetails)
        {
            string o = $"<p>Sản phẩm : {s.Product.Name_Product} - Số lượng: {s.Quantity} - Giá: ${s.Price_Total}</p><br>";
            OrderItems += o;
        }
        string orderContent = "<p><strong>Chi tiết đơn hàng:</strong></p>" +
                              $"<p>Mã đơn hàng: {order.Order_Id} {order.Created} </p>" + 
                              OrderItems+
                              $"<p><strong>Tổng cộng:{order.Actual_Price} </strong></p>";
        mailForm.Body = "Nội dung của email<br>" + orderContent;
        return mailForm;
    }
}