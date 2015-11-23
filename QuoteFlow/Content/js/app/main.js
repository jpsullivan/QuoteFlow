"use strict";

import App from "./app";
import "./helpers/handlebars";

// Need this globally so menu functions
import "@atlassian/aui/lib/js/aui/dropdown2";
import "@atlassian/aui/lib/js/aui/header-async";

// require moment w/ specific locales and globally set the locale
import moment from "moment";
import "moment/locale/en-au";
import "moment/locale/en-ca";
moment.locale(window.locale);

const app = new App({
    rootUrl: window.rootUrl,
    applicationPath: window.applicationPath,
    currentOrgId: window.currentOrganization,
    currentUser: window.currentUser
});

if (module.hot) {
    module.hot.accept();
}

app.start();
