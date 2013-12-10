﻿using Newtonsoft.Json;

namespace Pingdom.Client
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class PingdomBaseClient
    {
        private readonly HttpClient _baseClient;

        protected PingdomBaseClient()
            : this(new PingdomClientConfiguration())
        { }

        protected PingdomBaseClient(PingdomClientConfiguration configuration)
        {
            var credentials = new CredentialCache
                {
                    {
                        new Uri(configuration.BaseAddress), "basic", new NetworkCredential(configuration.UserName, configuration.Password)
                    }
                };

            var requestHandler = new WebRequestHandler { Credentials = credentials };

            _baseClient = new HttpClient(requestHandler)
            {
                BaseAddress = new Uri(configuration.BaseAddress)
            };

            _baseClient.DefaultRequestHeaders.Add("app-key", configuration.AppKey);
        }

        #region Rest Methods

        internal Task<string> GetAsync(string apiMethod)
        {
            return _baseClient.GetStringAsync(apiMethod);
        }

        internal async Task<T> GetAsync<T>(string apiMethod)
        {
            return await SendAsync<T>(apiMethod, null, HttpMethod.Get);
        }

        internal async Task<T> PostAsync<T>(string apiMethod, object data)
        {
            return await SendAsync<T>(apiMethod, data, HttpMethod.Post);
        }

        internal Task<T> PutAsync<T>(string apiMethod, object data)
        {
            return SendAsync<T>(apiMethod, data, HttpMethod.Put);
        }

        internal async Task<T> DeleteAsync<T>(string apiMethod)
        {
            return await DeleteAsync<T>(apiMethod, null);
        }

        internal async Task<T> DeleteAsync<T>(string apiMethod, object data)
        {
            return await SendAsync<T>(apiMethod, data, HttpMethod.Delete);
        }

        #endregion

        #region Shared Methods

        private async Task<T> SendAsync<T>(string apiMethod, object data, HttpMethod httpMethod)
        {
            var request = new HttpRequestMessage(httpMethod, apiMethod);

            request.Headers.Add("Context-Type", "application/x-www-form-urlencoded");

            if (data != null) request.Content = GetFormUrlEncodedContent(data);

            var sendAsyncTask = _baseClient.SendAsync(request);

            using (var response = await sendAsyncTask)
            {
                var stringResult = await response.Content.ReadAsStringAsync();
                return await JsonConvert.DeserializeObjectAsync<T>(stringResult);
            }
        }

        private static FormUrlEncodedContent GetFormUrlEncodedContent(object anonymousObject)
        {
            var properties = from propertyInfo in anonymousObject.GetType().GetProperties()
                             where propertyInfo.GetValue(anonymousObject, null) != null
                             select new KeyValuePair<string, string>(propertyInfo.Name, WebUtility.UrlEncode(propertyInfo.GetValue(anonymousObject, null).ToString()));

            return new FormUrlEncodedContent(properties);
        }

        #endregion

    }
}