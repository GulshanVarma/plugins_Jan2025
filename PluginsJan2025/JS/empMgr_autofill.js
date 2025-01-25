// Form > Field -> Manager > Events > OnChange
function fetchRelatedEntityFieldValue(executionContext) {
    var formContext = executionContext.getFormContext(); 

    var lookupValue = formContext.getAttribute("gv_mgr").getValue();
    
    if (lookupValue !== null && lookupValue.length > 0) {
        var ManagerID = lookupValue[0].id.replace(/[{}]/g, ""); // Get the GUID of the Contact
        
        Xrm.WebApi.retrieveRecord("gv_manager", ManagerID, "?$select=gv_managerid,gv_location,_gv_managingproject_value")
            .then(function (result) {
                debugger
                var updatedLocation = result.gv_location; 
                formContext.getAttribute("gv_location").setValue(updatedLocation);
                formContext.getAttribute("gv_sf_projectreference").setValue(result._gv_managingproject_value);
            })
            .catch(function (error) {
                console.error("Error fetching related entity field: " + error.message);
            });
    }
    else{
        formContext.getAttribute("gv_location").setValue(null);
        formContext.getAttribute("gv_sf_projectreference").setValue(null);
    }
}


// Form Properties > Events > OnSave
function SaveRelatedEntityFieldValue(executionContext) {
    var formContext = executionContext.getFormContext(); 
    if(formContext.getAttribute("gv_location").getValue() === null){
        var lookupValue = formContext.getAttribute("gv_mgr").getValue();
        
        if (lookupValue !== null && lookupValue.length > 0) {
            var contactId = lookupValue[0].id.replace(/[{}]/g, ""); // Get the GUID of the Contact
            
            Xrm.WebApi.retrieveRecord("gv_manager", contactId, "?$select=gv_managerid,gv_location,_gv_managingproject_value")
                .then(function (result) {
                    var updatedLocation = result.gv_location; 
                    formContext.getAttribute("gv_location").setValue(updatedLocation);
                    formContext.getAttribute("gv_sf_projectreference").setValue(result._gv_managingproject_value);
                })
                .catch(function (error) {
                    console.error("Error fetching related entity field: " + error.message);
                });
        }
        else{
            formContext.getAttribute("gv_location").setValue(null);
            formContext.getAttribute("gv_sf_projectreference").setValue(null);
        }
    }
}