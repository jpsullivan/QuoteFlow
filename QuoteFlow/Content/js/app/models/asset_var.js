QuoteFlow.Model.AssetVar = Backbone.Model.extend({
    
    defaults: function() {
        return {
            Id: null,
            Name: "",
            Description: "",
            ValueType: "",
            OrganizationId: "",
            Enabled: true,
            CreatedUtc: null,
            CreatedBy: 0
        }
    },

    isEnabled: function () {
        var enabled = this.get("Enabled");
        if (enabled === false || enabled === undefined) {
            return false;
        }
        return true;
    },
})