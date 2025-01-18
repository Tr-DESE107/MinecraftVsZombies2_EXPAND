using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace MVZ2.Supporters
{
    [SerializeField]
    class SponsorQueryParams
    {
        public int page;
        public int per_page;
        public string user_id;

        public SponsorQueryParams(int page, int numPerPage = 100, int[] userID = null)
        {
            this.page = page;
            this.per_page = numPerPage;
            if (userID != null)
            {
                this.user_id = string.Join(",", userID.Select(i => i.ToString()));
            }
        }
    }
}
