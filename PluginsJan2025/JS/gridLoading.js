// https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/clientapi-form-context

// gridContext.getFetchXml()  -- get current fetch xml
// Xrm.Page.data.entity.attributes.get()  // to view all FORM attributes

// Get value
//var website = formContext.getAttribute("websiteurl").getValue();

// Set value
//formContext.getAttribute("websiteurl").setValue("http://newvalue.com");


//add "" in XML
//https://www.ashishvishwakarma.com/FetchXmlFormatter/

// https://learn.microsoft.com/en-us/power-apps/developer/model-driven-apps/clientapi/reference/xrm-webapi/retrieverecord
				
// POSTMAN - entity name "PLURAL"
// JS - entity name "SINGULAR"

// NOTE -- fetchXML doesnt work on related entity filter with JS, only works with GUI
// NOTE -- addPreSearch works only on Lookup (filter), before user start typing show related records
	//  Xrm.Page.getControl("lookupfieldid").addPreSearch(FilterLookup);
	// function FilterLookup() {var fetchXML = ""; Xrm.Page.getControl("lookupfieldid").addCustomFilter(fetchXML);}
// NOTE -- GUID current entity - 1. remove {} 2. toLowerCase()

// Approaches
// 1. Use WebAPIs to get related entity (lookup) details -- FAILED, couldnt attach ProjectID with filter as no col is present in emp
// 2. Use related Lookup Field as filter (view) -- FAILED, as no col is present in emp (proj lookup in Manager entity)
// 3. Use Surrogate String Field with Project GUID, JS to autofill project ID when EMP is created (WebAPI - from MgrID, get ProjID)
//		get FetchXML query from View, update with params in JS ()

// STEPS -- 
// 1. Create filter in GUI View. 
// 2. Copy fetchXML in js + run. 
// 3.Replace fetchXML with parameters

function LoadDataInSubgridEmp(executionContext) {
	var formContext = executionContext.getFormContext();
	var gridContext = formContext.getControl("Subgrid_Employees");
	var projectID = formContext.data.entity.getId().toString().slice(1,-1);

	var fetchXMLQ = "<fetch version='1.0' output-format='xml-platform' mapping='logical' savedqueryid='af2b2a45-bfc6-ef11-b8e8-7c1e523d27d3' distinct='true' no-lock='false'>"+
	"	<entity name='gv_emp'>"+
	"		<attribute name='gv_newcolumn'/>"+
	"		<attribute name='gv_empid'/>"+
	"		<attribute name='gv_mgr'/>"+
	"		<attribute name='gv_location'/>"+
	"		<attribute name='gv_sal'/>"+
	"		<filter type='and'>"+
	"			<condition attribute='gv_sf_projectreference' operator='eq' value='"+projectID.toLowerCase()+"'/>"+
	"		</filter>"+
	"		<link-entity alias='a_e4198aac9ecb443fa91de4258631401c' name='gv_manager' to='gv_mgr' from='gv_managerid' link-type='outer' visible='false'>"+
	"			<attribute name='gv_managingproject'/>"+
	"			<attribute name='gv_name'/>"+
	"			<attribute name='gv_location'/>"+
	"			<attribute name='modifiedby'/>"+
	"		</link-entity>"+
	"		<order attribute='gv_newcolumn' descending='false'/>"+
	"	</entity>"+
	"</fetch>";

	gridContext.setFilterXml(fetchXMLQ);
	formContext.getControl("Subgrid_Employees").refresh();	
}

 


/*
<fetch version="1.0" output-format="xml-platform" mapping="logical" savedqueryid="af2b2a45-bfc6-ef11-b8e8-7c1e523d27d3" distinct="true" returntotalrecordcount="true" page="1" count="4" no-lock="false">
	<entity name="gv_emp">
		<attribute name="gv_newcolumn"/>
		<attribute name="gv_empid"/>
		<attribute name="gv_mgr"/>
		<attribute name="gv_location"/>
		<attribute name="gv_sal"/>
		<link-entity alias="a_e4198aac9ecb443fa91de4258631401c" name="gv_manager" to="gv_mgr" from="gv_managerid" link-type="inner">
			<attribute name="gv_managingproject"/>
			<attribute name="gv_name"/>
			<attribute name="gv_location"/>
			<attribute name="modifiedby"/>
			<filter type="and">
				<condition attribute="gv_managingprojectname" operator="like" value="%-%"/>
			</filter>
		</link-entity>
		<order attribute="gv_newcolumn" descending="false"/>
	</entity>
</fetch>*/



/*
	// timeout as data is not loaded in grid instantly
	// print all data in grid	

	 setTimeout(function () {
		var rows = gridContext.getGrid().getRows();
		
		rows.forEach(function (row, i) {
			var gridColumns = row.getData().getEntity().attributes;
			
			gridColumns.forEach(function (column, j) {
                var gridrowname = column.getName();
                var gridrowvalue = column.getValue();
				
				if(gridrowname == "gv_mgr"){ 
					// lookup column - remove '{}' from GUID - slice
					var GUIDMgr = gridrowvalue[0].id.toString().slice(1, -1); 
					
					Xrm.WebApi.retrieveRecord("gv_manager", GUIDMgr, "?$select=gv_name,_gv_managingproject_value").then(
						function success(result) {
							// Manager table have guid of project, formattedValue
							var resultProjectName = result["_gv_managingproject_value@OData.Community.Display.V1.FormattedValue"]
							console.log(resultProjectName)
						},
						function (error) {
							console.log(error.message)
						}
					);
				}
            });
		});
	 }, 5000);
*/
	
	


function LoadDataInSubgridEmpThroughWebAPI(executionContext) {
    var formContext = executionContext.getFormContext();
 
	// get grid 
    var gridContext = formContext.getControl("Subgrid_Employees");
	debugger
	// get field name 
	var projectName = Xrm.Page.getAttribute('gv_name').getValue();
	
	var associatedEmp;
	
	if(gridContext ==null){
		setTimeout(LoadDataInSubgridEmp,3000);
	}else{
		if(projectName != null){
		
			Xrm.WebApi.retrieveMultipleRecords("gv_emps", "?$select=gv_mgr&$top=10").then(
			function success(result) {
				for (var i = 0; i < result.entities.length; i++) {
					console.log(result.entities[i]);
				}                    
				// perform additional operations on retrieved records
			},
			function (error) {
				console.log(error.message);
				// handle error conditions
			}
			);
		}
	}
}
