// Parameters
var parameters = {};
parameters.inputmessage = "dummy"; // Edm.String

var req = new XMLHttpRequest();
req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.2/gv_bank_loans(137d1884-47d7-ef11-8eea-7c1e523d27d3)/Microsoft.Dynamics.CRM.gv_action_testSample", false);
req.setRequestHeader("OData-MaxVersion", "4.0");
req.setRequestHeader("OData-Version", "4.0");
req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
req.setRequestHeader("Accept", "application/json");
req.onreadystatechange = function () {
	if (this.readyState === 4) {
		req.onreadystatechange = null;
		if (this.status === 200 || this.status === 204) {
			var result = JSON.parse(this.response);
			console.log(result);
			// Return Type: mscrm.gv_action_testSampleResponse
			// Output Parameters
			var outputmessage = result["outputmessage"]; // Edm.String
		} else {
			console.log(this.responseText);
		}
	}
};
req.send(JSON.stringify(parameters));