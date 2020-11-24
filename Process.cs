using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commercc.Transaction.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Web;

public class Process
{
    public stcTransactionDetailSet ProcessTransaction(stcTransactionDetailSet sT)
    {
        try
        {
            var iR = new responseNMI();

            switch (sT.transaction.paymentStatusId)
            {
                case enTransactionStatusList.Authorized:
                    {
                        iR = AuthCaptureTransaction(sT);
                        break;
                    }
                case enTransactionStatusList.AuthCaptured:
                    {
                        iR = AuthCaptureTransaction(sT);
                        break;
                    }
                case enTransactionStatusList.Captured:
                    {
                        iR = CaptureTransaction(sT);
                        break;
                    }
                case enTransactionStatusList.Voided:
                    {
                        iR = VoidTransaction(sT);
                        break;
                    }
                case enTransactionStatusList.Refunded:
                    {
                        iR = RefundTransaction(sT);
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            string message = ex.Message.ToString();
            // This is the ONLY Point where it is indicated that RESPONSE has ERROR

            sT.transaction.response.responseErrorCode = new stcErrorCodeSet() { errorCode = enErrorCodeList.Transaction_Process_General_Error };
        }

        return sT;
    }

    private responseNMI AuthCaptureTransaction(stcTransactionDetailSet sT)
    {
        var strUrl = new StringBuilder();
        strUrl.AppendFormat("{0}", sT.merchant.processor.gateway.getAPIUrl);
        strUrl.AppendFormat("?security_key={0}", sT.merchant.processor.gateway.apiSecret);
        strUrl.AppendFormat("&payment={0}", sT.customer.payment.paymentModeId);
        strUrl.AppendFormat("&type={0}", "auth");
        strUrl.AppendFormat("&amount={0}", sT.cart.cartAmount);
        strUrl.AppendFormat("&ccnumber={0}", sT.customer.payment.card.cardNumber);
        strUrl.AppendFormat("&ccexp={0}", sT.customer.payment.card.cardExpiration);
        strUrl.AppendFormat("&cvv={0}", sT.customer.payment.card.cardCVV);
        if (!string.IsNullOrEmpty(sT.customer.payment.eCheck.bankAccountName))
            strUrl.AppendFormat("&checkname={0}", sT.customer.payment.eCheck.bankAccountName);
        if (!string.IsNullOrEmpty(sT.customer.payment.eCheck.bankABA))
            strUrl.AppendFormat("&checkaba={0}", sT.customer.payment.eCheck.bankABA);
        if (!string.IsNullOrEmpty(sT.customer.payment.eCheck.bankAccountNumber))
            strUrl.AppendFormat("&checkaccount={0}", sT.customer.payment.eCheck.bankAccountNumber);
        if (!string.IsNullOrEmpty(sT.customer.payment.eCheck.bankECheckType))
            strUrl.AppendFormat("&account_holder_type={0}", sT.customer.payment.eCheck.bankECheckType);
        if (!string.IsNullOrEmpty(sT.customer.payment.eCheck.bankAccountType))
            strUrl.AppendFormat("&account_type={0}", sT.customer.payment.eCheck.bankAccountType);
        if (!string.IsNullOrEmpty(sT.cart.currency.currency))
            strUrl.AppendFormat("&currency={0}", sT.cart.currency.currency);
        if (!string.IsNullOrEmpty(sT.customer.ipAddress))
            strUrl.AppendFormat("&ipaddress={0}", sT.customer.ipAddress);
        if (!string.IsNullOrEmpty(sT.customer.billing.firstName))
            strUrl.AppendFormat("&first_name={0}", sT.customer.billing.firstName);
        if (!string.IsNullOrEmpty(sT.customer.billing.lastName))
            strUrl.AppendFormat("&last_name={0}", sT.customer.billing.lastName);
        if (!string.IsNullOrEmpty(sT.customer.billing.companyName))
            strUrl.AppendFormat("&company={0}", sT.customer.billing.companyName);
        if (!string.IsNullOrEmpty(sT.customer.billing.address))
            strUrl.AppendFormat("&address1={0}", sT.customer.billing.address);
        if (!string.IsNullOrEmpty(sT.customer.billing.city))
            strUrl.AppendFormat("&city={0}", sT.customer.billing.city);
        if (!string.IsNullOrEmpty(sT.customer.billing.state))
            strUrl.AppendFormat("&state={0}", sT.customer.billing.state);
        if (!string.IsNullOrEmpty(sT.customer.billing.zipcode))
            strUrl.AppendFormat("&zip={0}", sT.customer.billing.zipcode);
        if (!string.IsNullOrEmpty(sT.customer.billing.country.country))
            strUrl.AppendFormat("&country={0}", sT.customer.billing.country.country);
        if (!string.IsNullOrEmpty(sT.customer.billing.phone))
            strUrl.AppendFormat("&phone={0}", sT.customer.billing.phone);
        if (!string.IsNullOrEmpty(sT.customer.billing.fax))
            strUrl.AppendFormat("&fax={0}", sT.customer.billing.fax);
        if (!string.IsNullOrEmpty(sT.customer.billing.email))
            strUrl.AppendFormat("&email={0}", sT.customer.billing.email);
        if (!string.IsNullOrEmpty(sT.customer.shipping.firstName))
            strUrl.AppendFormat("&shipping_firstname={0}", sT.customer.shipping.firstName);
        if (!string.IsNullOrEmpty(sT.customer.shipping.lastName))
            strUrl.AppendFormat("&shipping_lastname={0}", sT.customer.shipping.lastName);
        if (!string.IsNullOrEmpty(sT.customer.shipping.companyName))
            strUrl.AppendFormat("&shipping_company={0}", sT.customer.shipping.companyName);
        if (!string.IsNullOrEmpty(sT.customer.shipping.address))
            strUrl.AppendFormat("&shipping_address1={0}", sT.customer.shipping.address);
        if (!string.IsNullOrEmpty(sT.customer.shipping.city))
            strUrl.AppendFormat("&shipping_city={0}", sT.customer.shipping.city);
        if (!string.IsNullOrEmpty(sT.customer.shipping.state))
            strUrl.AppendFormat("&shipping_state={0}", sT.customer.shipping.state);
        if (!string.IsNullOrEmpty(sT.customer.shipping.zipcode))
            strUrl.AppendFormat("&shipping_zip={0}", sT.customer.shipping.zipcode);
        if (!string.IsNullOrEmpty(sT.customer.shipping.country.country))
            strUrl.AppendFormat("&shipping_country={0}", sT.customer.shipping.country.country);
        if (!string.IsNullOrEmpty(sT.customer.shipping.email))
            strUrl.AppendFormat("&shipping_email={0}", sT.customer.shipping.email);
        return ExecuteTransaction(strUrl.ToString(), sT);
    }

    private responseNMI CaptureTransaction(stcTransactionDetailSet sT)
    {
        var strUrl = new StringBuilder();
        strUrl.AppendFormat("{0}", sT.merchant.processor.gateway.getAPIUrl);
        strUrl.AppendFormat("?type={0}", "capture");
        strUrl.AppendFormat("&security_key={0}", sT.merchant.processor.gateway.apiSecret);
        strUrl.AppendFormat("&transactionid={0}", sT.transaction.transactionId);
        strUrl.AppendFormat("&amount={0}", sT.cart.cartAmount);
        return ExecuteTransaction(strUrl.ToString(), sT);
    }

    private responseNMI VoidTransaction(stcTransactionDetailSet sT)
    {
        var strUrl = new StringBuilder();
        strUrl.AppendFormat("{0}", sT.merchant.processor.gateway.getAPIUrl);
        strUrl.AppendFormat("?type={0}", "void");
        strUrl.AppendFormat("&security_key={0}", sT.merchant.processor.gateway.apiSecret);
        strUrl.AppendFormat("&transactionid={0}", sT.transaction.transactionId);
        strUrl.AppendFormat("&payment={0}", sT.customer.payment.paymentModeId);
        return ExecuteTransaction(strUrl.ToString(), sT);
    }

    private responseNMI RefundTransaction(stcTransactionDetailSet sT)
    {
        var strUrl = new StringBuilder();
        strUrl.AppendFormat("{0}", sT.merchant.processor.gateway.getAPIUrl);
        strUrl.AppendFormat("?type={0}", "refund");
        strUrl.AppendFormat("&security_key={0}", sT.merchant.processor.gateway.apiSecret);
        strUrl.AppendFormat("&transactionid={0}", sT.transaction.transactionId);
        strUrl.AppendFormat("&amount={0}", sT.cart.cartAmount);
        strUrl.AppendFormat("&payment={0}", sT.customer.payment.paymentModeId);
        return ExecuteTransaction(strUrl.ToString(), sT);
    }

    private responseNMI ExecuteTransaction(string strUrl, stcTransactionDetailSet sT)
    {
        {
            var iR = new responseNMI();
            string strResponse = "";
            HttpWebRequest hRequest = HttpWebRequest.Create(strUrl) as HttpWebRequest;
            hRequest.Method = "POST";
            hRequest.ContentType = "application/x-www-form-urlencoded";
            OpenAuthentication(sT, ref hRequest);

            using (HttpWebResponse hResponse = (HttpWebResponse)hRequest.GetResponse())
            {
                if (hResponse.StatusCode == HttpStatusCode.OK)
                {
                    var oReader = new StreamReader(hResponse.GetResponseStream());
                    strResponse = oReader.ReadToEnd();
                }
            }

            if (!string.IsNullOrEmpty(strResponse))
            {
                var dict = HttpUtility.ParseQueryString(strResponse);
                string json = JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v]));
                iR = JsonConvert.DeserializeObject<responseNMI>(json);

                iR.response_message = GetResponseText(iR.response);
                iR.avsresponse_message = GetAVSResponse(iR.avsresponse);
                iR.cvvresponse_message = GetAVSResponse(iR.cvvresponse);
                iR.response_code_message = GetResponseCodeMessage(iR.response_code);
            }

            return iR;
        }
    }

    private HttpWebRequest OpenAuthentication(stcTransactionDetailSet sT, ref HttpWebRequest hRequest)
    {
        string strEncoded = string.Format("{0}:", sT.merchant.processor.gateway.apiSecret);
        strEncoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(strEncoded));
        hRequest.Headers.Add("Authorization", string.Format("Basic {0}", strEncoded));
        return hRequest;
    }

    private string GetResponseText(string ResponseCode)
    {
        string ResponseText = "";

        switch (ResponseCode)
        {
            case "1":
                {
                    ResponseText = "Transaction Approved";
                    break;
                }
            case "2":
                {
                    ResponseText = "Transaction Declined";
                    break;
                }
            case "3":
                {
                    ResponseText = "Error in transaction data or system error";
                    break;
                }
        }

        return ResponseText;
    }

    private string GetAVSResponse(string AVSresponse)
    {
        string AVSresponseText = "";

        switch (AVSresponse)
        {
            case "X":
                {
                    AVSresponseText = "Exact match, 9-character numeric ZIP";
                    break;
                }
            case "Y":
                {
                    AVSresponseText = "Exact match, 5-character numeric ZIP";
                    break;
                }
            case "D":
                {
                    AVSresponseText = "Exact match, 5-character numeric ZIP";
                    break;
                }
            case "M":
                {
                    AVSresponseText = "Exact match, 5-character numeric ZIP";
                    break;
                }
            case "2":
                {
                    AVSresponseText = "Exact match, 5-character numeric ZIP, customer name";
                    break;
                }
            case "6":
                {
                    AVSresponseText = "Exact match, 5-character numeric ZIP, customer name";
                    break;
                }
            case "A":
                {
                    AVSresponseText = "Address match only";
                    break;
                }
            case "B":
                {
                    AVSresponseText = "Address match only / AVS not available";
                    break;
                }
            case "3":
                {
                    AVSresponseText = "Address, customer name match only";
                    break;
                }
            case "7":
                {
                    AVSresponseText = "Address, customer name match only";
                    break;
                }
            case "W":
                {
                    AVSresponseText = "9-character numeric ZIP match only";
                    break;
                }
            case "Z":
                {
                    AVSresponseText = "5-character ZIP match only";
                    break;
                }
            case "P":
                {
                    AVSresponseText = "5-character ZIP match only";
                    break;
                }
            case "L":
                {
                    AVSresponseText = "5-character ZIP match only";
                    break;
                }
            case "1":
                {
                    AVSresponseText = "5-character ZIP, customer name match only";
                    break;
                }
            case "5":
                {
                    AVSresponseText = "5-character ZIP, customer name match only";
                    break;
                }
            case "N":
                {
                    AVSresponseText = "No address or ZIP match only";
                    break;
                }
            case "C":
                {
                    AVSresponseText = "No address or ZIP match only";
                    break;
                }
            case "4":
                {
                    AVSresponseText = "No address or ZIP or customer name match only";
                    break;
                }
            case "8":
                {
                    AVSresponseText = "No address or ZIP or customer name match only";
                    break;
                }
            case "U":
                {
                    AVSresponseText = "Address unavailable";
                    break;
                }
            case "G":
                {
                    AVSresponseText = "Non-U.S. issuer does not participate";
                    break;
                }
            case "I":
                {
                    AVSresponseText = "Non-U.S. issuer does not participate";
                    break;
                }
            case "R":
                {
                    AVSresponseText = "Issuer system unavailable";
                    break;
                }
            case "E":
                {
                    AVSresponseText = "Not a mail/phone order";
                    break;
                }
            case "S":
                {
                    AVSresponseText = "Service not supported";
                    break;
                }
            case "0":
                {
                    AVSresponseText = "AVS not available";
                    break;
                }
            case "O":
                {
                    AVSresponseText = "AVS not available";
                    break;
                }
        }

        return AVSresponseText;
    }

    private string GetCVVResponse(string CVVresponse)
    {
        string CVVresponseText = "";

        switch (CVVresponse)
        {
            case "M":
                {
                    CVVresponseText = "CVV2/CVC2 match";
                    break;
                }
            case "N":
                {
                    CVVresponseText = "CVV2/CVC2 no match";
                    break;
                }
            case "P":
                {
                    CVVresponseText = "Not processed";
                    break;
                }
            case "S":
                {
                    CVVresponseText = "Merchant has indicated that CVV2/CVC2 is not present on card";
                    break;
                }
            case "U":
                {
                    CVVresponseText = "Issuer is not certified and/or has not provided Visa encryption keys";
                    break;
                }
        }

        return CVVresponseText;
    }

    private string GetResponseCodeMessage(string CodeMessage)
    {
        string ResponseCodeMessage = "";

        switch (CodeMessage)
        {
            case "100":
                {
                    ResponseCodeMessage = "Transaction was approved.";
                    break;
                }
            case "200":
                {
                    ResponseCodeMessage = "Transaction was declined by processor.";
                    break;
                }
            case "201":
                {
                    ResponseCodeMessage = "Do not honor.";
                    break;
                }
            case "202":
                {
                    ResponseCodeMessage = "Insufficient funds.";
                    break;
                }
            case "203":
                {
                    ResponseCodeMessage = "Over limit.";
                    break;
                }
            case "204":
                {
                    ResponseCodeMessage = "Transaction not allowed.";
                    break;
                }
            case "220":
                {
                    ResponseCodeMessage = "Incorrect payment information.";
                    break;
                }
            case "221":
                {
                    ResponseCodeMessage = "No such card issuer.";
                    break;
                }
            case "222":
                {
                    ResponseCodeMessage = "No card number on file with issuer.";
                    break;
                }
            case "223":
                {
                    ResponseCodeMessage = "Expired card.";
                    break;
                }
            case "224":
                {
                    ResponseCodeMessage = "Invalid expiration date.";
                    break;
                }
            case "225":
                {
                    ResponseCodeMessage = "Invalid card security code.";
                    break;
                }
            case "226":
                {
                    ResponseCodeMessage = "Invalid PIN.";
                    break;
                }
            case "240":
                {
                    ResponseCodeMessage = "Call issuer for further information.";
                    break;
                }
            case "250":
                {
                    ResponseCodeMessage = "Pick up card.";
                    break;
                }
            case "251":
                {
                    ResponseCodeMessage = "Lost card.";
                    break;
                }
            case "252":
                {
                    ResponseCodeMessage = "Stolen card.";
                    break;
                }
            case "253":
                {
                    ResponseCodeMessage = "Fraudulent card.";
                    break;
                }
            case "260":
                {
                    ResponseCodeMessage = "Declined with further instructions available. (See response text)";
                    break;
                }
            case "261":
                {
                    ResponseCodeMessage = "Declined-Stop all recurring payments.";
                    break;
                }
            case "262":
                {
                    ResponseCodeMessage = "Declined-Stop this recurring program.";
                    break;
                }
            case "263":
                {
                    ResponseCodeMessage = "Declined-Update cardholder data available.";
                    break;
                }
            case "264":
                {
                    ResponseCodeMessage = "Declined-Retry in a few days.";
                    break;
                }
            case "300":
                {
                    ResponseCodeMessage = "Transaction was rejected by gateway.";
                    break;
                }
            case "400":
                {
                    ResponseCodeMessage = "Transaction error returned by processor.";
                    break;
                }
            case "410":
                {
                    ResponseCodeMessage = "Invalid merchant configuration.";
                    break;
                }
            case "411":
                {
                    ResponseCodeMessage = "Merchant account is inactive.";
                    break;
                }
            case "420":
                {
                    ResponseCodeMessage = "Communication error.";
                    break;
                }
            case "421":
                {
                    ResponseCodeMessage = "Communication error with issuer.";
                    break;
                }
            case "430":
                {
                    ResponseCodeMessage = "Duplicate transaction at processor.";
                    break;
                }
            case "440":
                {
                    ResponseCodeMessage = "Processor format error.";
                    break;
                }
            case "441":
                {
                    ResponseCodeMessage = "Invalid transaction information.";
                    break;
                }
            case "460":
                {
                    ResponseCodeMessage = "Processor feature not available.";
                    break;
                }
            case "461":
                {
                    ResponseCodeMessage = "Unsupported card type.";
                    break;
                }
        }

        return ResponseCodeMessage;
    }

    public class responseNMI
    {
        private string _response;
        public string response
        {
            get
            {
                return _response;
            }

            set
            {
                _response = value;
            }
        }

        private string _response_message;
        public string response_message
        {
            get
            {
                return _response_message;
            }

            set
            {
                _response_message = value;
            }
        }

        private string _responsetext;
        public string responsetext
        {
            get
            {
                return _responsetext;
            }

            set
            {
                _responsetext = value;
            }
        }

        private string _authcode;
        public string authcode
        {
            get
            {
                return _authcode;
            }

            set
            {
                _authcode = value;
            }
        }

        private string _transaction_id;
        public string transactionid
        {
            get
            {
                return _transaction_id;
            }

            set
            {
                _transaction_id = value;
            }
        }

        private string _avsresponse;
        public string avsresponse
        {
            get
            {
                return _avsresponse;
            }

            set
            {
                _avsresponse = value;
            }
        }

        private string _avsresponse_message;
        public string avsresponse_message
        {
            get
            {
                return _avsresponse_message;
            }

            set
            {
                _avsresponse_message = value;
            }
        }

        private string _cvvresponse;
        public string cvvresponse
        {
            get
            {
                return _cvvresponse;
            }

            set
            {
                _cvvresponse = value;
            }
        }

        private string _cvvresponse_message;
        public string cvvresponse_message
        {
            get
            {
                return _cvvresponse_message;
            }

            set
            {
                _cvvresponse_message = value;
            }
        }

        private string _orderid;
        public string orderid
        {
            get
            {
                return _orderid;
            }

            set
            {
                _orderid = value;
            }
        }

        private string _type;

        public string type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        private string _response_code;

        public string response_code
        {
            get
            {
                return _response_code;
            }

            set
            {
                _response_code = value;
            }
        }

        private string _response_code_message;

        public string response_code_message
        {
            get
            {
                return _response_code_message;
            }

            set
            {
                _response_code_message = value;
            }
        }

        private string _cc_number;

        public string cc_number
        {
            get
            {
                return _cc_number;
            }

            set
            {
                _cc_number = value;
            }
        }

        private string _customer_vault_id;

        public string customer_vault_id
        {
            get
            {
                return _customer_vault_id;
            }

            set
            {
                _customer_vault_id = value;
            }
        }

        private string _checkaba;

        public string checkaba
        {
            get
            {
                return _checkaba;
            }

            set
            {
                _checkaba = value;
            }
        }

        private string _checkaccount;

        public string checkaccount
        {
            get
            {
                return _checkaccount;
            }

            set
            {
                _checkaccount = value;
            }
        }
    }
}