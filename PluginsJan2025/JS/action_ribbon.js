if (BankingPortal == undefined){
	var BankingPortal = {__namespace:true}
}
if (BankingPortal.gv_bank_loan == undefined) {
	BankingPortal.gv_bank_loan = { __namespace: true }
}
if (BankingPortal.gv_bank_loan.Ribbon == undefined) {
	BankingPortal.gv_bank_loan.Ribbon = { __namespace: true }
}


BankingPortal.gv_bank_loan.Ribbon.Command = (function (primaryControl, primaryitemids, primaryentitytypename,
	primaryentitytypecode, firstprimaryitemid, selectedentitytypecode, selectedentitytypename) {               // ()() = immediate invoking function

	var createActionFunction = function (primaryControl, primaryitemids, primaryentitytypename,
		primaryentitytypecode, firstprimaryitemid, selectedentitytypecode, selectedentitytypename) {
		debugger
		var formContext = primaryControl;
		var inputParam = window.prompt("enter input: ");
		// Parameters
		var parameters = {};
		parameters.inputmessage = inputParam; // Edm.String
		var outputmessage = "";
		var req = new XMLHttpRequest();
		var guid = primaryControl._data._entity._entityId.guid;
		req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.2/gv_bank_loans(" + guid + ")/Microsoft.Dynamics.CRM.gv_action_testSample", false);
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
					 outputmessage = result["outputmessage"]; // Edm.String
				} else {
					console.log(this.responseText);
				}
			}
		};
		req.send(JSON.stringify(parameters));
		Xrm.Utility.alertDialog(outputmessage);
	}

	return{                                                               // return for ribbon button
		var_CREATEActionFunction : createActionFunction
	}
})();   
	
// BankingPortal.gv_bank_loan.Ribbon.Command.var_CREATEActionFunction


BankingPortal.gv_bank_loan.Ribbon.CommandGrid = (function (primaryControl,
	selectedcontrolselecteditemids, selectedcontrolselecteditemrefernces, selectedcontrolallitemids, selectedcontrolselecteditemreferences) {               // ()() = immediate invoking function

	var createActionFunction = function (primaryControl, selectedcontrolselecteditemids, selectedcontrolselecteditemrefernces, selectedcontrolallitemids, selectedcontrolselecteditemreferences) {
		debugger
		var formContext = primaryControl;
		var inputParam = window.prompt("enter input: ");


		var alertStrings = { confirmButtonLabel: "Yes", text: "This is an alert.", title: "Sample title" };
		var alertOptions = { height: 120, width: 260 };
		Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
			function (success) {
				console.log("Alert dialog closed");
			},
			function (error) {
				console.log(error.message);
			}
		);


		// Parameters
		var parameters = {};
		parameters.inputmessage = inputParam; // Edm.String
		var outputmessage = "";
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
					outputmessage = result["outputmessage"]; // Edm.String
				} else {
					console.log(this.responseText);
				}
			}
		};
		req.send(JSON.stringify(parameters));
		Xrm.Utility.alertDialog(outputmessage);
	}

	return {                                                               // return for ribbon button
		var_CREATEActionFunction: createActionFunction
	}
})();   

// BankingPortal.gv_bank_loan.Ribbon.CommandGrid.var_CREATEActionFunction