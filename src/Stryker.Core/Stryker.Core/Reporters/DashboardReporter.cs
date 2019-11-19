using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stryker.Core.Reporters
{
    public partial class DashboardReporter : IReporter
    {
        private readonly StrykerOptions _options;
        private readonly IChalk _chalk;

        public DashboardReporter(StrykerOptions options, IChalk chalk = null)
        {
            _options = options;
            _chalk = chalk ?? new Chalk();
        }

        public void OnAllMutantsTested(IReadOnlyInputComponent mutationTree)
        {
            var mutationReport = JsonReport.Build(_options, mutationTree);

            var reportUrl = PublishReport(mutationReport.ToJsonHtmlSafe()).Result;

            if (reportUrl != null)
            {
                _chalk.Green($"\nYour stryker report has been uploaded to: \n " +
                    $"{reportUrl} \n" +
                    $"You can open it in your browser of choice. \n");
            }
            else
            {
                _chalk.Red("Uploading to stryker dashboard failed...");
            }
        }

        private async Task<string> PublishReport(string json)
        {
            var url = new Uri($"{_options.DashboardUrl}/api/{_options.ProjectName}/{_options.ProjectVersion}");
            if (_options.ModuleName != null)
            {
                url = new Uri(url, $"?module={_options.ModuleName}");
            }

            using (var httpclient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                requestMessage.Headers.Add("X-Api-Key", _options.DashboardApiKey);

                var response = await httpclient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var jsonReponse = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeAnonymousType(json, new { Href = "" }).Href;
                }
                else
                {
                    var logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardReporter>();

                    logger.LogError("Dashboard upload failed with statuscode and message: ", response.StatusCode.ToString(), response.ReasonPhrase);

                    return null;
                }

            }
        }

        public void OnMutantsCreated(IReadOnlyInputComponent reportComponent)
        {
        }

        public void OnMutantTested(IReadOnlyMutant result)
        {
        }

        public void OnStartMutantTestRun(IEnumerable<IReadOnlyMutant> mutantsToBeTested, IEnumerable<TestDescription> testDescriptions)
        {
        }
    }
}
