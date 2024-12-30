using Microsoft.AspNetCore.Mvc;


	public class AppSettingsJsonController : ControllerBase
	{

		private readonly IConfiguration _configuration;

		public AppSettingsJsonController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet("formatting/islocalized")]
		public bool GetIsLocalized()
		{
			var localize = _configuration.GetValue<bool>("Formatting:Localize");

			return localize;
		}

	}

