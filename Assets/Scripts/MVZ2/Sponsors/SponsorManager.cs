using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MVZ2.IO;
using MVZ2.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace MVZ2.Supporters
{
    public class SponsorManager : MonoBehaviour
    {
        public async Task PullSponsors(TaskProgress progress)
        {
            SponsorInfos sponserCache = null;
            try
            {
                sponserCache = LoadSponsorsCache();
            }
            catch (Exception e)
            {
                Debug.LogError($"加载赞助者名单缓存时出现错误：{e}");
            }
            try
            {
                if (!ShouldRepull(sponserCache))
                {
                    SetCurrentSponsorInfos(sponserCache);
                    progress.SetProgress(1);
                    return;
                }
                var items = await GetAllSponsors(progress);
                var test = items.OrderByDescending(s => s.LastPayTime).Select(s => s.User.Name).ToArray();
                sponserCache = new SponsorInfos(items);
                sponserCache.lastUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                SetCurrentSponsorInfos(sponserCache);
                SaveSponsorsCache(sponserCache);
            }
            catch (Exception e)
            {
                Debug.LogError($"更新赞助者名单时出现错误：{e}");
            }
        }
        public string[] GetSponsorPlanNames(int rank, int rankType)
        {
            if (currentSponsors == null)
                return Array.Empty<string>();
            return currentSponsors.sponsors.Where(s => s.plans.Any(p => p.rankType == rankType && p.rank >= rank)).Select(s => s.name).ToArray();
        }
        public bool HasSponsorPlan(string name, int rank, int rankType)
        {
            if (currentSponsors == null)
                return false;
            return currentSponsors.sponsors.Any(s => s.plans.Any(p => p.rankType == rankType && p.rank >= rank));
        }
        private async Task<SponsorItem[]> GetAllSponsors(TaskProgress progress)
        {
            List<SponsorItem> sponsorList = new List<SponsorItem>();
            int numPerPage = 100;
            var resp = await RequestSponsors(1, numPerPage);
            sponsorList.AddRange(resp.Result.List);
            var totalPage = resp.Result.TotalPage;
            progress.SetProgress(1 / (float)totalPage);

            for (int i = 2; i <= resp.Result.TotalPage; i++)
            {
                var obj = await RequestSponsors(i, numPerPage);
                sponsorList.AddRange(obj.Result.List);
                progress.SetProgress(i / (float)totalPage);
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

        private SponsorInfos LoadSponsorsCache()
        {
            var path = GetSponsorCacheFilePath();
            if (!File.Exists(path))
            {
                return null;
            }
            var json = Main.FileManager.ReadJsonFile(path);
            return BsonSerializer.Deserialize<SponsorInfos>(json);
        }
        private void SaveSponsorsCache(SponsorInfos saveInfo)
        {
            var path = GetSponsorCacheFilePath();
            FileHelper.ValidateDirectory(path);
            var json = saveInfo.ToJson();
            Main.FileManager.WriteJsonFile(path, json);
        }
        private bool ShouldRepull(SponsorInfos infos)
        {
            if (infos == null)
                return true;
            var lastTime = DateTimeOffset.FromUnixTimeSeconds(infos.lastUpdateTime).LocalDateTime;
            var nowTime = DateTimeOffset.Now;
            if (nowTime.Date != lastTime.Date)
            {
                return true;
            }
            return false;
        }
        private string GetSponsorCacheFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "sponsors.dat");
        }
        private void SetCurrentSponsorInfos(SponsorInfos infos)
        {
            currentSponsors = infos;
        }
        public MainManager Main => MainManager.Instance;
        private SponsorInfos currentSponsors;
        [SerializeField]
        private string userID;
        [SerializeField]
        private string apiToken;
        [SerializeField]
        private string baseUrl = "https://afdian.com/api";
        [SerializeField]
        private string sponsorQueryPath = "/open/query-sponsor";
    }
}
