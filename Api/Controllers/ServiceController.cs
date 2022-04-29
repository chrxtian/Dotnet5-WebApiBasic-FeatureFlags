using Api.Data;
using Api.Enums;
using Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private IFeatureManager _featureManager { get; set; }

        public ServiceController(IFeatureManager featureManager)
        {
            _featureManager = featureManager;
        }

        [HttpGet("feature-flags")]
        public async Task<IActionResult> settings()
        {
            var featureFlagsDict = new Dictionary<string, bool>();

            foreach (FeatureFlags featureFlag in Enum.GetValues(typeof(FeatureFlags)))
            {
                var featureName = featureFlag.ToString();
                var featureValue = await _featureManager.IsEnabledAsync(featureName);
                featureFlagsDict.Add(featureName, featureValue);
            }
            
            return Ok(new { FeatureFlags = featureFlagsDict.ToList() });
        }
    }
}
