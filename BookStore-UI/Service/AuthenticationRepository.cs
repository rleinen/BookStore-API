using Blazored.LocalStorage;
using BookStore_UI.Contracts;
using BookStore_UI.Models;
using BookStore_UI.Providers;
using BookStore_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookStore_UI.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _storage;
        private readonly AuthenticationStateProvider _authProv;

        public AuthenticationRepository(IHttpClientFactory client, ILocalStorageService storage, AuthenticationStateProvider authProv)
        {
            _client = client;
            _storage = storage;
            _authProv = authProv;

        }

        public async Task<bool> Login(LoginModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.LoginEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if(response.IsSuccessStatusCode == false)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                var token = JsonConvert.DeserializeObject<TokenResponse>(content);
                // Store Token
                await _storage.SetItemAsync("authToken", token.Token);

                // Change App state
                ((ApiAuthStateProv)_authProv).LoggedIn();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);
            }
            catch (Exception e)
            {
                var error = e;
            }


            return true;
        }

        public async Task Logout()
        {
            await _storage.RemoveItemAsync("authToken");
            ((ApiAuthStateProv)_authProv).LoggedOut();

        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RegisterEndpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(user),Encoding.UTF8, "application/json");

            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;

        }
    }
}
