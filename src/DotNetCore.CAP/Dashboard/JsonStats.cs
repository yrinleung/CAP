﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace DotNetCore.CAP.Dashboard
{
    internal class JsonStats : IDashboardDispatcher
    {
        public async Task Dispatch(DashboardContext context)
        {
            var requestedMetrics = await context.Request.GetFormValuesAsync("metrics[]");
            var page = new StubPage();
            page.Assign(context);

            var metrics = DashboardMetrics.GetMetrics().Where(x => requestedMetrics.Contains(x.Name));
            var result = new Dictionary<string, Metric>();

            foreach (var metric in metrics)
            {
                var value = metric.Func(page);
                result.Add(metric.Name, value);
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new JsonConverter[] { new StringEnumConverter { CamelCaseText = true } }
            };
            var serialized = JsonConvert.SerializeObject(result, settings);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(serialized);
        }

        private class StubPage : RazorPage
        {
            public override void Execute()
            {
            }
        }
    }
}