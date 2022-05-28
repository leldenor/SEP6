using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using SEP6.Models;
using SEP6.Serivces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEP6.Authentication {
    public class AuthProvider {

        private readonly IJSRuntime jsRuntime;
        private readonly AccountService accountService;

        private Account cachedUser;

        public AuthProvider(IJSRuntime jsRuntime, AccountService accountService) {
            this.jsRuntime = jsRuntime;
            this.accountService = accountService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            ClaimsIdentity identity = new ClaimsIdentity();
            if (cachedUser == null) {
                string userAsJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
                if (!string.IsNullOrEmpty(userAsJson)) {
                    cachedUser = JsonSerializer.Deserialize<Account>(userAsJson);

                    identity = SetupClaimsForUser(cachedUser);
                }
            }
            else {
                identity = SetupClaimsForUser(cachedUser);
            }

            ClaimsPrincipal cachedClaimsPrincipal = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(cachedClaimsPrincipal));
        }

        private ClaimsIdentity SetupClaimsForUser(Account cachedUser) {
            throw new NotImplementedException();
        }

        public async Task ValidateLogin(string username, string password) {
            if (string.IsNullOrEmpty(username)) throw new Exception("Enter username");
            if (string.IsNullOrEmpty(password)) throw new Exception("Enter password");
            ClaimsIdentity identity = new ClaimsIdentity();
            try {
                Account user = await accountService.Login(username, password);
                identity = SetupClaimsForUser(user);
                string serialisedData = JsonSerializer.Serialize(user);
                await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
                cachedUser = user;
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
        }

        private void NotifyAuthenticationStateChanged(Task<AuthenticationState> task) {
            throw new NotImplementedException();
        }

        public void Logout() {
            cachedUser = null;
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", null);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task ReCacheUser() {

            Account newAccount = await accountService.GetUser(cachedUser.Username);
            string serialisedData = JsonSerializer.Serialize(newAccount);
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
        }
        public async Task ReCacheUser(string username) {

            Account newAccount = await accountService.GetUser(username);
            string serialisedData = JsonSerializer.Serialize(newAccount);
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
        }
    }
}
