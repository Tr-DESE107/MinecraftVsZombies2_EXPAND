using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MVZ2.Supporters
{
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
