QuoteFlow.UI.Catalog.SelectAssetVarModal = QuoteFlow.Views.Base.extend({
    templateName: "catalog/select-asset-var-modal",

    options: {
        okFunc: null
    },

    events: {},

    presenter: function () {
        return _.extend(this.defaultPresenter(), {
            assetVars: this.assetVars.toJSON()
        });
    },

    initialize: function (options) {
        // so we can use options throughout each function
        this.options = options || {};

        // required sinced AJS overrides 'this'
        _.bindAll(this, 'submitModal', 'closeModal', 'newAssetVarKeypressHandler', 'disableAssetVarsDropdown', 'createAssetVar', 'getNewAssetVarName', 'getSelectedExistingAssetVar');

        this.assetVars = this.fetchAssetVars();
    },

    postRenderTemplate: function () {
        var self = this;

        _.defer(function () {
            self.modalAJS = AJS.dialog2("#asset_var_selection_modal");
            self.assetVarsDropdown = AJS.$('#select_asset_var').auiSelect2();

            // handle event bindings here since AJS apparently overrides them...
            AJS.$('#new_asset_var').on('keyup', self.newAssetVarKeypressHandler);
            AJS.$('#dialog-close-button').on('click', self.closeModal);
            AJS.$('#dialog-create').on('click', self.submitModal);
        });
    },

    fetchAssetVars: function() {
        var assetVars = new QuoteFlow.Collection.AssetVars();
        assetVars.fetch({
            data: $.param({ id: QuoteFlow.CurrentOrganizationId }),
            async: false
        });

        return assetVars;
    },

    showModal: function () {
        var self = this;
        _.defer(function() {
            self.modalAJS.show();
        });
    },

    closeModal: function () {
        AJS.dialog2("#asset_var_selection_modal").hide();
        this.remove();
        AJS.dialog2("#asset_var_selection_modal").remove();
    },

    submitModal: function(e) {
        var el = $(e.currentTarget);
        el.attr('aria-disabled', "true");

        var assetVar;
        if (this.assetVarsDropdown.prop('disabled')) {
            // user opted to create a new assetvar
            this.createAssetVar(this.getNewAssetVarName());
            this.assetVars = this.fetchAssetVars(); // re-fetch collection to get the id
            assetVar = this.assetVars.at(this.assetVars.length - 1);
        } else {
            // the user has selected an existing assetvar
            var assetVarId = this.getSelectedExistingAssetVar();
            assetVar = this.assetVars.findWhere({ Id: parseInt(assetVarId, 10) });
        }

        this.options.okFunc(assetVar);

        this.closeModal();
    },

    newAssetVarKeypressHandler: function (e) {
        var el = $(e.currentTarget);
        
        if (el.val() !== "") {
            this.disableAssetVarsDropdown();
        } else {
            this.enableAssetVarsDropdown();
        }
    },

    disableAssetVarsDropdown: function() {
        this.assetVarsDropdown.select2("enable", false);
    },

    enableAssetVarsDropdown: function() {
        this.assetVarsDropdown.select2("enable", true);
    },

    createAssetVar: function(assetVarName) {
        if (assetVarName === "") {
            // todo: return failed validation for empty string
        }

        // does this assetvar exist yet?
        var existing = this.assetVars.findWhere({ Name: assetVarName });
        if (existing != undefined) {
            // todo: return failed validation because assetvar already exists
        }

        var assetVar = new QuoteFlow.Model.AssetVar({
            Name: assetVarName,
            Description: null,
            ValueType: "String",
            OrganizationId: QuoteFlow.CurrentOrganizationId,
            Enabled: true,
            CreatedUtc: moment().format(),
            CreatedBy: QuoteFlow.CurrentUserId
        });

        return this.assetVars.create(assetVar, { wait: true });
    },

    getNewAssetVarName: function() {
        return $('#new_asset_var').val();
    },

    getSelectedExistingAssetVar: function() {
        return this.assetVarsDropdown.val();
    }
})