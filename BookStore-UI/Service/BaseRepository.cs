using BookStore_UI.Contracts;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;

namespace BookStore_UI.Service
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _client;

        public BaseRepository(IHttpClientFactory client)
        {
            _client = client;

        }


        public async Task<bool> Create(string url, T obj)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (obj == null) return false;
            request.Content = new StringContent(JsonConvert.SerializeObject(obj));
            var client = _client.CreateClient();
            HttpResponseMessage resp = await client.SendAsync(request);
            if (resp.StatusCode == HttpStatusCode.Created) return true;
            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1) return false;
            var request = new HttpRequestMessage(HttpMethod.Delete, url + id);

            var client = _client.CreateClient();
            HttpResponseMessage resp = await client.SendAsync(request);
            if (resp.StatusCode == HttpStatusCode.NoContent) return true;
            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url + id);

            var client = _client.CreateClient();
            HttpResponseMessage resp = await client.SendAsync(request);

            if (resp.StatusCode == HttpStatusCode.OK) 
            {
                var content = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return null;
        }

        public async Task<IList<T>> Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _client.CreateClient();
            HttpResponseMessage resp = await client.SendAsync(request);

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var content = await resp.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }
            return null;
        }

        public async Task<bool> Update(string url, T obj)
        {
            if (obj == null) return false;
            var request = new HttpRequestMessage(HttpMethod.Put, url);

            request.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            HttpResponseMessage resp = await client.SendAsync(request);
            if (resp.StatusCode == HttpStatusCode.NoContent) return true;
            return false;
        }
    }
}
