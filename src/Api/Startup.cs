using Api.Common;
using Api.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(o =>
                        {
                            o.Filters.Add<HttpGlobalExceptionFilter>();
                        })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(
                        opt =>
                            {
                                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                opt.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                                var resolver = opt.SerializerSettings.ContractResolver;

                                if (resolver == null)
                                {
                                    return;
                                }

                                if (resolver is DefaultContractResolver res)
                                {
                                    res.NamingStrategy = new CamelCaseNamingStrategy();
                                }
                            });

            services.AddMediatRServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
