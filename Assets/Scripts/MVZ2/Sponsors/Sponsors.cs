using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace MVZ2.Supporters
{
    public class SponsorManager : MonoBehaviour
    {
        public async Task PullSponsors()
        {
            try
            {
                await GetAllSponsors();
            }
            catch (Exception e)
            {
                Debug.LogError($"更新赞助者名单时出现错误：{e}");
            }
        }
        private async Task<SponsorItem[]> GetAllSponsors()
        {
            List<SponsorItem> sponsorList = new List<SponsorItem>();
            int numPerPage = 100;
            var resp = await RequestSponsors(0, numPerPage);
            sponsorList.AddRange(resp.Result.List.Where(p => p.Plans.Length > 0));

            for (int i = 1; i < resp.Result.TotalPage; i++)
            {
                var obj = await RequestSponsors(i + 1, numPerPage);
                sponsorList.AddRange(obj.Result.List.Where(p => p.Plans.Length > 0));
            }

            return sponsorList.ToArray();
        }
        private async Task<GenericResp<ItemList<SponsorItem>>> RequestSponsors(int page = 0, int numPerPage = 100, int[] targetUserID = null)
        {
            SponsorQueryParams sponsorParams = new SponsorQueryParams(page, numPerPage, targetUserID);
            var param = JsonUtility.ToJson(sponsorParams);
            var url = baseUrl + sponsorQueryPath;
            var json = await RequestText(url, param);

            return BsonSerializer.Deserialize<GenericResp<ItemList<SponsorItem>>>(json);
        }
        private void ErrorHandler(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                throw new SponsorNetworkException("无法连接至赞助者服务器。");
            }
            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new SponsorNetworkException("赞助者服务器发送了一个错误响应。");
            }
            if (request.result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new SponsorNetworkException("处理赞助者服务器发送的数据时发生错误。");
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new SponsorNetworkException("发生未知错误。");
            }
        }
        private async Task<string> RequestText(string url, string param)
        {
            var request = await Request(url, param);
            var result = request.downloadHandler.text;
            request.Dispose();
            return result;
        }
        private async Task<UnityWebRequest> Request(string url, string param)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var strToCalc = apiToken + "params" + param + "ts" + timestamp + "user_id" + userID;
            var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(strToCalc));
            var hashStr = string.Concat(hash.Select(b => b.ToString("x2")).ToArray());

            var wwwForm = new WWWForm();
            wwwForm.AddField("user_id", userID);
            wwwForm.AddField("params", param);
            wwwForm.AddField("ts", timestamp);
            wwwForm.AddField("sign", hashStr);
            var request = UnityWebRequest.Post(url, wwwForm);

            request.timeout = 5;

            var tcs = new TaskCompletionSource<AsyncOperation>();

            var requestOp = request.SendWebRequest();
            requestOp.completed += (op) =>
            {
                tcs.SetResult(op);
            };

            await tcs.Task;

            ErrorHandler(request);

            return request;
        }

        [SerializeField]
        private string userID;
        [SerializeField]
        private string apiToken;
        [SerializeField]
        private string baseUrl = "https://afdian.com/api";
        [SerializeField]
        private string sponsorQueryPath = "/open/query-sponsor";
    }
    class SponsorNetworkException : Exception
    {
        public SponsorNetworkException(string message) : base(message) { }
    }
    [SerializeField]
    class SponsorQueryParams
    {
        public int page;
        public int numPerPage;
        public string userID;

        public SponsorQueryParams(int page, int numPerPage = 100, int[] userID = null)
        {
            this.page = page;
            this.numPerPage = numPerPage;
            if (userID != null)
            {
                this.userID = string.Join(",", userID.Select(i => i.ToString()));
            }
        }
    }
    [BsonIgnoreExtraElements]
    [Serializable]
    class GenericResp<T>
    {
        [BsonElement("ec")]
        public int ErrorCode { get; set; }

        [BsonElement("em")]
        public string Message { get; set; }

        [BsonElement("data")]
        public T Result { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Serializable]
    class ItemList<T>
    {
        [BsonElement("total_count")]
        public int TotalCount { get; set; }
        [BsonElement("total_page")]
        public int TotalPage { get; set; }
        [BsonElement("list")]
        public T[] List { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Serializable]
    class SponsorItem
    {
        [BsonElement("sponsor_plans")]
        public Plan[] Plans { get; set; }
        [BsonElement("current_plan")]
        public Plan CurrentPlan { get; set; }
        [BsonElement("all_sum_amount")]
        public string AllSumAmount { get; set; }
        [BsonElement("first_pay_time")]
        public int FirstPayTime { get; set; }
        [BsonElement("last_pay_time")]
        public int LastPayTime { get; set; }
        [BsonElement("user")]
        public User User { get; set; }

    }
    [BsonIgnoreExtraElements]
    [Serializable]
    class Plan
    {
        [BsonElement("plan_id")]
        public string PlanID { get; set; }
        [BsonElement("rank")]
        public int Rank { get; set; }
        [BsonElement("user_id")]
        public string UserID { get; set; }
        [BsonElement("status")]
        public int Status { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("pic")]
        public string Picture { get; set; }
        [BsonElement("desc")]
        public string Description { get; set; }
        [BsonElement("price")]
        public string Price { get; set; }
        [BsonElement("update_time")]
        public int UpdateTime { get; set; }
        [BsonElement("pay_month")]
        public int PayMonth { get; set; }
        [BsonElement("show_price")]
        public string ShowPrice { get; set; }
        [BsonElement("independent")]
        public int Independent { get; set; }
        [BsonElement("permanent")]
        public int Permanent { get; set; }
        [BsonElement("can_buy_hide")]
        public int CanBuyHide { get; set; }
        [BsonElement("need_address")]
        public int NeedAddress { get; set; }
        [BsonElement("product_type")]
        public int ProductType { get; set; }
        [BsonElement("sale_limit_count")]
        public int SaleLimitCount { get; set; }
        [BsonElement("need_invite_code")]
        public bool NeedInviteCode { get; set; }
        [BsonElement("expire_time")]
        public int ExpireTime { get; set; }
        [BsonElement("rankType")]
        public int RankType { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Serializable]
    class User
    {

        [BsonElement("name")]
        public string Name { get; set; }
    }

}
