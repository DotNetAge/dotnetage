/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoDataSource", {
        options: {
            keyField: "id",
            data: null,
            pageIndex: -1,
            pageSize: 20,
            timeout: 30000,
            serverPaging: false, // 如果此选被开启 则发送请求时会向 data 加入 index 和 size 两个参数
            serverGrouping: false,
            serverSorting: false,
            serverFiltering: false,
            actions: {
                read: null,
                insert: null,
                update: null,
                remove: null
            },
            autoLoad: false,
            mapper: null,
            master: null,
            masterKey: null,
            sort: null,
            group: null,
            filter: null,
            cache: true,
            schema: {},
            //Events
            inserted: null,
            updated: null,
            removed: null,
            changed: null,
            stateChanged: null,
            position: null,
            detail: null,
            process: null,
            complete: null,
            error: null,
            cancel: null,
            beforeMap: null,
            pagend: null,
            pagestart: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix;
            this._cursor = 0;
            this.isloading = false;
            this.keymap = {};
            this._state = "position";
            this._unobtrusive();
            if (opts.inserted) el.bind(eventPrefix + "inserted", opts.inserted);
            if (opts.updated) el.bind(eventPrefix + "updated", opts.updated);
            if (opts.stateChanged) el.bind(eventPrefix + "stateChanged", opts.stateChanged);
            if (opts.changed) el.bind(eventPrefix + "changed", opts.changed);
            if (opts.removed) el.bind(eventPrefix + "removed", opts.removed);
            if (opts.position) el.bind(eventPrefix + "position", opts.position);
            if (opts.detail) el.bind(eventPrefix + "detail", opts.detail);
            if (opts.process) el.bind(eventPrefix + "process", opts.process);
            if (opts.error) el.bind(eventPrefix + "error", opts.error);
            if (opts.cancel) el.bind(eventPrefix + "cancel", opts.cancel);
            if (opts.complete) el.bind(eventPrefix + "complete", opts.complete);
            if (opts.beforeMap) el.bind(eventPrefix + "beforeMap", opts.beforeMap);
            //            if (opts.pagend) el.bind(eventPrefix + "pagend", opts.pagend);
            //            if (opts.pagestart) el.bind(eventPrefix + "pagestart", opts.pagestart);

            if (opts.serverPaging && opts.pageIndex == -1)
                opts.pageIndex = 1;

            if (!opts.mapper)
                opts.mapper = new tao.mapper();

            if (opts.data)
                this._setData(opts.data);
            else {
                if (opts.autoLoad)
                    this.read();
            }
            el.hide();
        },
        state: function (_val) {
            if (_val) {
                //if (this._state != _val) {
                this._state = _val;
                this._triggerEvent("stateChanged", _val);
                //}
            } else {
                return this._state;
            }
        },
        _unobtrusive: function () {
            var el = this.element, self = this, opts = this.options;
            var $mapper = $("[data-role=mapper]", el);
            if ($mapper.length) opts.mapper = $mapper.mapper();
            if (el.data("key")) opts.keyField = el.data("key");
            if (el.data("pageindex") != undefined) opts.pageIndex = el.dataInt("pageindex");
            if (el.data("pagesize")) opts.pageSize = el.data("pagesize");

            if (el.data("page-index") != undefined) opts.pageIndex = el.dataInt("page-index");
            if (el.data("page-size")) opts.pageSize = el.data("page-size");

            if (el.data("server-paging") != undefined) opts.serverPaging = el.dataBool("server-paging");
            if (el.data("server-sorting") != undefined) opts.serverSorting = el.dataBool("server-sorting");
            if (el.data("server-filtering") != undefined) opts.serverFiltering = el.dataBool("server-filtering");
            if (el.data("server-grouping") != undefined) opts.serverGrouping = el.dataBool("server-grouping");
            if (el.data("autoload") != undefined) opts.autoLoad = el.dataBool("autoload");
            if (el.data("cache") != undefined) opts.cache = el.dataBool("cache");

            if (el.data("onchanged")) opts.changed = new Function("event", "results", el.data("onchanged"));
            if (el.data("ondetail")) opts.detail = new Function("event", "dataItem", el.data("ondetail"));
            if (el.data("oninserted")) opts.inserted = new Function("event", "data", el.data("oninserted"));
            if (el.data("onupdated")) opts.updated = new Function("event", "data", el.data("onupdated"));
            if (el.data("onremoved")) opts.removed = new Function("event", "data", el.data("onremoved"));
            if (el.data("onposition")) opts.position = new Function("event", "data", el.data("onposition"));
            //if (el.data("ondetail")) opts.detail = new Function("event", "data", el.data("ondetail"));
            if (el.data("onprocess")) opts.process = new Function("event", "data", el.data("onprocess"));
            if (el.data("onerror")) opts.error = new Function("event", "data", el.data("onerror"));
            if (el.data("onstate")) opts.stateChanged = new Function("event", "data", el.data("onstate"));
            if (el.data("oncancel")) opts.cancel = new Function("event", "data", el.data("oncancel"));
            if (el.data("oncomplete")) opts.complete = new Function("event", "data", el.data("oncomplete"));
            if (el.data("onmap")) opts.beforeMap = new Function("event", "data", el.data("onmap"));

            if (el.data("change")) opts.changed = new Function("event", "results", el.data("change"));
            if (el.data("detail")) opts.detail = new Function("event", "dataItem", el.data("detail"));
            if (el.data("inserted")) opts.inserted = new Function("event", "data", el.data("inserted"));
            if (el.data("updated")) opts.updated = new Function("event", "data", el.data("updated"));
            if (el.data("removed")) opts.removed = new Function("event", "data", el.data("removed"));
            if (el.data("process")) opts.process = new Function("event", "data", el.data("process"));
            if (el.data("error")) opts.error = new Function("event", "data", el.data("error"));
            if (el.data("state")) opts.stateChanged = new Function("event", "data", el.data("state"));
            if (el.data("cancel")) opts.cancel = new Function("event", "data", el.data("cancel"));
            if (el.data("complete")) opts.complete = new Function("event", "data", el.data("complete"));
            if (el.data("map")) opts.beforeMap = new Function("event", "data", el.data("map"));
            //            if (el.data("onpagend")) opts.pagend = new Function("event", "data", el.data("onpagend"));
            //            if (el.data("onpagestart")) opts.pagestart = new Function("event", "data", el.data("onpagestart"));


            //Shortcut
            if (el.data("read-url")) {
                opts.actions.read = {
                    url: el.data("read-url")
                };
            }

            if (el.data("mapper")) {
                $mapper = el.datajQuery("mapper");
                opts.mapper = $mapper.mapper();
            }

            if (el.data("master")) {
                opts.master = el.datajQuery("master");
                this._setMaster(opts.master);
            }

            var _setupAction = function (actEl) {
                var act = {};
                if (actEl.data("timeout")) act.timeout = actEl.data("timeout");
                if (actEl.data("url")) act.url = actEl.data("url");
                if (actEl.data("cache")) act.cache = eval(actEl.data("cache"));
                if (actEl.data("type")) act.dataType = actEl.data("type");
                if (actEl.data("content-type")) act.contentType = actEl.data("content-type");
                if (actEl.data("method")) act.type = actEl.data("method");
                var actname = actEl.data("action");
                if (actname) {
                    if ((actname == "insert" || actname == "update" || actname == "remove") && !act.type)
                        act.type = "POST";
                }
                return act;
            };

            var _getParams = function (actEL) {
                var _param = {}, _pELs = $(actEL).children();
                _pELs.each(function (j, p) {
                    var paramEL = $(p), name = "", value = "";
                    if (paramEL.data("name")) name = paramEL.data("name");

                    if (paramEL.data("from")) {
                        value = function () {
                            var valFrom = paramEL.datajQuery("from");
                            if (valFrom.length) {
                                if (valFrom.isInput())
                                    return valFrom.val();
                                else
                                    return valFrom.text();
                            }
                        }
                    }

                    if (paramEL.data("value")) value = paramEL.data("value");

                    if (name && paramEL.data("master-field"))
                        self.keymap = { key: name, masterKey: paramEL.data("master-field") };

                    if (name && value)
                        _param[name] = value;

                });
                return _param;
            };

            //setup actions
            var actions = $("[data-action]", el);
            actions.each(function (i, a) {
                var _act = _setupAction($(a)), _pData = _getParams($(a));
                if (!$.isEmptyObject(_pData))
                    _act.data = _pData;
                opts.actions[$(a).data("action")] = _act;
            });

            var fields = $(">[data-field]", el);
            if (opts.schema.fields == undefined)
                opts.schema.fields = [];

            //var _parsers = this._getParsers();

            fields.each(function (i, f) {
                var field = $(f), _f = {
                    name: field.data("field"),
                    title: field.data("title") ? field.data("title") : field.data("field"),
                    type: field.data("type") ? field.data("type") : "string",
                    desc: field.data("desc") ? field.data("desc") : "",
                    isKey: field.data("key") != undefined ? field.dataBool("key") : false,
                    defaultValue: field.data("default") != undefined ? field.data("default") : null,
                    watermark: field.data("watermark") ? field.data("watermark") : null,
                    isReadonly: field.data("readonly") != undefined ? field.dataBool("readonly") : false,
                    hidden: field.data("hidden") != undefined ? field.dataBool("hidden") : false,
                    validation: {
                        rules: {},
                        messages: {}
                    }
                };

                if (_f.isKey)
                    opts.keyField = _f.name;

                _f.validation = field.getValidationInfo();
                opts.schema.fields.push(_f);
            });
        },
        _setMaster: function (master) {
            var self = this, opts = this.options;
            master.bind("taoDataSourceposition", function (event, dataItem) {
                if (!$.isEmptyObject(self.keymap) && self._remoteReadable()) {
                    var ropts = self.options.actions.read;

                    if (opts.serverFiltering) {
                        var masterVal = dataItem[self.keymap.masterKey];
                        self.filter(opts.masterKey + "=" + ($.type(masterVal) == "number" ? masterVal : "\"" + masterVal + "\""));
                    }
                    else {
                        if ($.type(ropts) == "string")
                            ropts = { url: ropts };

                        if (ropts.data == undefined)
                            ropts.data = {};

                        var exDat = {};
                        exDat[self.keymap.key] = dataItem[self.keymap.masterKey];
                        $.extend(ropts.data, exDat);
                    }

                    self.read();
                } else {
                    self._triggerEvent("detail", dataItem);
                }
            });
        },
        _cache: function (uri, key, value) {
            if (!this.cache || this.cache.url != uri)
                this.cache = { url: uri, items: [] };

            var cacheCtx = this.cache, isNew = true;

            if (value == undefined) { //get item
                for (var i = 0; i < cacheCtx.items.length; i++) {
                    if (cacheCtx.items[i].key == key)
                        return cacheCtx.items[i].data;
                }
                return null;
            }
            else {
                for (var i = 0; i < this.cache.items.length; i++) { //set item
                    if (this.cache.items[i].key == key) {
                        this.cache.items[i].data = value;
                        isNew = false;
                    }
                }
            }

            if (isNew)
                this.cache.items.push({ key: key, data: value });
        },
        reset: function () {
            var opts = this.options;
            opts.pageIndex = 1;
            this.cache = null;
            this._cursor = 0;
        },
        pos: function (dataItem) {
            if (this._currentData) {
                if (this._currentData == dataItem)
                    return this;
            }

            this._currentData = dataItem;
            if ($.isArray(this.options.data))
                for (var i = 0; i < this.options.data.length; i++) {
                    if (this.options.data[i] == dataItem) {
                        this._cursor = i;
                        break;
                    }
                }
           // console.log("Cursor is move to "+this._cursor);
            this.state("position");
            this._triggerEvent("position", dataItem);
            return this;
        },
        index: function () { return this._cursor; },
        prev: function () {
            if (this.options.data) {
                if (this._cursor > 0) {
                    this._cursor--;
                    var dataItem = this.get(this._cursor);
                    if (dataItem)
                        return this.pos(dataItem);
                }
            }
            return null;
        },
        next: function () {
            if (this.options.data) {
                if (this._cursor < this.options.data.length) {
                    this._cursor++;
                    var dataItem = this.get(this._cursor);
                    if (dataItem)
                        return this.pos(dataItem);
                }
            }
            return null;
        },
        nextPage: function () {
            //This is a helper method to get the next page data
            var opts = this.options, totalpages = this.totalPages();

            if (opts.serverPaging) {
                if (opts.pageIndex < totalpages && totalpages) {
                    opts.pageIndex++;
                    return this.read();
                }
                //                else {
                //                    this._triggerEvent("pagend", { index: opts.pageIndex, totalPages: totalpages, size: opts.pageSize });
                //                }
            } else {
                // Get local paging data
            }

            return $.Deferred().resolve();
        },
        isFirstPage: function () {
            return this.options.pageIndex == 1
        },
        isLastPage: function () {
            var opts = this.options, totalpages = this.totalPages();

            if (opts.pageIndex < totalpages && totalpages)
                return false;

            return true;
        },
        read: function (data) {
            var self = this, opts = this.options, _remote = false, dfd = $.Deferred();
            if (data == undefined || data == null)
                _remote = this._remoteReadable();
            else {
                if (!$.isArray(data)) {
                    //We could be set the data to remote url or read option object.
                    if ($.type(data) == "string" || $.isPlainObject(data)) {
                        if ($.isPlainObject(data))
                            opts.actions.read = data;
                        else
                            opts.actions.read.url = data;
                        _remote = true;
                    }
                }
            }

            if (_remote) {
                return this._serverRead(dfd);
            }
            else {
                var _viewData = [];
                if (data) {
                    this._setData(data);
                    _viewData = $.makeArray(data);// $.map(data);

                    //There we need convert the data
                    if (opts.filter)
                        _viewData = this._filtering(opts.filter, _viewData);

                    if (opts.sort)
                        _viewData = this._sorting(opts.sort, _viewData);

                    if (opts.pageSize > -1)
                        _viewData = this._paging(opts.pageIndex, opts.pageSize, _viewData);

                    self._triggerEvent("changed", { data: _viewData, pageIndex: opts.pageIndex, pageSize: opts.pageSize });

                    return dfd.resolve({ data: _viewData });
                }
                else {
                    self._triggerEvent("changed", { data: opts.data, pageIndex: opts.pageIndex, pageSize: opts.pageSize });
                    return dfd.resolve({ data: opts.data });
                }
            }
        },
        _remoteReadable: function () {
            var opts = this.options;
            if (opts.actions != undefined && opts.actions != null) {
                if (opts.actions.read != undefined && opts.actions.read != null) {
                    if ($.type(opts.actions.read) == "string") {
                        return opts.actions.read != "";
                    }
                    else {
                        if (opts.actions.read.url)
                            return opts.actions.read.url != "";
                    }
                }
            }
            return false;
        },
        _serverRead: function (dfd) {
            var opts = this.options, self = this;

            //if loading wait 500ms and retry
            if (self.isloading) {
                setTimeout(function () {
                    self.isloading = false;
                    self._serverRead(dfd);
                }, 500);
                return dfd.promise();
            }

            var options = $.isPlainObject(opts.actions.read) ? opts.actions.read : { url: opts.actions.read };
            this._srvPaging(options)
                   ._srvSorting(options)
                   ._srvPaging(options)
                   ._srvFiltering(options)
                   ._srvGrouping(options);

            var uri = options.url,
            key = options.data ? $.param(options.data).toLowerCase() : "default",
            cacheItem = opts.cache ? self._cache(uri, key) : null;

            if (cacheItem) {
                //self._setData(cacheItem);
                opts.data = cacheItem;
                self._triggerEvent("changed", { options: options.data, data: cacheItem });
                return dfd.resolve({ data: cacheItem });
            }
            else {
                self.isloading = true;
                self._triggerEvent("process", "read")
                       ._setTimeout(opts.timeout);

                if (opts.timeout)
                    options.timeout = opts.timeout;

                return $.ajax(options)
                                .pipe(function (d) {
                                    self._clearTimeout();
                                    self._setData(d);

                                    var _mapper = self.options.mapper,
                                     results = _mapper ? _mapper.map(d) : d;

                                    self._total = _mapper ? _mapper.total(d) : 0;
                                    self.isloading = false;
                                    self._cache(uri, key, _mapper ? results.data : d);

                                    self._triggerEvent("changed", {
                                        type: "loaded",
                                        total: self._total,
                                        options: options.data,
                                        data: _mapper ? results.data : d
                                    });
                                    self.state("loaded");

                                    //new version

                                    d = results;
                                    return dfd.resolve(results);
                                })
                                .fail(function () {
                                    self._clearTimeout()
                                           ._triggerEvent("error", this);
                                })
                                 .always(function () {
                                     self._triggerEvent("complete");
                                 });
            }
        },
        _clearTimeout: function () {
            if (this._timer) {
                clearTimeout(this._timer);
                this._timer = null;
            }
            return this;
        },
        _setTimeout: function (val) {
            var self = this, _timeout = val == undefined ? this.options.timeout : val;
            if (_timeout) {
                self._timer = setTimeout(function () {
                    self._triggerEvent("cancel");
                    self._timer = null;
                }, _timeout);
            }
            return this;
        },
        _srvPaging: function (readOptions) {
            var self = this, opts = this.options;
            if (opts.serverPaging) {
                if (readOptions.data)
                    $.extend(readOptions.data, { index: opts.pageIndex, size: opts.pageSize });
                else
                    readOptions.data = { index: opts.pageIndex, size: opts.pageSize };
            }
            return this;
        },
        _srvSorting: function (readOptions) {
            var self = this, opts = this.options;
            if (opts.serverSorting && opts.sort) {

                var sortingExpr = "";

                if ($.isArray(opts.sort)) {
                    var exprs = [];
                    $.each(opts.sort, function (i, expr) {
                        exprs.push(expr.field + "~" + (expr.dir ? expr.dir : "asc"));
                    });
                    sortingExpr = exprs.join("-");
                }
                else {
                    var expr = opts.sort;
                    sortingExpr = expr.field + "~" + (expr.dir ? expr.dir : "asc");
                }

                if (!readOptions.data)
                    readOptions.data = {};

                $.extend(readOptions.data, { orderby: sortingExpr });
            }
            return this;
        },
        _srvFiltering: function (readOptions) {
            var self = this, opts = this.options;

            if (opts.serverFiltering && opts.filter) {
                var filterExpr = "";
                if ($.type(opts.filter) == "string") {
                    filterExpr = opts.filter;
                    filterExpr = filterExpr.replace("==", "~eq~")
                                          .replace("!=", "~neq~")
                                          .replace(">", "~gt~")
                                          .replace(">=", "~ge~")
                                          .replace("<", "~lt~")
                                          .replace("<=", "~le~")
                                          .replace(" && ", "~and~")
                                          .replace(" || ", "~and~")
                                          .replace(" !", "~not~")
                                          .replace("^=", "~startswith~")
                                          .replace("$=", "~endswith~")
                                          .replace("*=", "~contains~");

                    filterExpr = encodeURIComponent(filterExpr);
                } else {
                    if ($.isArray(opts.filter)) {
                        var builder = new tao.exprBuilder();
                        builder.addExprs(opts.filter);
                        filterExpr = encodeURIComponent(builder.getResult());
                    }
                }
                if (filterExpr) {
                    if (!readOptions.data)
                        readOptions.data = {};

                    $.extend(readOptions.data, { filter: filterExpr });
                }
            }
            return this;
        },
        _srvGrouping: function (readOptions) {
            var self = this, opts = this.options;
            if (opts.serverGrouping && opts.group) {
                var groupingExpr = "";
                if ($.isArray(opts.group)) {
                    groupingExpr = opts.group.join("-");
                } else {
                    groupingExpr = opts.group.replace(",", "-");
                }

                if (!readOptions.data)
                    readOptions.data = {};

                $.extend(readOptions.data, { groupby: encodeURIComponent(groupingExpr) });
            }
            return this;
        },
        _doPost: function (_eventName, _ajaxSettings, entity) {
            var opts = this.options, dfd = $.Deferred(), self = this;

            if (_ajaxSettings) {
                options = {};

                if ($.type(_ajaxSettings) == "string") {
                    options.url = _ajaxSettings;
                    options.type = "POST";
                } else {
                    $.extend(options, _ajaxSettings);
                }

                if (opts.timeout)
                    options.timeout = opts.timeout;

                this._setTimeout();

                if (_ajaxSettings.data) {
                    options.data = _ajaxSettings.data;
                    $.extend(options.data, entity);
                } else {
                    options.data = entity;
                }

                return $.ajax(options).done(function (result) {
                    self._clearTimeout()
                          ._triggerEvent(_eventName, _eventName == "inserted" ? result : { orginal: entity, key: opts.keyField, result: result });

                    self.state(_eventName);

                }).fail(function () {
                    self._clearTimeout()
                          ._triggerEvent("error", this);
                }).always(function () {
                    self._triggerEvent("complete");
                });
            }

            return dfd.reject();
        },
        _setData: function (_data) {
            this.options.data = _data;
            var _transform = _data ? _data : [];

            if (_transform) {
                if (this.options.mapper) {
                    var mapResult = this.options.mapper.map(_data);
                    _transform = mapResult.data;
                    this._total = mapResult.total;
                    this.options.data = _transform;
                } else
                    this._total = _transform.length;
            } else this._total = 0;

            //this._triggerEvent("changed", { data: _transform });
            return this;
        },
        _setOption: function (key, value) {
            if (key == "data") {
                this._setData(value);
                return this;
            }

            if (key == "cache") {
                if (value == false) //clear cache
                    this.cache = null;
            }

            return $.Widget.prototype._setOption.call(this, key, value);
        },
        _paging: function (index, size, data) {
            if (index > -1) { //local paging
                var skipCount = (index - 1) * size, pageData = [];
                for (var i = skipCount; i < data.length; i++)
                    pageData.push(data[i]);
                return pageData;
            }
            return data;
        },
        _sorting: function (fields, data) {
            if (data) {
                if (fields && $.isArray(data)) {
                    ///TODO:We need to support multi fields sorting.
                    var comparer = function (obj1, obj2) {
                        var val1 = obj1[fields[0].field], val2 = obj2[fields[0].field], dir = fields[0].dir;
                        if (!dir) dir = "asc";
                        if (dir == "asc")
                            return val1 > val2 ? 1 : -1;
                        else
                            return val1 > val2 ? -1 : 1;
                    };
                    return data.sort(comparer);
                }
            }
            return data;
        },
        _filtering: function (filters, data) {
            var viewData = $.makeArray(data);

            if (!filters)
                return data;

            var __applyFilter = function (filter, inputData) {
                var results = [];
                for (var i = 0; i < inputData.length; i++) {
                    var dataItem = inputData[i],
                    fieldVal = dataItem[filter.field],
                    op = filter.operator;

                    if (op == "eq") {
                        if (fieldVal == filter.val) {
                            results.push(dataItem);
                            continue;
                        }
                    }

                    if (op == "neq") {
                        if (fieldVal != filter.val) {
                            results.push(dataItem);
                            continue;
                        }
                    }
                    if (op == "lt") {
                        if (fieldVal < filter.val) {
                            results.push(dataItem);
                            continue;
                        }
                    }

                    if (op == "le") {
                        if (fieldVal <= filter.val) {
                            results.push(dataItem);
                            continue;
                        }
                    }

                    if (op == "gt") {
                        if (fieldVal > filter.val) {
                            results.push(dataItem);
                            continue;
                        }
                    }

                    if (op == "ge") {
                        if (fieldVal >= filter.val) {
                            results.push(dataItem);
                            continue;
                        }
                    }

                    if (fieldVal != "" && $.type(fieldVal) == "string") {
                        if (op == "startswith") {
                            if (fieldVal.slice(0, filter.val.length) == filter.val) {
                                results.push(dataItem);
                                continue;
                            }
                        }

                        if (op == "endswith") {
                            if (fieldVal.slice(-filter.val.length) == fieldVal.val) {
                                results.push(dataItem);
                                continue;
                            }
                        }

                        if (op == "contains") {
                            if (fieldVal.indexOf(filter.val) > -1) {
                                results.push(dataItem);
                                continue;
                            }
                        }
                    }

                }
                return results;
            };

            for (var j = 0; j < filters.length; j++) {
                var filter = filters[j];
                viewData = __applyFilter(filter, viewData);
                if (viewData.length == 0)
                    break;
            }

            return viewData;
        },
        insert: function (entity) {
            var self = this, opts = this.options, dfd = $.Deferred();

            if (entity) {
                if (this.options.actions.insert) {
                    return this._doPost("inserted", this.options.actions.insert, entity);
                }
                else {
                    if (!opts.data)
                        opts.data = [];

                    //if (this.find(entity[opts.keyField]) == null) {
                    var _entity = entity;
                    if (this.options.mapper) {
                        var tmp = [_entity];
                        tmp = this.options.mapper.map(tmp);
                        _entity = tmp.data[0];
                    }

                    opts.data.push(_entity);
                    this._triggerEvent("inserted", _entity);
                    this.state("inserted");
                    return dfd.resolve(_entity);
                    //}
                }
            }
            return dfd.reject();
        },
        update: function (entity) {
            if (entity) {
                if (this.options.actions.update) {
                    this._doPost("updated", this.options.actions.update, entity);
                }
                else {
                    var key = this.options.keyField, _entity = entity;
                    //if (key) {
                    //    if (this.options.mapper) {
                    //        var tmp = [_entity];
                    //        tmp = this.options.mapper.map(tmp);
                    //        _entity = tmp.data[0];
                    //    }

                    //    for (var i = 0; i < this.options.data.length; i++) {
                    //        if (this.options.data[i][key] == _entity[key]) {
                    //            var orginal = this.options.data[i];
                    //            this.options.data[i] = _entity;
                    //            //this._triggerEvent("stateChanged", "updated");
                    //            this.state("updated");
                    //            this._triggerEvent("updated", { orginal: orginal, result: _entity });
                    //        }
                    //    }
                    //} else {
                    var orginal = this.options.data[this._cursor];
                    this.options.data[this._cursor] = entity;
                    this._currentData = entity;
                    this.state("updated");
                    this._triggerEvent("updated", { orginal: orginal, result: entity });
                    // }
                }
                return this;
            }

            throw "Entity not found and it could not be null";
        },
        remove: function (val) {
            var entity = val == undefined ? this.get() : val, self = this, opts = this.options, dfd = $.Deferred();

            if (entity) {
                if (this.options.actions.remove) {
                    return this._doPost("removed", this.options.actions.remove, entity);
                }
                else {
                    //var _tmp = [], keyValue = entity[opts.keyField];
                    var _tmp = [];
                    $.each(opts.data, function (i, n) {
                        if (i != self._cursor)
                            _tmp.push(n);
                        //if (n[opts.keyField] != keyValue)
                        //    _tmp.push(n);
                    });

                    this.options.data = _tmp;

                    this.state("removed");
                    this._triggerEvent("removed", { orginal: entity, key: this._cursor, result: true });
                    //this._cursor = 0;
                    if (_tmp.length)
                        this.pos(_tmp[0]);
                }
                return dfd.resolve();
            }

            return dfd.reject("Could not delete the empty entity.");
        },
        removeAt: function (id) {
            var d = this.find(id);
            if (d) this.remove(d);
        },
        field: function (val) {
            if (this.options.schema) {
                if (this.options.schema.fields) {
                    for (var i = 0; i < this.options.schema.fields.length; i++) {
                        var _field = this.options.schema.fields[i];
                        if (_field.name) {
                            if (_field.name == val)
                                return _field;
                        }
                    }
                }
            }
            return null;
        },
        get: function (index) {
            if (index == undefined) {
                if (this._currentData)
                    return this._currentData;
                return this.options.data[this._cursor];
            } else {
                if ($.isNumeric(index)) {
                    this._cursor = index;
                    return this.options.data[index];
                }
                else {
                    if ($.isPlainObject(index)) {
                        var key = this.keyField;
                        for (var i = 0; i < this.options.data.length; i++) {
                            if (this.options.data[i][key] == index[key]) {
                                this._cursor = i;
                                return this.options.data[i];
                            }
                        }
                    }
                }
                return null;
            }
        },
        page: function (index) {
            var opts = this.options;
            opts.pageIndex = index;
            return this.read();
        },
        find: function (val) {
            ///<summary>Find data by input val, if the key not set this method maybe return more then one match results.</summary>
            ///<remark>Only availdable for local mode</remark>
            var key = this.options.keyField, _findResult = null;

            if (key) {
                $.each(this.options.data, function (i, n) {
                    if (n[key] == val) {
                        _findResult = n;
                        return n;
                    }
                });
            } else {
                if ($.isPlainObject(val)) {
                    var _results = [];

                    for (var i = 0; i < this.options.data.length; i++) {
                        var _isMath = true, b = this.options.data[i];
                        for (var v in val) {
                            if (val[v] != b[v]) {
                                _isMath = false;
                                break;
                            }
                        }
                        if (_isMath)
                            _results.push(b);
                    }
                    return _results;
                } else {
                    for (var i = 0; i < this.options.data.length; i++) {
                        if (this.options.data[i] == val)
                            return this.options.data[i];
                    }
                }
            }
            return _findResult;
        },
        filter: function (filters) {
            var opts = this.options;
            opts.filter = filters;
            if (opts.serverFiltering) {
                return this.read();
            }
            else {
                var viewData = [];
                if (opts.data)
                    viewData = this._filtering(opts.filter, opts.data);
                this._triggerEvent("changed", { data: viewData, type: "filter" });
                return $.Deferred().resolve({ data: viewData });

            }
        },
        sort: function (fields) {
            var opts = this.options;
            opts.sort = fields;
            if (this.options.serverSorting) { //server sorting
                return this.read();
            }
            else { //local sorting
                var sortData = this._sorting(fields, opts.data);
                this._triggerEvent("changed", { data: sortData, pageIndex: opts.pageIndex, pageSize: opts.pageSize });
            }
        },
        group: function (val) {
            var opts = this.options;
            opts.group = val;
            if (opts.serverGrouping) {
                return this.read();
            }
            else {
            }
        },
        total: function () {
            if (this._total)
                return this._total;
            return 0;

        },
        totalPages: function () { /*return total pages*/
            var _total = this.total();
            if (_total)
                return Math.ceil(_total / this.options.pageSize);
            return 0;
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        save: function (val) {
            if (localStorage) {
                if (this.options.data)
                    localStorage[val] = this.options.data;
            }
        },
        load: function (val) {
            if (localStorage) {
                if (localStorage[val])
                    this._read(localStorage[val]);
            }
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);


