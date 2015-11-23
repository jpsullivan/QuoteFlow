"use strict";

import $ from "jquery";

const ctx = $(document);
const Events = {
    /**
     * Subscribes to events
     *
     * @param types
     * @param data
     * @param fn
     */
    bind (types, data, fn) {
        ctx.bind(types, data, fn);
    },

    /**
     * Bind to an event once
     *
     * @param evt
     * @param handler
     */
    one (evt, handler) {
        ctx.one(evt, handler);
    },

    /**
     * Unbind an event
     *
     * @param evt
     * @param handler
     */
    unbind (evt, handler) {
        ctx.unbind(evt, handler);
    },

    /**
     * Publishes events
     *
     * @param evt
     * @param args
     */
    trigger (evt, args) {
        ctx.trigger(evt, args);
    }
};

export default Events;
