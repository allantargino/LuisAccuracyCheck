//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services

//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/ProjectOxford-ClientSDK

//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using Microsoft.Cognitive.LUIS.Structures.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Cognitive.LUIS.Manager
{
    public class IntentHandler : LuisHandler
    {
        /// <summary>
        /// Creates a intent LUIS service handler.
        /// </summary>
        /// <param name="appId">The application ID of the LUIS application.</param>
        /// <param name="subscriptionKey">The subscription key of the LUIS account.</param>
        /// <param name="baseApiUrl">Root URI for the service endpoint.</param>
        public IntentHandler(string appId, string subscriptionKey, string baseApiUrl) : base(appId, subscriptionKey, baseApiUrl) { }

        /// <summary>
        /// Creates an intent on a LUIS application.
        /// </summary>
        /// <param name="name">Name of the intent.</param>
        /// <param name="versionId">Application version to be used.</param>
        /// <returns>The created intent in a LUIS application.</returns>
        public async Task<LuisIntent> AddIntentAsync(string name, string versionId = DEFAULT_VERSION_ID)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var uri = CreateHandlerUri($"api/v2.0/apps/{_appId}/versions/{versionId}/intents");

            var intention = new
            {
                Name = name
            };
            var response = await _httpClient.RestPost(uri, intention);
            var id = new Guid(response.ToString());

            return new LuisIntent(_appId, _subscriptionKey, _baseApiUrl, id, name);
        }

        /// <summary>
        /// Searches and gets an intent on a LUIS application.
        /// </summary>
        /// <param name="name">Name of the intent.</param>
        /// <param name="versionId">Application version to be used.</param>
        /// <returns>The intent from LUIS application.</returns>
        public async Task<LuisIntent> GetIntentAsync(string name, string versionId = DEFAULT_VERSION_ID)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var path = $"api/v2.0/apps/{_appId}/versions/{versionId}/intents";
            var uri = CreateHandlerUri(path);

            var response = await _httpClient.RestGet(uri);
            var intents = response.ToObject<IEnumerable<Intent>>();

            var intent = (from i in intents
                       where i.Name.ToLower() == name.ToLower()
                       select new LuisIntent(_appId, _subscriptionKey, _baseApiUrl, i.Id, name)).First();

            return intent;
        }

        /// <summary>
        /// Gets an intent on a LUIS application.
        /// </summary>
        /// <param name="id">Id of the intent.</param>
        /// <param name="versionId">Application version to be used.</param>
        /// <returns>The intent from LUIS application.</returns>
        public async Task<LuisIntent> GetIntentAsync(Guid id, string versionId = DEFAULT_VERSION_ID)
        {
            if (id == Guid.Empty) throw new ArgumentNullException(nameof(id));

            var path = $"api/v2.0/apps/{_appId}/versions/{versionId}/intents/{id}";
            var uri = CreateHandlerUri(path);

            var response = await _httpClient.RestGet(uri);
            var intent = response.ToObject<Intent>();

            return new LuisIntent(_appId, _subscriptionKey, _baseApiUrl, intent.Id, intent.Name);
        }

        public async Task<IEnumerable<LuisIntent>> GetIntentsAsync(string versionId = DEFAULT_VERSION_ID)
        {
            var path = $"api/v2.0/apps/{_appId}/versions/{versionId}/intents?skip=0&take=100";
            var uri = CreateHandlerUri(path);

            var uri2 = $"https://westus.api.cognitive.microsoft.com/luis/api/v2.0/apps/{_appId}/versions/{versionId}/intents";

            var response = await _httpClient.RestGet(uri2);
            var intents = response.ToObject<IEnumerable<Intent>>();

            return
                from i in intents
                select new LuisIntent(_appId, _subscriptionKey, _baseApiUrl, i.Id, i.Name);
        }

        public async Task<IEnumerable<LabelResponse>> GetLabelsAsync(int skip = 0, int take = 500, string versionId = DEFAULT_VERSION_ID)
        {
            var uri = CreateHandlerUri($"api/v2.0/apps/{_appId}/versions/{versionId}/examples?skip={skip}&take={take}");

            var response = await _httpClient.RestGet(uri);

            var labels = response.ToObject<IEnumerable<LabelResponse>>();

            //var app = (from a in applications
            //           where a.Name.ToLower() == name.ToLower()
            //           select new LuisApplication(a.Id, name, _subscriptionKey)).First();

            return labels;
        }

        public async Task<IEnumerable<LabelResponse>> GetAllLabelsAsync(string versionId = DEFAULT_VERSION_ID)
        {
            var labels = new List<LabelResponse>();

            var skip = 0;
            var take = 500;
            while (true)
            {
                var result = await GetLabelsAsync(skip, take, versionId);
                labels.AddRange(result);

                if (result.Count() == take)
                    skip += take;
                else
                    break;
            }

            return labels;
        }
    }
}
