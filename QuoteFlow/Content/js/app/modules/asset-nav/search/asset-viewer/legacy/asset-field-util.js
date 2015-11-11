"use strict";

var _ = require('underscore');
var $ = require('jquery');

var AssetFieldUtil = {
    getFieldSelector: function(id) {
        if (id === "issuetype") {
            return "#type-val";
        } else if (id === "fixVersions") {
            return "#fixfor-val";
        } else if (id === "summary") {
            return "#summary-val";
        } else if (id === "labels") {
            return "#wrap-labels .value";
        } else if (id === "duedate") {
            return "#due-date";
        } else {
            return "#" + id + "-val";
        }
    },

    matchesFieldSelector: function(id) {
        return $(this.getFieldSelector(id)).length === 1;
    },

    /**
     * Parses raw response into an object our IssueFieldCollection can consume.
     * In the case of field htmls we strip away labels.
     *
     * @param {Object} resp - raw response from server
     */
    transformFieldHtml: function(resp) {
        resp.fields = _.map(resp.fields, function(field) {

            var $html = $("<div />").htmlCatchExceptions(field.editHtml);

            // Crappy but our edit html includes the label or legend (in case of multi radio/checkbox fields). But we only want the input(s)
            if ($html.find("input.radio,input.checkbox").length !== 0) {
                $html.find("legend").eq(0).remove();
            } else {
                $html.find("label").eq(0).remove();
            }

            return {
                id: field.id,
                label: field.label,
                editHtml: $html.html(),
                required: field.required,
                contentId: field.contentId
            };
        });

        return resp;
    },

    /**
     * Returns the accessKey modifiers required for this browser.
     * @return String The accessKey modifiers.
     */
    getAccessKeyModifier: function() {
        var homeLinkTitle = $("#home_link_drop").attr("title"),
            re = /\(([\w\+]+)\+\w\)/i;

        if (re.test(homeLinkTitle)) {
            return re.exec(homeLinkTitle)[1];
        } else {
            return "Alt";
        }
    }
};

module.exports = AssetFieldUtil;
