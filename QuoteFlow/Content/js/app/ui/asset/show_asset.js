/// <reference path="../../quoteflow.ts" />
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var QuoteFlow;
(function (QuoteFlow) {
    (function (UI) {
        (function (Asset) {
            var ShowAsset = (function (_super) {
                __extends(ShowAsset, _super);
                function ShowAsset() {
                    _super.apply(this, arguments);
                }
                return ShowAsset;
            })(QuoteFlow.Views.Base.extend);
        })(UI.Asset || (UI.Asset = {}));
        var Asset = UI.Asset;
    })(QuoteFlow.UI || (QuoteFlow.UI = {}));
    var UI = QuoteFlow.UI;
})(QuoteFlow || (QuoteFlow = {}));
//# sourceMappingURL=show_asset.js.map
