"use strict";

var _ = require('underscore');
var $ = require('jquery');
var AsyncData = require('../legacy/async-data');

var ViewAssetData = AsyncData.extend({

    initialize: function(options) {
        AsyncData.prototype.initialize.call(this, _.defaults(options || {}, {
            disableCache: false,
            maxCacheSize: +10
        }));
    },

    /**
     * @param {String} id The id of the asset
     */
    fetch: function(id, options) {
        var data = {
            assetId: id,
            decorator: "none",
            prefetch: !!options.prefetch
        };

        // JRA-36659: keep track of whether we are in detail view - if we are we won't update the current catalog
        data.shouldUpdateCurrentCatalog = !options.detailView;

        if (options.mergeIntoCurrent) {
            if (this.data && this.data[id] && this.data[id].value && this.data[id].value.asset) {
                data.lastReadTime = this.data[id].value.readTime;

                data.fields = [];
                _.each(this.data[id].value.fields, function(field) {
                    data.fields.push(field.id + ":" + field.contentId);
                });

                data.asset = ["summary:" + this.data[id].value.asset.id];

                data.links = [];
                var collectGroupLinks = function(group) {
                    _.each(group.links, function(link) {
                        data.links.push(link.id + ":" + link.contentId);
                    });
                    _.each(group.groups, function(nestedGroup) {
                        collectGroupLinks(nestedGroup);
                    });
                };
                // _.each(this.data[id].value.asset.operations.linkGroups, function(group) {
                //     collectGroupLinks(group);
                // });
                //
                // data.panels = [];
                // var instance = this;
                // var collectPanels = function(location) {
                //     _.each(instance.data[id].value.panels[location], function(panel) {
                //         data.panels.push(panel.completeKey + ":" + panel.contentId);
                //     });
                // };
                // collectPanels("leftPanels");
                // collectPanels("rightPanels");
                // collectPanels("infoPanels");
            }
        }

        if ("loadFields" in options) {
            data.loadFields = options.loadFields;
        }

        if (options.assetEntity) {
            _.defaults(data, options.assetEntity.viewAssetQuery);
        }

        var jqXhr = $.ajax({
            url: options.mergeIntoCurrent ? QuoteFlow.RootUrl + "api/asset/GetAssetMergeCurrent" : QuoteFlow.RootUrl + "api/asset/GetAsset",
            data: data,
            contentType: 'application/json',
            type: options.mergeIntoCurrent ? "POST" : "GET"
        });
        var deferred = jqXhr.pipe(function(data) {
            return data;
        }, function(xhr) {
            return _.pick(xhr, 'status', 'responseText', 'statusText');
        });
        deferred.abort = function() {
            jqXhr.abort.apply(jqXhr, arguments);
        };
        return deferred;
    },

    mergeFetchedAndCached: function(cachedData, fetchedData, options) {
        if (this._skipMerging(cachedData, fetchedData, options)) {
            _.extend(cachedData, fetchedData);

            return null;
        }

        // Copy all properties except "value".
        _.each(_.pairs(fetchedData), function(pair) {
            if (pair[0] !== "value") {
                cachedData[pair[0]] = pair[1];
            }
        });

        var fetched = fetchedData.value;

        // Merge values.
        var fieldsKeysToRemove = [];
        var panelKeysToRemove = [];
        var linkKeysToRemove = [];
        _.each(fetched.removedContentIds, function(keys, category) {
            var keysToRemove;
            if (category === "fields") {
                keysToRemove = fieldsKeysToRemove;
            } else if (category === "panels") {
                keysToRemove = panelKeysToRemove;
            } else if (category === "links") {
                keysToRemove = linkKeysToRemove;
            }

            if (keysToRemove) {
                _.each(keys, function(key) {
                    keysToRemove.push(key);
                });
            }
        });

        var fetchedFields = fetched.fields;
        var fetchedLinkGroups = fetched.issue.operations.linkGroups;
        var fetchedPanels = fetched.panels;
        var fetchedSummary = fetched.issue.summary;

        if (this._nothingChanged(fieldsKeysToRemove, panelKeysToRemove, linkKeysToRemove,
                fetchedFields, fetchedPanels, fetchedLinkGroups, fetchedSummary)) {

            return null;
        }

        var changedData = {updated: {}, added: {}, deleted: {}};
        var cached = cachedData.value;
        var comparator = this._comparator;

        if (fieldsKeysToRemove.length > 0 || fetchedFields.length > 0) {
            this._mergeFields(fetchedFields, cached, changedData, fieldsKeysToRemove);
        }
        if (panelKeysToRemove.length > 0 || fetchedPanels.leftPanels.length > 0 || fetchedPanels.rightPanels.length > 0 || fetchedPanels.infoPanels.length > 0) {
            this._mergePanels(fetchedPanels, cached, changedData, panelKeysToRemove, comparator);
        }
        if (linkKeysToRemove.length > 0 || fetchedLinkGroups.length > 0) {
            this._mergeLinks(fetchedLinkGroups, cached, changedData, linkKeysToRemove, comparator);
        }
        if (fetchedSummary) {
            this._mergeSummary(fetched, cached, changedData);
        }

        return changedData;
    },

    _skipMerging: function(cachedData, fetchedData, options) {
        return _.isEmpty(cachedData) || !options.mergeIntoCurrent || !cachedData.value || !cachedData.value.issue ||
            fetchedData.error === true;
    },

    _nothingChanged: function(fieldsKeysToRemove, panelKeysToRemove, linkKeysToRemove,
                              fetchedFields, fetchedPanels, fetchedLinkGroups, fetchedSummary) {
        return fieldsKeysToRemove.length === 0 && panelKeysToRemove.length === 0 && linkKeysToRemove.length === 0 &&
            fetchedFields.length === 0 && fetchedLinkGroups.length === 0 &&
            fetchedPanels.leftPanels.length === 0 && fetchedPanels.rightPanels.length === 0 && fetchedPanels.infoPanels.length === 0 && !fetchedSummary;
    },

    _mergeFields: function(fetchedFields, cached, changedData, keysToRemove) {
        changedData.added.fields = [];
        changedData.updated.fields = [];
        changedData.deleted.fields = [];

        _.each(fetchedFields, function(fetchedField) {
            var existing = _.find(cached.fields, function(cachedField) {
                return cachedField.id === fetchedField.id;
            });
            if (existing) {
                changedData.updated.fields.push(fetchedField.id);
            } else {
                changedData.added.fields.push(fetchedField.id);
            }
        });

        var newFields = fetchedFields;

        _.each(cached.fields, function(oldField) {
            if (_.contains(keysToRemove, oldField.id)) {
                changedData.deleted.fields.push(oldField.id);
            } else if (!_.contains(changedData.updated.fields, oldField.id)) {
                newFields.push(oldField);
            }
        });

        cached.fields = newFields;
    },

    _mergeSummary: function(fetched, cached, changedData) {
        changedData.updated.asset = ["summary"];

        cached.aset.summary = fetched.asset.summary;
        cached.asset.summaryContentId = fetched.asset.summaryContentId;
    },

    _mergePanels: function(fetchedPanels, cached, changedData, keysToRemove, comparator) {
        changedData.added.panels = {};
        changedData.updated.panels = {};
        changedData.deleted.panels = {};

        var storePanels = function(location) {
            changedData.added.panels[location] = [];
            changedData.updated.panels[location] = [];
            changedData.deleted.panels[location] = [];

            _.each(fetchedPanels[location], function(fetchedPanel) {
                var existing = _.find(cached.panels[location], function(cachedPanel) {
                    return cachedPanel.id === fetchedPanel.id;
                });
                if (existing) {
                    changedData.updated.panels[location].push(fetchedPanel.id);
                } else {
                    changedData.added.panels[location].push(fetchedPanel.id);
                }
            });

            var newPanels = fetchedPanels[location];

            _.each(cached.panels[location], function(oldPanel) {
                if (_.contains(keysToRemove, oldPanel.completeKey)) {
                    changedData.deleted.panels[location].push(oldPanel.id);
                } else if (!_.contains(changedData.updated.panels[location], oldPanel.id)) {
                    newPanels.push(oldPanel);
                }
            });

            newPanels.sort(comparator);

            cached.panels[location] = newPanels;
        };

        storePanels("leftPanels");
        storePanels("rightPanels");
        storePanels("infoPanels");
    },

    _mergeLinks: function(fetchedLinkGroups, cached, changedData, keysToRemove, comparator) {
        changedData.added.groups = {};
        changedData.updated.groups = {};
        changedData.deleted.groups = {};

        var newLinkGroups = [];

        this._mergeOperationsLinks(fetchedLinkGroups, cached, newLinkGroups, changedData, keysToRemove, comparator);
        this._mergeToolsLinks(fetchedLinkGroups, cached, newLinkGroups, changedData, keysToRemove, comparator);

        cached.asset.operations.linkGroups = newLinkGroups;
    },

    _mergeOperationsLinks: function(fetchedLinkGroups, cached, newLinkGroups, changedData, keysToRemove, comparator) {
        changedData.added.groups["view.asset.opsbar"] = [];
        changedData.updated.groups["view.asset.opsbar"] = [];
        changedData.deleted.groups["view.asset.opsbar"] = [];

        var operationsGroupContainer = _.find(cached.asset.operations.linkGroups, function(group) {
            return "view.asset.opsbar" === group.id;
        });
        var fetchedOperationsGroupContainer = _.find(fetchedLinkGroups, function(group) {
            return "view.asset.opsbar" === group.id;
        });

        var fetchedLinkMapping = {};
        var fetchedLinkToGroupIdMapping = {};
        if (fetchedOperationsGroupContainer) {
            _.each(fetchedOperationsGroupContainer.groups, function(fetchedOperationsGroup) {
                var existingGroup = _.find(operationsGroupContainer.groups, function(group) {
                    return fetchedOperationsGroup.id === group.id;
                });
                if (existingGroup) {
                    changedData.updated.groups["view.asset.opsbar"].push(fetchedOperationsGroup.id);
                } else {
                    changedData.added.groups["view.asset.opsbar"].push(fetchedOperationsGroup.id);
                }

                _.each(fetchedOperationsGroup.links, function(link) {
                    fetchedLinkMapping[link.id] = link;
                    fetchedLinkToGroupIdMapping[link.id] = fetchedOperationsGroup.id;
                });

                if (fetchedOperationsGroup.groups && fetchedOperationsGroup.groups.length > 0) {
                    var dropdown = fetchedOperationsGroup.groups[0];

                    _.each(dropdown.groups, function(dropdownGroup) {
                        _.each(dropdownGroup.links, function(link) {
                            fetchedLinkMapping[link.id] = link;
                            fetchedLinkToGroupIdMapping[link.id] = dropdownGroup.id;
                        });
                    });
                }
            });
        }

        if (operationsGroupContainer) {
            var newOperationsGroups = [];

            if (fetchedOperationsGroupContainer) {
                _.each(fetchedOperationsGroupContainer.groups, function(fetchedOperationsGroup) {
                    if (_.contains(changedData.added.groups["view.asset.opsbar"], fetchedOperationsGroup.id)) {
                        // There was no such group but now it was fetched.
                        newOperationsGroups.push(fetchedOperationsGroup);
                    }
                });
            }

            _.each(operationsGroupContainer.groups, function(operationsGroup) {
                var newGroupLinks = [];

                var fetchedOperationsGroup = null;
                if (fetchedOperationsGroupContainer) {
                    fetchedOperationsGroup = _.find(fetchedOperationsGroupContainer.groups, function(group) {
                        return operationsGroup.id === group.id;
                    });

                    if (fetchedOperationsGroup) {
                        _.each(fetchedOperationsGroup.links, function(fetchedLink) {
                            var existingLink = _.find(operationsGroup.links, function(link) {
                                return fetchedLink.id === link.id;
                            });
                            if (!existingLink) {
                                // There was no such link but now it was fetched.
                                newGroupLinks.push(fetchedLink);
                            }
                        });
                    }
                }

                var groupUpdated = false;

                var checkLinks = function(group, newLinks) {
                    _.each(group.links, function(link) {
                        var fetchedGroupId = fetchedLinkToGroupIdMapping[link.id];
                        if (fetchedGroupId && fetchedGroupId !== group.id) {
                            // Link moved from another group.
                            groupUpdated = true;
                        } else if (fetchedLinkMapping[link.id]) {
                            newLinks.push(fetchedLinkMapping[link.id]);
                        } else if (!_.contains(keysToRemove, link.id)) {
                            newLinks.push(link);
                        } else {
                            // Existing link got deleted.
                            groupUpdated = true;
                        }
                    });
                };

                checkLinks(operationsGroup, newGroupLinks);

                newGroupLinks.sort(comparator);
                operationsGroup.links = newGroupLinks;

                var fetchedDropdown = null;
                if (fetchedOperationsGroup && fetchedOperationsGroup.groups && fetchedOperationsGroup.groups.length > 0) {
                    fetchedDropdown = fetchedOperationsGroup.groups[0];
                }

                var dropdown = null;
                if (operationsGroup.groups && operationsGroup.groups.length > 0) {
                    dropdown = operationsGroup.groups[0];

                    var newDropdownGroups = [];

                    if (fetchedDropdown) {
                        _.each(fetchedDropdown.groups, function(fetchedDropdownGroup) {
                            var existingDropdownGroup = _.find(dropdown.groups, function(group) {
                                return fetchedDropdownGroup.id === group.id;
                            });
                            if (!existingDropdownGroup) {
                                // There was no such dropdown group but now it was fetched.
                                newDropdownGroups.push(fetchedDropdownGroup);
                            }
                        });
                    }

                    _.each(dropdown.groups, function(dropdownGroup) {
                        var newSectionLinks = [];

                        if (fetchedDropdown) {
                            var fetchedDropdownGroup = _.find(fetchedDropdown.groups, function(fetchedDropdownGroup) {
                                return fetchedDropdownGroup.id === dropdownGroup.id;
                            });
                            if (fetchedDropdownGroup) {
                                _.each(fetchedDropdownGroup.links, function(fetchedLink) {
                                    var existingDropdownLink = _.find(dropdownGroup.links, function(link) {
                                        return fetchedLink.id === link.id;
                                    });
                                    if (!existingDropdownLink) {
                                        // There was no such dropdown link but now it was fetched.
                                        newSectionLinks.push(fetchedLink);
                                    }
                                });
                            }
                        }

                        checkLinks(dropdownGroup, newSectionLinks);

                        if (newSectionLinks.length > 0) {
                            newSectionLinks.sort(comparator);
                            dropdownGroup.links = newSectionLinks;
                            newDropdownGroups.push(dropdownGroup);
                        }
                    });

                    if (newDropdownGroups.length > 0) {
                        newDropdownGroups.sort(comparator);
                        dropdown.groups = newDropdownGroups;
                    } else {
                        dropdown = null;
                        groupUpdated = true;
                    }
                } else if (fetchedDropdown) {
                    // There was no dropdown but now it was fetched.
                    dropdown = fetchedOperationsGroup.groups[0];
                }

                operationsGroup.groups = [];
                if (dropdown) {
                    operationsGroup.groups[0] = dropdown;
                }

                if (newGroupLinks.length > 0 || dropdown) {
                    newOperationsGroups.push(operationsGroup);

                    if (groupUpdated && !_.contains(changedData.updated.groups["view.issue.opsbar"], operationsGroup.id)) {
                        changedData.updated.groups["view.issue.opsbar"].push(operationsGroup.id);
                    }
                } else {
                    changedData.deleted.groups["view.issue.opsbar"].push(operationsGroup.id);
                }
            });

            if (newOperationsGroups.length > 0) {
                newOperationsGroups.sort(comparator);
                operationsGroupContainer.groups = newOperationsGroups;
                newLinkGroups.push(operationsGroupContainer);
            }
        } else if (fetchedOperationsGroupContainer) {
            // There was no container in the cache but now it was fetched.
            newLinkGroups.push(fetchedOperationsGroupContainer);
        }

        changedData.added.groups["view.issue.opsbar"].sort();
        changedData.deleted.groups["view.issue.opsbar"].sort();
        changedData.updated.groups["view.issue.opsbar"].sort();
    },

    _mergeToolsLinks: function(fetchedLinkGroups, cached, newLinkGroups, changedData, keysToRemove, comparator) {
        changedData.updated.groups["jira.issue.tools"] = false;

        var toolsGroupContainer = _.find(cached.issue.operations.linkGroups, function(group) {
            return "jira.issue.tools" === group.id;
        });
        var fetchedToolsGroupContainer = _.find(fetchedLinkGroups, function(group) {
            return "jira.issue.tools" === group.id;
        });

        var fetchedLinkMapping = {};
        if (fetchedToolsGroupContainer) {
            // There must be just one tools top-level group.
            changedData.updated.groups["jira.issue.tools"] = true;

            _.each(fetchedToolsGroupContainer.links, function(link) {
                fetchedLinkMapping[link.id] = link;
            });

            if (fetchedToolsGroupContainer.groups && fetchedToolsGroupContainer.groups.length > 0) {
                _.each(fetchedToolsGroupContainer.groups[0].links, function(link) {
                    fetchedLinkMapping[link.id] = link;
                });
            }
        }

        if (toolsGroupContainer) {
            var newToolsLinks = [];

            if (fetchedToolsGroupContainer) {
                _.each(fetchedToolsGroupContainer.links, function(fetchedLink) {
                    var existingLink = _.find(toolsGroupContainer.links, function(link) {
                        return fetchedLink.id === link.id;
                    });
                    if (!existingLink) {
                        // There was no such link but now it was fetched.
                        newToolsLinks.push(fetchedLink);
                    }
                });
            }

            _.each(toolsGroupContainer.links, function(link) {
                if (fetchedLinkMapping[link.id]) {
                    newToolsLinks.push(fetchedLinkMapping[link.id]);
                } else if (!_.contains(keysToRemove, link.id)) {
                    newToolsLinks.push(link);
                }
            });

            newToolsLinks.sort(comparator);
            toolsGroupContainer.links = newToolsLinks;

            var fetchedViewGroup = null;
            if (fetchedToolsGroupContainer && fetchedToolsGroupContainer.groups && fetchedToolsGroupContainer.groups.length > 0) {
                fetchedViewGroup = fetchedToolsGroupContainer.groups[0];
            }

            if (toolsGroupContainer.groups && toolsGroupContainer.groups.length > 0) {
                var viewGroup = toolsGroupContainer.groups[0];

                var newViewLinks = [];

                if (fetchedViewGroup) {
                    _.each(fetchedViewGroup.links, function(fetchedLink) {
                        var existingLink = _.find(viewGroup.links, function(link) {
                            return fetchedLink.id === link.id;
                        });
                        if (!existingLink) {
                            // There was no such link but now it was fetched.
                            newViewLinks.push(fetchedLink);
                        }
                    });
                }

                _.each(viewGroup.links, function(link) {
                    if (fetchedLinkMapping[link.id]) {
                        newViewLinks.push(fetchedLinkMapping[link.id]);
                    } else if (!_.contains(keysToRemove, link.id)) {
                        newViewLinks.push(link);
                    }
                });

                toolsGroupContainer.groups = [];
                if (newViewLinks.length > 0) {
                    newViewLinks.sort(comparator);
                    viewGroup.links = newViewLinks;
                    toolsGroupContainer.groups[0] = viewGroup;
                }
            } else if (fetchedViewGroup) {
                // There was no view group in the cache but now it was fetched.
                toolsGroupContainer.groups = [fetchedViewGroup];
            }

            newLinkGroups.push(toolsGroupContainer);
        } else if (fetchedToolsGroupContainer) {
            // There was no container in the cache but now it was fetched.
            newLinkGroups.push(fetchedToolsGroupContainer);
        }
    },

    _comparator: function(objOne, objTwo) {
        if (objOne.weight && objTwo.weight) {
            if (objOne.weight > objTwo.weight) {
                return 1;
            }
            if (objOne.weight < objTwo.weight) {
                return -1;
            }
            return 0;
        }

        if (objOne.weight && !objTwo.weight) {
            return 1;
        }
        if (!objOne.weight && objTwo.weight) {
            return -1;
        }

        return 0;
    },

    updateIssue: function(key, data) {
        var cached = this.getMeta(key);
        if (cached && cached.value) {
            cached.value.fields = data.fields || {};
        }
    }
});

module.exports = ViewAssetData;
