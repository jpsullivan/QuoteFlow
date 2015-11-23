"use strict";

import Marionette from "backbone.marionette";

// admin routers
import AdminQuoteStatusRouter from "./modules/admin/quote-status/router";

// standard routers
import AssetRouter from "./modules/asset/router";
import AssetNavRouter from "./modules/asset-nav/router";
import CatalogRouter from "./modules/catalog/router";
/**
 * The global router for Intra web. Handles spinning up every subrouter
 * throughout the frontend and the management console.
 * @extends {Marionette.AppRouter}
 */
export default Marionette.AppRouter.extend({
    initialize (options) {
        this._loadAdminRoutes(options);
        this._loadStandardRoutes(options);
    },

    _loadAdminRoutes (options) {
        new AdminQuoteStatusRouter({app: options.application, appRouter: this});
    },

    _loadStandardRoutes (options) {
        new AssetRouter({app: options.application, appRouter: this});
        new AssetNavRouter({app: options.application, appRouter: this});
        new CatalogRouter({app: options.application, appRouter: this});
    }
});
