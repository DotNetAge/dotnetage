/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($, window, document, undefined) {

    $.extend(String.prototype, {
        startsWith: function (val) {
            if ($.type(this) == "string")
                return this.slice(0, val.length) == val;
            return false;
        },
        endsWith: function (val) {
            if ($.type(this) == "string")
                return this.slice(-val.length) == val;
            return false;
        },
        contains: function (val) {
            if ($.type(this) == "string")
                return this.indexOf(val) > -1;
            return false;
        },
        isUrl: function () {
            //var regexp = /(ftp|http|https):\/\/(\w+:{0,1}\w*@@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@@!\-\/]))?/
            var regexp = /^(http[s]?:\/\/){0,1}(www\.){0,1}[a-zA-Z0-9\.\-]+\.[a-zA-Z]{2,5}[\.]{0,1}/;
            return regexp.test(this);
        }
    });

    tao = {};

    tao.mapper = function (schema, filter) {
        if (schema != undefined) {
            this._init(schema);
            if (filter != undefined)
                this.filter = filter;
        }
    };

    tao.mapper.prototype = {
        schema: {},
        _init: function (schema) {
            if (schema)
                $.extend(this.schema, schema);
        },
        data: function (data) {
            if ($.isArray(data))
                return data;
            if (data.Model != undefined)
                return data.Model; //Only for DNA
            return data;
        },
        total: function (data) {
            if ($.isArray(data))
                return data.length;

            if (data.Total != undefined)
                return data.Total;
        },
        filter: null,
        convert: function (data) {
            var self = this;
            if (this.schema) {
                if (!$.isEmptyObject(this.schema)) {
                    if ($.isArray(data)) {
                        var schema = this.schema, target = [];

                        $.each(data, function (i, dat) {
                            var o = {};
                            for (var field in schema)
                                o[field] = dat[schema[field]];

                            if ($.isFunction(self.filter)) {
                                var filterResult = self.filter(dat, o);
                                if (filterResult == undefined || filterResult == true)
                                    target.push(o);
                            } else
                                target.push(o);
                        });
                        return target;
                    }
                }
            }
            return data;
        },
        map: function (data) {
            var model = $.isFunction(this.data) ? this.data(data) : data;
            var total = $.isFunction(this.total) ? this.total(data) : 0;

            if ($.isFunction(this.convert))
                model = this.convert(model);

            return {
                total: total,
                data: model
            };
        },
        setup: function (element) {
            //read the mapper settings from html element
            var el = $(element);
            //setup schema
            var _root = $("[data-role='schema']", element);
            if (_root.length)
                this._setupSchema(_root);

            this._setupMethods(element);
            return $(element);
        },
        _setupSchema: function (element) {
            var _schema = {},
                fields = $("[data-to]", element);

            if (fields.length == 0) { //old version
                fields = $("[data-field]", element);
                fields.each(function (i, f) {
                    var fieldName = $(f).data("field"), mapTo = $(f).text();
                    if (fieldName && mapTo)
                        _schema[$.trim(fieldName)] = $.trim(mapTo);
                });
            } else {         //new usage
                fields.each(function (i, f) {
                    var fieldName = $(f).data("to"), mapTo = $(f).data("from");
                    if (fieldName && mapTo)
                        _schema[$.trim(fieldName)] = $.trim(mapTo);
                });
            }

            this.schema = _schema;
        },
        _setupMethods: function (el) {
            //setup methods
            if (el.data("filter")) this.filter = new Function("dataItem", "output", el.data("filter"));
            if (el.data("data")) this.data = new Function("data", el.data("data"));
            if (el.data("total")) this.total = new Function("data", el.data("total"));
            if (el.data("convert")) this.convert = new Function("data", el.data("convert"));
        }
    };

    $.fn.mapper = function () {
        var T = new tao.mapper();
        if (this.data("instance"))
            return this.data("instance");

        if (this.data("type")) {
            var _type = this.data("type");
            if ($.isFunction(eval(_type)))
                T = eval("new " + _type + "()");
        }

        T.setup(this);
        this.data("instance", T);
        this.hide();
        return T;
    }

    tao.xmlMapper = function (schema) { this._init(schema); };

    $.extend(tao.xmlMapper.prototype, tao.mapper.prototype);

    $.extend(tao.xmlMapper.prototype, {
        schema: {
            tagName: "node",
            fields: [
                    { name: "text", role: "attribute", ref: "text" },
                    { name: "img", role: "attribute", ref: "imageUrl" },
                    { name: "link", role: "attribute", ref: "navigateUrl" },
                    { name: "rel", role: "attribute", ref: "rel" },
                    { name: "url", role: "attribute", ref: "dataUrl" },
                    { name: "disabled", role: "attribute", ref: "disabled" },
                    { name: "checked", role: "attribute", ref: "checked" },
                    { name: "expanded", role: "attribute", ref: "expanded" }
            ]
        },
        _getTypedValue: function (value) {
            if (value != undefined) {
                if ($.isNumeric(value)) {
                    if (value.indexOf(".") > -1)
                        return parseFloat(value);
                    else
                        return parseInt(value);
                }

                if (value.indexOf("/Date(") > -1)
                    return new Date(parseInt(value.substring(6, 19)));

                if (value.toLowerCase() == "true") return true;
                if (value.toLowerCase() == "false") return false;
                return value;
            }
            return "";
        },
        _mapNode: function (node) {
            var nodeData = {}, self = this;
            $.each(this.schema.fields, function (i, field) {
                var fieldName = field.name.toString();
                if (field.role == "attribute") {
                    if (node.attr(field.ref))
                        nodeData[fieldName] = self._getTypedValue(node.attr(field.ref));
                }
                else {
                    if (field.role == "element")
                        nodeData[fieldName] = node.text();

                    else
                        nodeData[fieldName] = node[0].tagName;
                }
            });
            return nodeData;
        },
        convert: function (data) {
            var self = this, _parseChildren = function (nodes, parentNode) {
                nodes.each(function (i, node) {
                    var ndat = self._mapNode($(node));
                    if ($.isArray(parentNode))
                        parentNode.push(ndat);
                    else {
                        if (parentNode.children == undefined)
                            parentNode.children = new Array();
                        parentNode.children.push(ndat);
                    }
                    var _cns = self.schema.tagName ? $(node).children(self.schema.tagName) : $(node).children();
                    if (_cns.length)
                        _parseChildren(_cns, ndat);
                });
            };

            if (data) {
                var result = [];
                _parseChildren($(data.documentElement).children(), result);
                return result;
            }

            return data;
        }
    });

    tao.htmlSelectMapper = function () { };

    $.extend(tao.htmlSelectMapper.prototype, tao.mapper.prototype, {
        convert: function (data) {
            var target = [];
            if (data) {
                var popuplateDataAttrs = function (element, dataObj) {
                    for (var i = 0; i < element.attributes.length; i++) {
                        if (element.attributes[0].name.startsWith("data-")) {
                            _name = element.attributes[0].name.replace("data-", "");
                            dataObj[_name] = element.attributes[0].value;
                        }
                    }
                },
                    readOptionData = function (element) {
                        var el = $(element),
                            optionObj = {
                                label: el.text(),
                                selected: el.attr("selected") != undefined ? el.attr("selected") : false,
                                value: el.attr("value") != undefined ? el.attr("value") : el.text()
                            };
                        popuplateDataAttrs(element, optionObj);
                        return optionObj;
                    },
                    readOptionGroupData = function (element) {
                        var group = [];
                        group.label = $(element).attr("label");
                        popuplateDataAttrs(element, group);
                        $(element).children().each(function (i, opt) {
                            group.push(readOptionData(opt));
                        });
                        return group;
                    },
                    groups = 0;

                $(data).children().each(function (i, opt) {
                    if (opt.tagName.toLowerCase() == "option")
                        target.push(readOptionData(opt));
                    else {
                        target.push(readOptionGroupData(opt));
                        groups++;
                    }
                });

                if (groups)
                    target.groups = groups;
            }
            return target;
        }
    });

    tao.htmlTableMapper = function () { };

    $.extend(tao.htmlTableMapper.prototype, tao.mapper.prototype, {
        convert: function (data) {
            if ($.isArray(data) || $.isPlainObject(data))
                return data;

            var headers = $(">thead th,>thead td", $(data)), self = this;
            this.schema = {
                columns: []
            };
            var target = [];

            headers.each(function (i, n) {
                self.schema.columns.push({
                    name: $(n).attr("data-field") ? $(n).attr("data-field") : $.trim($(n).text()),
                    title: $.trim($(n).text()),
                    width: $(n).data("width") != undefined ? $(n).dataInt("width") : 0,
                    type: $(n).attr("data-type") ? $(n).attr("data-type") : "string"
                });
            });

            var cols = this.schema.columns,
                rows = $(data).children(":not(thead)").find("tr");
            rows.each(function (i, n) {
                var row = $(n);
                if (row.parent()[0].tagName.toLowerCase() != "thead") {
                    var dataItem = {};

                    $(">td", row).each(function (j, cell) {
                        var orgVal = $.trim($(cell).html());
                        var formatVal = null;

                        if (!isNaN(parseFloat(orgVal)))
                            formatVal = parseFloat(orgVal);

                        if (orgVal == "True" || orgVal == "true")
                            formatVal = true;

                        if (orgVal == "False" || orgVal == "false")
                            formatVal = false;

                        if (formatVal == null)
                            formatVal = orgVal;

                        if (cols[j]) {
                            dataItem[cols[j].name] = formatVal;
                            if (cols[j].type == undefined)
                                $.extend(cols[j], { type: $.type(formatVal) });
                        }
                    });

                    target.push(dataItem);
                }
            });
            return target;
        }
    });

    tao.exprBuilder = function () {
        this._init();
    };

    tao.exprBuilder.prototype = {
        _init: function () {
            this.exprs = [];
        },
        getResult: function () {
            var exprStrs = [], self = this;
            $.each(this.exprs, function (i, expr) {
                exprStrs.push(self._exprStr(expr));
            });
            return exprStrs.join("-");
        },
        _addExpr: function (field, expr, val) {
            var _val = val;
            if (val && ($.type(val) == "string" || $.type(val) == "date"))
                _val = encodeURIComponent("\"" + val + "\"");

            this.exprs.push({
                field: field,
                operator: expr,
                val: _val
            });
            return this;
        },
        _exprStr: function (expr) {
            var formatExpr = expr.operator.replace("==", "~eq~")
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

            return expr.field + "~" + formatExpr + "~" + expr.val;
        },
        addExprs: function (val) {
            if ($.isArray(val))
                this.exprs = val;
        },
        eq: function (field, val) { // expr : =
            return this._addExpr(field, "eq", val);
        },
        neq: function (field, val) { // expr : !=
            return this._addExpr(field, "neq", val);
        },
        lt: function (field, val) { // expr : <
            return this._addExpr(field, "lt", val);
        },
        le: function (field, val) { // expr : <=
            return this._addExpr(field, "le", val);
        },
        gt: function (field, val) { // expr : >
            return this._addExpr(field, "gt", val);
        },
        ge: function (field, val) { // expr: >=
            return this._addExpr(field, "ge", val);
        },
        startswith: function (field, val) {
            return this._addExpr(field, "startswith", val);
        },
        endswith: function (field, val) {
            return this._addExpr(field, "endswith", val);
        },
        contains: function (field, val) {
            return this._addExpr(field, "contains", val);
        },
        and: function () {
            return this._addExpr("", "and", "");
        },
        or: function () {
            return this._addExpr("", "or", "");
        },
        not: function () {
            return this._addExpr("", "not", "");
        }
    };

    /// Helper methods

    $.fn.isOverflow = function () {
        var $this = $(this);
        var $children = $this.children(); //$this.find('*');
        var len = $children.length;

        if (len) {
            var maxWidth = 0;
            var maxHeight = 0
            $children.map(function () {
                maxWidth = Math.max(maxWidth, $(this).outerWidth(true));
                maxHeight = Math.max(maxHeight, $(this).outerHeight(true));
            });

            return maxWidth > $this.width() || maxHeight > $this.height();
        }

        return false;
    };

    $.fn.isVisible = function (val) {
        if (val == undefined) {
            return $(this).is(":visible");
        } else {
            if (val)
                $(this).show();
            else
                $(this).hide();
        }
    };

    $.fn.isError = function (val) {
        if (val != undefined) {
            if (val) $(this).addClass("d-state-error");
            else
                $(this).removeClass("d-state-error");
            return $(this);
        }
        return $(this).hasClass("d-state-error");
    };

    $.fn.isDisable = function (val) {
        if (val != undefined) {
            if (val) $(this).addClass("d-state-disable");
            else
                $(this).removeClass("d-state-disable");
            return $(this);
        }
        return $(this).hasClass("d-state-disable");
    };

    $.fn.isHover = function (val) {
        var self = $(this);
        if (val != undefined) {
            if (val) {
                if (!self.isDisable() && !self.isError())
                    self.addClass("d-state-hover");
            }
            else
                self.removeClass("d-state-hover");
        }
        return self.hasClass("d-state-hover");
    };

    $.fn.isActive = function (val) {
        var self = $(this);
        if (val != undefined) {
            if (val) {
                if (!self.isDisable() && !self.isError())
                    self.addClass("d-state-active");
            }
            else
                self.removeClass("d-state-active");
        }
        return self.hasClass("d-state-active");
    };

    $.fn.isReadonly = function (val) {
        var self = $(this);
        if (val != undefined) {
            if (val) self.addClass("d-state-readonly");
            else
                self.removeClass("d-state-readonly");
            return self;
        }

        if (self.attr("readonly") != undefined)
            return self.attr("readonly") == true || self.attr("readonly") == "readonly";

        return self.hasClass("d-state-readonly");
    };

    $.fn.scrollEnd = function (options) {
        var self = this, doCallback = function () {
            if (options) {
                if ($.isFunction(options))
                    options();
                else {
                    if ($.isPlainObject(options)) {
                        if ($.isFunction(options.callback))
                            options.callback();
                    }
                }
            }
        }, _children = ":first";

        if (options) {
            if (options.children)
                _children = options.children;
        }

        if (this.is(document)) {
            $(document).bind("scroll", function (event) {
                var offset = (($(document).height() - $(window).height()) / 2) + ($(window).height() / 3);
                if ($(window).scrollTop() >= offset || $(window).scrollTop() > ($(document).height() - $(window).height() - 5)) {
                    doCallback();
                }
            });
        } else {
            this.bind("scroll", function (event) {
                var st = self.scrollTop(),
                                ph = self.height(),
                                ch = self.find(_children + ":first").height();

                if ((ch - st) <= ph)
                    doCallback();
            });
        }
        return this;
    };

    $.preload = function (href) {
        var _rel = ($.browser.safari || $.browser.webkit) ? "preload" : "prefetch";
        if (href) {
            if ($("link[href='" + href + "']").length == 0) {
                return $("<link/>").attr("href", href)
                                                    .attr("rel", _rel)
                                                   .appendTo($("head"));
            }
        }
    }

    $.fn.preload = function () {
        var _rel = ($.browser.safari || $.browser.webkit) ? "preload" : "prefetch", _result = [];

        this.each(function (i, link) {
            _href = $(link).attr("href");
            if (_href) {
                if (_href != "#" && _href.indexOf("javascript:") == -1) {
                    if ($("link[href='" + _href + "']").length == 0) {
                        _result.push($("<link/>").attr("href", _href)
                                               .attr("rel", _rel)
                                               .appendTo($("head")));

                    }
                }
            }
        });

        return _result;
    };

    $.fn.datajQuery = function (key, parent) {
        var el = this, selector = el.data(key);

        if (selector == "body" || selector == "document" || selector == "window")
            return $(selector);

        if (selector == "self")
            return el;

        if (selector == "prev")
            return el.prev();

        if (selector == "next")
            return el.next();

        if (selector == "parent")
            return el.parent();

        if (selector[0] == "." || selector[0] == ">" || selector[0] == "#")
            return parent ? $(selector, $(parent)) : $(selector);

        return parent ? $("#" + selector, $(parent)) : $("#" + selector);
    };

    $.fn.isEmpty = function () {
        if (this.isInput())
            return this.val() == "" || this.val() == undefined || this.val() == null;
        return true;
    };

    $.fn.dataBool = function (key) {
        var val = this.data(key);

        if (val != undefined && val != null) {
            if ($.type(val) == "boolean")
                return val;

            if ($.type(val) == "string")
                return val.toLowerCase() == "true" ? true : false;
        }

        return false;
    };

    $.fn.dataInt = function (key) {
        if (this.data(key) == undefined)
            return 0;
        return parseInt(this.data(key));
    };

    $.fn.isInput = function () {
        var _tName = this[0].tagName.toLowerCase();
        if (_tName == "input" || _tName == "select" || _tName == "textarea")
            return true;
        return false;
    };

    $.fn.canFocus = function () {
        var _tName = this[0].tagName.toLowerCase();
        if (_tName == "input" || _tName == "select" || _tName == "textarea" || this.attr("tabIndex") != undefined) {
            if (this.isVisible() && !this.isReadonly() && !this.isDisable()) {
                return true;
            }
        }
        return false;
    };

    $.notify = function (opts, cls) {
        var container = $("<div/>").addClass("d-ui-widget d-notices").appendTo("body"),
            defaults = {
                title: "Information",
                message: "",
                modal: true,
                image: null,
                width: 300,
                icon: "d-icon-info",
                notifyClass: "d-state-info",
                close: true
            };

        if ($.isPlainObject(opts)) {
            $.extend(defaults, opts);
        } else {
            defaults.message = opts;
            if (cls)
                defaults.notifyClass = cls;
        }

        var overlay = null;

        if (defaults.modal)
            overlay = $("<div/>").addClass("d-overlay")
                 .css({ "z-index": $.topMostIndex() })
                 .height($(document).height())
                 .appendTo("body");
        //.click(function () {
        //    overlay.remove();
        //    container.remove();
        //});

        container.addClass(defaults.notifyClass);
        container[0].close = function () {
            //container.animate({ top: ($(window).height()) + "px", "opacity": "0" }, 300, function () { container.remove(); });
            // container.trigger("close");
            container.remove();
            if (overlay)
                overlay.remove();
        };

        if (defaults.close)
            $("<a/>").addClass("d-item-close d-icon-cross-3")
                           .attr("href", "javascript:void(0);")
                           .appendTo(container)
                           .bind("click", function () {
                               container[0].close();
                           });

        if (defaults.image)
            $("<div/>").addClass("d-notices-image")
                              .append($("<img/>").attr("src", defaults.image))
                              .appendTo(container);

        if (defaults.title)
            $("<div/>").text(defaults.title)
                              .addClass("d-item-heading")
                              .appendTo(container);

        if (defaults.message) {
            var msgBody = $("<div/>").addClass("d-item-desc").appendTo(container);
            if (defaults.message.jquery)
                msgBody.append(defaults.message);
            else
                msgBody.text(defaults.message);
        }

        if (defaults.icon)
            $("<span/>").addClass("d-notices-icon")
                                 .addClass(defaults.icon)
                                 .appendTo(container);
        //console.log(defaults.width);
        if (defaults.width)
            container.width(defaults.width);

        container.css({
            "z-index": $.topMostIndex() + 1
            //,"opacity": 0
        })
        //    .position({
        //    of: $(window), //overlay ? overlay : $(document),
        //    at: "center top",
        //    my: "center top",
        //})
                        .position({
                            of: $(window), // overlay ? overlay : $(document),
                            at: "center center",
                            my: "center center"
                            //,using: function (using) {
                            //    $.extend(using, { opacity: 1 });
                            //    $(this).stop(true, false).animate(using, 300);
                            //}
                        });

        return container;
    };

    $.info = function (msg, title, timeout) {
        var n = $.notify(msg, title);
        setTimeout(function () {
            if (n && n[0])
                n[0].close();
        }, timeout != undefined ? timeout : 5000);
    }

    $.err = function (msg, title) {
        $.notify({
            title: title ? title : "Error",
            message: msg,
            icon: "d-icon-cancel",
            notifyClass: "d-state-error"
        });
    };

    $.warn = function (msg) {
        $.notify(msg, "d-state-warning");
    };

    $.loading = function (msg, title) {
        var loader = $("body").data("loader");
        if ("hide" == msg && loader && loader.length)
            loader[0].close();
        else {
            if (loader && loader.length)
                loader[0].close();

            var defaultTitle = $("body").data("loading") ? $("body").data("loading") : "Loading";
            if (title)
                defaultTitle = title;

            $("body").data("loader", $.notify({
                title: defaultTitle,
                message: msg ? msg : "Please wait ...",
                close: false,
                icon: "d-icon-loading"
            }));
        }
    }

    $.fn.loading = function (msg) {
        if (msg == 'hide') {
            if ($(this).find(".d-overlay").length)
                $(this).find(".d-overlay").remove();
            $(this).find(".d-overlay-loadingholder").remove();
            return this;
        }

        if ($(this).find(".d-overlay").length)
            return this;

        var _imgdat = "data:image/gif;base64,R0lGODlhEAAQAPMPAN3d3bu7u5mZmXd3d1VVVTMzMxEREQAAAO7u7qqqqoiIiGZmZkRERCIiIgARAAAAACH/C05FVFNDQVBFMi4wAwEAAAAh+QQFBwAPACwAAAAAEAAQAAAEcfDJh+gideoHGkDHAVCbBBwN4ojIqAGjcigrnEkIM4MOQXymDEDnCLhwQBwiUTjiNAqGT1CiIK5RBoNaul6tgcEtqEEoDAxnyzgZNARmBQyQoLA5I0AhkBAA5E8TCAQDDwJUAHAlAAMjhxIBYyUBAVURACH5BAUHAA8ALAAAAAAPABAAAARd8Mn5xpKITiabBEaiSQXSOEBzBKMkNIZwcBMRI0ZhHUeWSYRDATAJEFu01iOxGAxYyoeAsFhAox/B6DdJMAhbwE/BSCAECYD6KNYsAAHzVTPQxiVzjCBzfxyVaiMRACH5BAUHAA8ALAAAAAAQAA8AAARa8Mn5hKJYLskaQkUmOQTSIYSIVEfSJRMQSEORgIyiFAegHD2NIRUTAA+zSWCxUh4ukoBOAMh8romBVsR9wLqUhaLZFQwCiAD6qpIoAHAEgIwRzOAPOfeLz1MiACH5BAUHAA8ALAAAAAAQABAAAARd8MkpBb1yIETYJgH2FFqHLBgyGUEXHCgCSMoCIMwgCMyBCAeD6jEg3SaBhoh4AcBuiIAgkRAtDAYHaCeYXRaHcIESagLOQ4DCskR2Zd7lMLoZtgOzTVvi1e8nfhcRACH5BAUHAAgALAAAAAAQAA4AAARGEMmJxqBYEnkOz9zWIQYoDUfQCUdJEUaAHIXlVaN2bFh3CBgepjABCI6ggi9muWQMvpsEgFERMcCqj4rgmnwZLyUgnJomEQAh+QQFBwAOACwAAAAAEAAQAAAEbNDJ6UKg2KGEkBrdAGTIwCmK14wZsAAoYChYkACeYA0FEjQMiU8RQEwQAMKhwJIAEhgEQ9DpAK5GjIBAYCiu2Iyg0TAsKNkJoCCIXinPwyEoxFU7hkPyYEhnEgcEElBRFA0HFyQYAAdtGY9+EQAh+QQFBwAPACwAAAAAEAAQAAAEX/DJ+QCgWAaEhOBCJgkAknwCI0qKSTIJZnFJYAmLxOQSYGcLAwGDuGAWCY6SI0osFoSWsskoFAarCSGJyGoOh0VXNFg0DojFobAyHBQHrE+EcBuNdMfB4oitEAQFYxQRACH5BAUHAA4ALAEAAQAPAA8AAARa0EmAUAg1yS1Dx8HCcWAALJ6DqBvgAsmAAIRSzes4FMOYc4rUaBgYGDXDTYhQS0oQqOEM8AQ0Dr3R4aBQMBoIxYEBxDYaAu4QsAUYDIhCMnHAvGcNYWtDSEYAACH5BAUHAA8ALAAAAAAQABAAAARg8Mn5EKJYWgSADVfmcR4ghBhHIgqQbXCQSIqQpcJi35jg8hiAYgicAAbImSTQCGBaFADjcNhNGAaFxnEoAA4NQWLBQAgaC4nASSsUElXJj9JAuBEEB8/haRQkDDwhSDcRACH5BAUHAA4ALAAAAQAQAA8AAARc0MnpkEWAamll2Nr1bcB1AZkjjOCUKAGipRTrLEa+gEkfFIcgAyQQwDAAgYEmEbQcsQliUXBKGocFosHoKQiIAGEgSdAEBEbAkKiACog0YlAABRoSxvDG3CgUIBEAIfkEBQcADwAsAAABABAADwAABF3wyUkfqngiUAHHwEUdh4KcqIQEX0Ae3wSw1XAknSg1fPYEQADBYTAMMsDEB5FgYHAVAikmUSygj8JhgGAQgIKFZCCQBCSC22ABKPgeBgZijRA4QfcF4XR8PwRlGBEAIfkEBQcADwAsAAABAA8ADwAABF7wyWCAvPiG07KXxJF8UvcAx+EByNmkwnghLCY4QYbsiFIwDIWHBgAMGkihrsjaBQie3GxwMHgSisAupEAsBsWEgBY7TQwBBXhhebQkiMKiN0Ak6p6FRL0TkCQJMhgRADs=",
        _img = $("<img/>").attr("src", _imgdat).addClass("d-overlay-loadingholder");

        $(this).css("position", "relative");

        var overlay = $("<div/>").addClass("d-overlay")
                 .css({ "z-index": $.topMostIndex() })
                 .height($(this).outerHeight(true))
                 .width($(this).outerWidth(true))
                 .appendTo(this);

        _img.appendTo(this)
                .css({
                    "z-index": $.topMostIndex() + 1,
                    "position":"absolute"
                })
                .position({
                    of: $(this),
                    at: "center center",
                    my: "center center"
                });
        return this;
    }

    $.fn.getValidationInfo = function () {
        ///<summary>This method will be parse element attributes and get rules and messages option for validation</summary>
        if (!$.validator.methods.regex)
            $.validator.addMethod("regex", function (value, element, params) {
                var match;
                if (this.optional(element)) {
                    return true;
                }

                match = new RegExp(params).exec(value);
                return (match && (match.index === 0) && (match[0].length === value.length));
            });

        if (window._validation_parsers == undefined) {
            var _parsers = [];
            _parsers.addBool = function (_name) {
                this.push({
                    name: _name,
                    parse: function (element, options) {
                        if (element.attr("data-val-" + _name) != undefined) {
                            options.rules[_name] = true;
                            options.messages[_name] = element.attr("data-val-" + _name);
                        }
                    }
                });
                return this;
            };
            _parsers.addSingle = function (_name, _param) {
                this.push({
                    name: _name,
                    parse: function (element, options) {
                        if (element.attr("data-val-" + _name) != undefined) {
                            options.rules[_name] = element.attr("data-val-" + _param);
                            options.messages[_name] = element.attr("data-val-" + _name);
                        }
                    }
                });
                return this;
            };
            _parsers.addRange = function (_name, _minRole, _maxRole, _rangeRole) {
                this.push({
                    parse: function (element, options) {
                        if (element.attr("data-val-" + _name) != undefined)
                            var _min = element.attr("data-val-" + _name + "-min"), _max = element.attr("data-val-" + _name + "-max");

                        if (_min && _max) {
                            options.rules[_rangeRole] = [_min, _max];
                            options.messages[_rangeRole] = element.attr("data-val-" + _name);
                        } else {
                            if (_min != undefined) {
                                options.rules[_minRole] = _min;
                                options.messages[_minRole] = element.attr("data-val-" + _name);
                            }
                            else {
                                if (_max != undefined) {
                                    options.rules[_maxRole] = _max;
                                    options.messages[_maxRole] = element.attr("data-val-" + _name);
                                }
                            }
                        }
                    }
                });

                return this;
            };
            _parsers.parse = function (element) {
                var results = { rules: {}, messages: {} };
                for (var i = 0; i < this.length; i++) {
                    this[i].parse(element, results);
                }
                return results;
            };
            _parsers.addSingle("accept", "exts").addSingle("regex", "pattern")
                                 .addBool("creditcard").addBool("date").addBool("dateISO").addBool("digits").addBool("email").addBool("number").addBool("url").addBool("required")
                                 .addRange("length", "minlength", "maxlength", "rangelength").addRange("range", "min", "max", "range")
                                 .push({
                                     name: "equalTo",
                                     parse: function (element, options) {
                                         if (element.attr("data-val-equalto")) {
                                             options.rules.equalTo = new Function(element.data("val-other"));
                                             options.messages.equalTo = element.data("val-equalto");
                                         }
                                     }
                                 });
            _parsers.push({
                name: "remote",
                parse: function (element, options) {
                    if (element.attr("data-val-remote")) {
                        options.rules.remote = {
                            url: element.data("val-remote-url") ? element.data("val-remote-url") : "",
                            type: element.data("val-remote-type") || "GET",
                            data: {}
                        };
                        options.messages.remote = element.data("val-remote");
                    }
                }
            });

            window._validation_parsers = _parsers;
        }
        return window._validation_parsers.parse(this);
    };

    $.confirm = function (message, title, okText, cancelText) {
        var dfd = $.Deferred();

        //var confirmEle = $("<div/>").attr("title", title ? title : "Confirm").appendTo("body");
        ////confirmEle.append($("<h2/>").text(title ? title : "Confirm").css("margin-top", "0px"));
        //confirmEle.append($("<div/>").css({ "text-indent": "10px", "min-height": "180px" }).text(message));

        var confirmEle = $("<div/>").text(message),
            buttons = $("<div/>").css({ "padding": "10px", "min-height": "24px" })
                                              .appendTo(confirmEle)
                                              .append($("<button/>").text(cancelText ? cancelText : "Cancel")
                                                                                    .width(75)
                                                                                    .click(function () {
                                                                                        dfd.reject();
                                                                                        _notifier.remove();
                                                                                        $(".d-overlay:first", "body").remove();
                                                                                    })
                                                                                    .css("float", "right")
                                                                                    .taoButton())
                                              .append($("<button/>").text(okText ? okText : "OK").width(75)
                                                                                    .attr("data-default", true)
                                                                                    .click(function () {
                                                                                        $(".d-overlay:first", "body").remove();
                                                                                        dfd.resolve();
                                                                                        _notifier.remove();
                                                                                    })
                                                                                    .css({ "float": "right", "margin-right": "5px" })
                                                                                    .taoButton());
        var _notifier = $.notify({
            title: title ? title : "Question",
            icon: "d-icon-question-sign",
            message: confirmEle,
            close: false
        });

        //notifier.bind("close", function () { dfd.resolve(); });
        //confirmEle.dialog({
        //    autoOpen: true,
        //    modal: true,
        //    width: 400,
        //    //height: 300,
        //    resizable: false,
        //    close: function () {
        //        confirmEle.remove();
        //        dfd.reject();
        //    }
        //});

        return dfd;
    };

    $.topMostIndex = function () {
        var maxZ = Math.max.apply(null, $.map($('body > *'), function (e, n) {
            return parseInt($(e).css("z-Index")) || 1;
        })
        );
        return maxZ;
    };

    $.fn.fitSize = function (callback) {
        var _imgWidth = this[0].width, container = this.parent(),
        _imgHeight = this[0].height,
                    _wrapperWidth = container.width(),
                    _wrapperHeight = container.height(),
                    ratio = Math.min(_wrapperWidth / _imgWidth, _wrapperHeight / _imgHeight);

        $(this).width(0)
                  .height(0)
                  .show()
                  .stop(true, false).animate({
                      height: (_imgHeight * ratio) + "px",
                      width: (_imgWidth * ratio) + "px"
                  }, function () {
                      if ($.isFunction(callback))
                          callback();
                  });

        return this;
    };

    $.fn.resize = function (w, h) {
        var theImage = this;
        //new Image();
        //theImage.src = this.attr("src");
        var imgwidth = theImage.width();
        var imgheight = theImage.height();

        var containerwidth = w;
        var containerheight = h;

        if (imgwidth > containerwidth) {
            var newwidth = containerwidth;
            var ratio = imgwidth / containerwidth;
            var newheight = imgheight / ratio;
            if (newheight > containerheight) {
                var newnewheight = containerheight;
                var newratio = newheight / containerheight;
                var newnewwidth = newwidth / newratio;
                imgwidth = newnewwidth;
                imgheight = newnewheight;

                //theImage.width = newnewwidth;
                //theImage.height = newnewheight;
            }
            else {
                imgwidth = newwidth;
                imgheight = newheight;
                //theImage.width(newwidth);
                //theImage.height(newheight);
            }
        }
        else if (imgheight > containerheight) {
            var newheight = containerheight;
            var ratio = imgheight / containerheight;
            var newwidth = imgwidth / ratio;
            if (newwidth > containerwidth) {
                var newnewwidth = containerwidth;
                var newratio = newwidth / containerwidth;
                var newnewheight = newheight / newratio;
                //            theImage.height(newnewheight);
                //            theImage.width(newnewwidth);
                imgwidth = newnewwidth;
                imgheight = newnewheight;
            }
            else {
                //            theImage.width(newwidth);
                //            theImage.height(newheight);
                imgwidth = newwidth;
                imgheight = newheight;
            }
        }

        this.css({
            'width': imgwidth + "px",
            'height': imgheight + "px",
            'margin-top': ((containerheight / 2) - (imgheight / 2)) + 'px',
            'margin-left': ((containerwidth / 2) - (imgwidth / 2)) + 'px'
        });
    }

    $.getFileName = function (url) {
        if (url) {
            var segs = url.split("/");
            return segs[segs.length - 1];
        }
        return "";
    };

    $.getFileExtension = function (file) {
        if (file)
            return file.split(".").pop();
        return "";
    };

    $.getGEOLocation = function () {
        var dfd = $.Deferred();
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                try {
                    var _coords = { lon: position.coords.longitude, lat: position.coords.latitude }, geocoder = new google.maps.Geocoder(), _ll = new google.maps.LatLng(_coords.lat, _coords.lon);
                    geocoder.geocode({ "latLng": _ll }, function (results, status) {
                        if (status == google.maps.GeocoderStatus.OK)
                            return dfd.resolve({ result: results, coords: _coords, error: "" });
                        else
                            return dfd.reject({ error: "Geocode was not successful for the following reason: " + status });
                    });
                    //console.log(_coords);
                }
                catch (e) {
                    return dfd.reject({ error: "Map service is not avalidable." });
                };

            }, function (error) {
                return dfd.reject({ error: error });
            }, { timeout: 60000 });
        } else {
            return dfd.reject({ error: "Your browser doesnot support geo location." });
        }
        return dfd;
    };

    $.jsonDate = function (val) {
        return new Date(parseInt(val.substring(6, 19)));
    };

    $.fn.insertFormat = function (_format) {
        var element = $(this);
        if (document.selection) {
            element.focus();
            var _selection = document.selection.createRange();
            _selection.text = _format.replace("{0}", _selection.text);
        }
        else {
            var _ele = element[0];
            if (_ele.selectionEnd) {
                var _start = _ele.selectionStart, _end = _ele.selectionEnd,
                        _rep = _format.replace("{0}", _ele.value.substring(_start, _end)),
                        _pre = _ele.value.substring(0, _start),
                          _last = _ele.value.substring(_end);
                element.val(_pre + _rep + _last);
            } else
                element.val(element.val() + _format);
        }
    };

    $.dateDiff = function (date1, date2) {
        var timespan = date2 - date1;
        return {
            minutes: timespan / (60 * 1000),
            hours: timespan / (60 * 60 * 1000),
            days: timespan / (24 * 60 * 60 * 1000)
        };
    };

    Number.prototype.toShort = function () {
        if (this > 100 && this < 1000)
            return "99+";
        if (this > 1000 && this < 10000)
            return (this / 1000).toFixed() + "k+";

        if (this > 10000)
            return (this / 10000).toFixed() + "m+";

        return this.toString();
    }

    String.prototype.slug = function (expr) {
        if (!expr)
            expr = "-";

        if (!this)
            return "";

        var formattedTitle = this.toLowerCase();
        var chars = [/ /g, /;/g, /,/g, /\?/g, /\>/g, /\</g, /\./g, /\'/g, /\\/g, /\//g, /\\/g, /\~/g, /\:/g, /\!/g, /@/g, /#/g, /\{/g, /\}/g, /\[/g, /\]/g,
            /\|/g, /_/g, /\=/g, /\$/g, /\%/g, /\^/g, /\*/g, /\(/g, /\)/g, /\+/g, /-/g, /\&/g, /！/g, /·/g, /￥/g, /%/g, /…/g, /—/g, /（/g, /）/g, /＝/g, /、/g, /，/g, /。/g,
            /‘/g, /’/g, /“/g, /”/g, /；/g, /：/g, /？/g, /《/g, /》/g];

        for (var i = 0; i < chars.length; i++)
            formattedTitle = formattedTitle.replace(chars[i], expr);

        if (formattedTitle.endsWith(expr)) {
            if (formattedTitle.length >= 2)
                formattedTitle = formattedTitle.substring(0, formattedTitle.Length - 1);
        }

        while (formattedTitle.indexOf(expr + expr) > -1)
            formattedTitle = formattedTitle.replace(/--/g, expr);
        return formattedTitle;

    }

    $.friendlyDate = function (val, formats) {
        var _date = $.jsonDate(val),
                 timespan = $.dateDiff(_date, new Date()),
                 fs = {
                     theDayBeforeYesterday: "The day before yesterday",
                     yesterday: "Yesterday",
                     ago: " ago",
                     days: "days",
                     hours: "hours",
                     minutes: "minutes",
                     justNow: "just now"
                 };

        if (formats)
            $.extend(fs, formats);

        if (timespan.days > 1) {
            if (timespan.days <= 3) {
                if (timespan.days <= 3 && timespan.days >= 2)
                    return fs.theDayBeforeYesterday;

                if (timespan.days <= 2 && timespan.days >= 1)
                    return fs.yesterday;

                return Math.round(timespan.days) + " " + fs.days + fs.ago;
            }
            else {
                if (window.Intl)
                    return Intl.DateTimeFormat($("body").attr("lang")).format(_date);
                else
                    return _date.toLocaleString();
            }
        }
        else {
            if (timespan.hours > 1)
                return Math.round(timespan.hours) + " " + fs.hours + fs.ago;
            else {
                if (fs.minutes)
                    return Math.round(timespan.minutes) + " " + fs.minutes + fs.ago;
                else
                    return fs.justNow;
            }
        }

        if (Intl)
            return Intl.DateTimeFormat($("body").attr("lang")).format(_date);
        else
            return _date.toLocaleString();

    };

    $.reportAbuse = function (url, owner, objectType, content, title) {
        $.ajaxDialog({
            title: title ? title : "Report abuse",
            url: $("body").data("root") + "abuse/report?locale=" + $("body").attr("lang"),
            open: function () {
                $("#abuse_uri").val(url);
                $("#abuse_owner").val(owner);
                $("#abuse_objectType").val(objectType);
                $("#abuse_content").val(content);
            }
        });
    }

    $.recollect = function (id) {
        //if ($.isAuth) {
        //    var dlg=
        //} else
        //    $.login();
    }

    $.design = function (name, selector, title) {
        var dlg = $("<div/>").appendTo("body");
        $("body").blockUI();

        dlg.load($("body").data("root") + "theme/design?locale=" + $("body").attr("lang") + "&name=" + name + "&element=" + selector,
            function () {
                //$("body").unblockUI();
                $.loading();
                dlg.taoUI();
                //dlg.unobtrusive_ajax().taoUI();
                //dlg.enableComments();
                dlg.dialog({
                    title: title ? title : "Change style",
                    autoOpen: true,
                    resizable: false,
                    //resizable: opts.resizable != undefined ? opts.resizable : false,
                    //width: opts.width ? opts.width : "auto",
                    //buttons: opts.buttons,
                    //open: opts.open ? opts.open : null,
                    close: function () { dlg.remove(); }
                });
            });
        return dlg;
    }

    $.login = function (title) {
        var _body = $("body"),
            dlg = $.openDlg({
                title: title ? title : "Login",
                url: _body.data("root") + "login?locale=" + _body.attr("lang"),
                autoOpen: true
            }),
             dfd = $.Deferred();

        $(document).one("login", function (data) {
            $.loading("hide");
            if (dlg)
                dlg.taoDialog("close");
            _body.data("auth", true);
            $(document).data("user", data);
            return dfd.resolve(data);
        });
        return dfd;
    }

    $.mailto = function (user, title, callback) {
        var _body = $("body"),
        dlg = $.openDlg({
            title: title ? title : "Send mail",
            url: _body.data("root") + "account/sendmail?locale=" + _body.attr("lang") + "&" + user,
            autoOpen: true,
            width: 300
        });
        return dlg;
    }

    $.currentUser = function () {
        return $(document).data("user");
    }

    $.isAuth = function () {
        return $("body").dataBool("auth");
    }

    $.hex2rgb = function (hex, opacity) {
        var rgb = hex.replace('#', '').match(/(.{2})/g);
        var i = 3;
        while (i--) {
            rgb[i] = parseInt(rgb[i], 16);
        }
        if (typeof opacity == 'undefined') {
            return 'rgb(' + rgb.join(', ') + ')';
        }
        return 'rgba(' + rgb.join(', ') + ', ' + opacity + ')';
    }

    $.rgb2hex = function (str) {
        if (str.startsWith("rgb")) {
            str = str.replace(/rgb\(|\)/g, "").split(",");
            str[0] = parseInt(str[0], 10).toString(16).toLowerCase();
            str[1] = parseInt(str[1], 10).toString(16).toLowerCase();
            str[2] = parseInt(str[2], 10).toString(16).toLowerCase();
            str[0] = (str[0].length == 1) ? '0' + str[0] : str[0];
            str[1] = (str[1].length == 1) ? '0' + str[1] : str[1];
            str[2] = (str[2].length == 1) ? '0' + str[2] : str[2];
            return ('#' + str.join(""));
        }
        return str;
    }

    $.invertColor = function (val) {
        var _rgb = val;
        if (val.startsWith("#"))
            _rgb = $.hex2rgb(val);

        str = _rgb.replace(/rgb\(|\)/g, "").split(",");
        str[0] = (255 - parseInt(str[0], 10)).toString(16).toLowerCase();
        str[1] = (255 - parseInt(str[1], 10)).toString(16).toLowerCase();
        str[2] = (255 - parseInt(str[2], 10)).toString(16).toLowerCase();

        str[0] = (str[0].length == 1) ? '0' + str[0] : str[0];
        str[1] = (str[1].length == 1) ? '0' + str[1] : str[1];
        str[2] = (str[2].length == 1) ? '0' + str[2] : str[2];
        return ('#' + str.join(""));
    }

    $.closePanels = function () {
        $(".d-panel[data-opened=true][data-display!=static]").each(function (j, pane) {
            $(pane).taoPanel("close");
        });
    }

    /***Only avaliable for dotnetage****/
    $.fileDialog = function (title, path, filter, _to) {
        var dfd = $.Deferred();
        if (window._fileDlg) {
            window._fileDlg.taoDialog("open");
            $("#btnSelectAnExistsFile").unbind("click").click(function () {
                var _url = $('#selectedFilePath').val();
                if (_to)
                    $(_to).val(_url).trigger("change");

                window._fileDlg.taoDialog("close");
                return dfd.resolve(_url);
            });
            return dfd;
        }

        $.loading();
        var _params = {};
        if (path)
            _params.path = path;
        if (filter)
            _params.filter = filter;

        //console.log($.param(_params));
        var paramStr = $.param(_params);

        var dlg = $.openDlg({
            title: title ? title : "Select a file",
            url: $("body").data("root") + "webfiles/files?website=" + $("body").data("web") + (paramStr ? ("&" + paramStr) : ""),
            autoOpen: true,
            load: function () {
                $.loading("hide");

                $("#btnSelectAnExistsFile", dlg).click(function () {
                    var _url = $('#selectedFilePath', dlg).val();
                    if (_to)
                        $(_to).val(_url).trigger("change");

                    dlg.taoDialog("close");
                    return dfd.resolve(_url);
                });
            }
        });

        window._fileDlg = dlg;

        return dfd;
    }

    $.folderDialog = function (title, path, _to, readonly) {
        var dfd = $.Deferred();
        if (window._folderDlg) {
            window._folderDlg.taoDialog("open");
            $("#btnSelectAnExistsFolder").unbind("click").click(function () {
                var _url = $('#selectedFolder').val();
                if (_to)
                    $(_to).val(_url).trigger("change");
                if (window._folderDlg)
                    window._folderDlg.taoDialog("close");
                return dfd.resolve(_url);
            });
            return dfd;
        }

        $.loading();

        var dlg = $.openDlg({
            title: title ? title : "Select a folder",
            url: $("body").data("root") + "webfiles/folders?website=" + $("body").data("web") + (path ? "&path=" + encodeURIComponent(path) : ""),
            autoOpen: true,
            load: function () {
                $.loading("hide");
                if (readonly) {
                    $("#sys_newfolder_container").hide();
                    $("#btnAddNewFolder").hide();
                } else {
                    $("#sys_newfolder_container").show();
                    $("#btnAddNewFolder").show();
                }

                $("#btnSelectAnExistsFolder", dlg).click(function () {
                    var _url = $('#selectedFolder', dlg).val();
                    if (_to)
                        $(_to).val(_url).trigger("change");
                    dlg.taoDialog("close");
                    return dfd.resolve(_url);
                });
            }
        });

        window._folderDlg = dlg;

        return dfd;
    }

    $.linkDialog = function (title, to) {
        var dfd = $.Deferred();
        if (window._linkDlg) {
            if (to)
                window._linkDlg.data("to", to);
            window._linkDlg.taoDialog("open");
            window._linkDlg.dfd = dfd;
            return dfd;
        }

        var dlg = $.openDlg({
            title: title ? title : "Pick a link",
            url: $("body").data("root") + "dynamicui/links?website=" + $("body").data("web") + "&locale=" + $("body").attr("lang"),
            height: 400,
            autoOpen: true,
            cache: true,
            close: function () {
                var selUrl = $("#sys_selected_url").val();

                if (dlg.data("to"))
                    $(dlg.data("to")).val(selUrl).trigger("change");

                var _selPage = $('#sys_selected_page').val(),
                    _sel = _selPage ? $.parseJSON(_selPage) : { url: selUrl, title: selUrl };
                if (dlg.dfd)
                    return dlg.dfd.resolve(_sel);
                else
                    return dfd.resolve(_sel);
            }
        });

        window._linkDlg = dlg;
        if (to)
            window._linkDlg.data("to", to);
        return dfd;
    }

    $.editLinks = function (title, lnks, callback) {
        var dlg = $("<div/>").css({ "padding": "0px" })
    .attr("title", title ? title : "")
    .appendTo("body")
    .taoDialog({
        url: $.resolveUrl("~/dynamicUI/linksEditor") + "?website=" + $("body").data("web") + "&locale=" + $("body").attr("lang") + "&links=" + (lnks ? JSON.stringify(lnks) : "[]"),
        width: 400,
        cache: false,
        autoOpen: true,
        load: function () {
            $("#sys_lnks_result_button").click(function () {
                var result = $("#sys_links_object").val();
                if ($.isFunction(callback)) {
                    callback(result ? $.parseJSON(result) : {});
                }
                dlg.taoDialog("close");
            });
        }
    });
    }

    $.editImages = function (title, imgs, callback) {
        var dlg = $("<div/>").css({ "padding": "0px" })
            .attr("title", title ? title : "")
            .appendTo("body")
            .taoDialog({
                url: $.resolveUrl("~/dynamicUI/gallery") + "?website=" + $("body").data("web") + "&locale=" + $("body").attr("lang") + "&objectStr=" + (imgs ? JSON.stringify(imgs) : "[]"),
                width: 600,
                cache: false,
                autoOpen: true,
                load: function () {
                    $("#sys_image_result_button").click(function () {
                        var result = $("#image_objects").val();
                        if ($.isFunction(callback)) {
                            callback(eval(result));
                        }
                        dlg.taoDialog("close");
                    });
                }
            });
    }

    $.openDlg = function (url, title) {

        return $("<div/>").css({ "padding": "0px" })
                                    .attr("title", title ? title : "")
                                    .appendTo("body")
                          .taoDialog($.isPlainObject(url) ? url : {
                              url: url,
                              autoOpen: true
                          });
    }

    $.openPanel = function (url, title, cache, pos, disp) {
        var panel = $("<div/>").appendTo("body"),
            header = null,
            _body = $("<div/>").appendTo(panel);

        if ($.isPlainObject(url)) {
            if (url.title)
                header = $("<h3/>").text(url.title).prependTo(panel);

            if ("dialog" != url.display)
                $.closePanels();

            panel.attr("data-display", url.display ? url.display : "overlay");

            if (panel.attr("data-display") == "overlay")
                panel.css("position", "fixed");

            panel.taoPanel(url);
        } else {
            if (title)
                header = $("<h3/>").text(title).prependTo(panel);
            panel.attr("data-pos", pos ? pos : "right").attr("data-display", disp ? disp : "overlay");

            if (panel.attr("data-display") == "overlay")
                panel.css("position", "fixed");

            if ("dialog" != disp)
                $.closePanels();

            panel.taoPanel({
                contentUrl: url,
                opened: true,
                autoRelease: cache != undefined ? cache : true
            });
        }
        return panel;
    }

    $.fn.mobilelist = function () {
        this.addClass("d-mobile-list d-ui-widget");

        if (this.data("inset") == undefined || this.dataBool("inset") == false)
            this.addClass("d-collapse-list");

        if (this.attr("data-link-icon") != undefined) {
            var _linkicon = this.attr("data-link-icon");
            $(">li", this).each(function (i, iit) {
                if ($(iit).attr("data-link-icon") == undefined)
                    $(iit).attr("data-link-icon", _linkicon);
            });
        }

        if (this.attr("data-icon") != undefined) {
            var _icon = this.attr("data-icon");
            $(">li", this).each(function (i, iit) {
                if ($(iit).attr("data-icon") == undefined)
                    $(iit).attr("data-icon", _icon);
            });
        }

        if (!this.hasClass("d-inline")) {
            var eles = this.children("li");
            eles.each(function (i, ele) {
                $(ele).mobilelistItem();
            });
            //Remove the children icon from menu
            this.find(".d-children-icon").remove();
        }
    }

    $.fn.mobilelistItem = function () {
        var firstLink = this.children("a:first"),
            self = this,
            _addicon = function (_parent) {
                if (self.data("icon")) {
                    $("<span/>").addClass(self.data("icon")).addClass("d-inline")
                                         .css({ "margin-right": "10px" })
                                         .prependTo(_parent);
                }
            };

        if ("fieldcontain" == this.data("role")) {
            this.addClass("d-field-container");
        } else {

            if (this.data("counter") != undefined) {
                var counter = $("<span/>").addClass("d-item-counter")
                                     .text(this.data("counter"))
                                     .appendTo(firstLink.length ? firstLink : this);
            }

            if (firstLink.length) {
                var _icon = this.data("link-icon") ? this.data("link-icon") : "d-icon-chevron-right";
                firstLink.addClass("d-link");

                if (firstLink.attr("href") == "#")
                    firstLink.attr("javascript:void(0);");

                if (firstLink.children().length == 0) {
                    firstLink.wrapInner("<span class='d-inline'/>");
                }

                firstLink.children("img").addClass("d-item-thumb");

                if (_icon != "no" && _icon != false) {
                    if (this.parent().data("link-icon") != "no")
                        //Add icon to right
                        $("<span/>").addClass(_icon)
                                             .addClass("d-link-icon")
                                             .appendTo(firstLink);
                }
            }

            if (this.children("a").length > 1) {
                //Spliter
                var spliter = this.children("a:last");
                if (spliter.length) {
                    spliter.addClass("d-item-spliter")
                              .hover(function () { $(this).addClass("d-ui-widget").isHover(true); $(this).parent().isHover(false); }, function () { $(this).removeClass("d-ui-widget").isHover(false); })
                             .bind("mousedown", function (e) { $(this).addClass("d-ui-widget").isActive(true); })
                             .bind("mouseup", function (e) { $(this).removeClass("d-ui-widget").isActive(false); });

                    var _href = spliter.attr("href");
                    if (_href == undefined || _href == "#" || _href == "javascript:void(0);") {
                        spliter.bind("click", function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                        })
                    }

                    var _cls = spliter.data("icon") ? spliter.data("icon") : "d-icon-cog";
                    if (!_cls.startsWith("d-icon"))
                        _cls = "d-icon-" + _cls;

                    $("<span/>").addClass(_cls).appendTo(spliter);

                    firstLink.css({
                        "margin-right": "40px",
                        "position": "relative"
                    });
                }
            }

            if (this.find(">img:first").length) {
                this.addClass("d-item-has-thumb");
                var _thumb = this.find(">img:first").addClass("d-item-thumb");
                if (_thumb.length) {
                    var _h = _thumb.height(),
                        _w = _thumb.width();

                    if (_thumb.data("size") != undefined) {
                        var _ts = _thumb.dataInt("size");
                        _thumb.css({
                            "margin-top": (-(Math.abs(_ts / 2))) + "px",
                            "max-height": _ts + "px",
                            "max-width": _ts + "px"
                        });

                        if ($("~h4", _thumb).length)
                            $("~h4", _thumb).css("margin-left", (10 + _ts) + "px");

                        if ($("~p", _thumb).length)
                            $("~p", _thumb).css("margin-left", (10 + _ts) + "px");

                        if (_h > 0 && _h < _ts) {
                            var mt = -((_h / 2) / 16);
                            _thumb.css("margin-top", mt + "em");
                        }

                        if (_w > 0 && _w < _ts) {
                            var ml = (_ts - _w) / 16;
                            _thumb.css("margin-left", ml + "em");
                        }
                    }
                }
            }

            this.find("h4").addClass("d-item-heading d-content-title");
            this.find("p").addClass("d-item-desc d-content-s");


            if ("divider" == this.data("role")) {
                if (firstLink.length == 0) {
                    var l = $("<a href='javascript:void(0);'/>").addClass("d-link");
                    this.wrapInner(l);
                    _addicon(this.children());
                } else
                    _addicon(this);

                this.addClass("d-ui-widget-header d-list-divider").unbind();

            } else {

                if (firstLink.length)
                    _addicon(firstLink);
                else
                    _addicon(this);
            }
        }

    }

    $.fn.buttonlist = function () {
        this.addClass("d-button-list");
        $(">li", this).addClass("d-button");
    }

    $.formatSize = function (val) {
        var size = val,
            unit = "bytes";

        if (size > 1024) {
            size = Math.round(size / 1024);
            unit = "kb";
        }

        if (size > 1024) {
            size = Math.round(size / 1024);
            unit = "mb";
        }

        if (size > 1024) {
            size = Math.round(size / 1024);
            unit = "gb";
        }
        if (size == 0) unit = "byte";

        return size + unit;
    }

    $.fn.marginWidth = function () {
        return this.outerWidth(true) - this.width();
    }

    $.fn.marginHeight = function () {
        return this.outerHeight(true) - this.height();
    }

    $.fn.getMargin = function (dir) {
        var v = $(this).css("margin" + (dir ? "-" + dir : ""));
        if (v) {
            var nt = parseInt(v.replace("px", ""));
            if (nt)
                return nt;
        }

        return 0;
    }

    if (!$.isFunction(XMLHttpRequest.prototype.sendAsBinary)) {
        //Impletement the sendAsBinary for chrome
        XMLHttpRequest.prototype.sendAsBinary = function (datastr) {
            var ui8a = new Uint8Array(datastr.length);
            for (var i = 0; i < datastr.length; i++) {
                ui8a[i] = (datastr.charCodeAt(i) & 0xff);
            }
            this.send(ui8a.buffer);
        }
    }

    $.upload = function (url, file, saveas, fnLoad, fnProgress) {
        var boundary = '------multipartformboundary' + (new Date).getTime(),
            _getBuilder = function (filename, filedata, boundary) {
                var dashdash = '--', crlf = '\r\n', builder = '';

                if (saveas) {
                    builder += dashdash;
                    builder += boundary;
                    builder += crlf;
                    builder += 'Content-Disposition: form-data; name="' + saveas + '"';
                    builder += crlf;
                    builder += crlf;
                    builder += _val;
                    builder += crlf;
                }

                builder += dashdash;
                builder += boundary;
                builder += crlf;
                builder += "Content-Disposition: form-data; name=\"file\"";
                builder += '; filename="' + filename + '"';
                builder += crlf;

                builder += 'Content-Type: application/octet-stream';
                builder += crlf;
                builder += crlf;

                builder += filedata;
                builder += crlf;

                builder += dashdash;
                builder += boundary;
                builder += dashdash;
                builder += crlf;
                return builder;
            },
            _send = function (evt) {
                var dashdash = '--',
                crlf = '\r\n',
                xhr = new XMLHttpRequest(),
                upload = xhr.upload,
                index = evt.target.index,
                start_time = new Date().getTime(),
                _fileName = file.name,
                builder = _getBuilder(encodeURIComponent(_fileName), evt.target.result, boundary);

                upload.index = index;
                upload.file = file;
                upload.downloadStartTime = start_time;
                upload.start = start_time;
                upload.progress = 0;
                upload.startData = 0;

                if ($.isFunction(fnProgress))
                    upload.addEventListener("progress", function (e) {
                        if (e.lengthComputable) {
                            var percentage = Math.round((e.loaded * 100) / e.total);
                            fnProgress(e, { percentage: percentage, file: upload.file, loaded: e.loaded, total: e.total });
                        }
                    }, false);
                //console.log(url);
                xhr.open("POST", url, true);
                xhr.setRequestHeader('content-type', 'multipart/form-data; boundary=' + boundary);
                if ($.isFunction(fnLoad)) {
                    xhr.onload = function (event) {
                        var fnLoadProxy = $.proxy(fnLoad, this);
                        fnLoadProxy(event, xhr, xhr.statusText, xhr.responseText);
                    };
                }
                xhr.sendAsBinary(builder);
            };
        var reader = new FileReader();
        reader.onloadend = _send;
        reader.readAsBinaryString(file);
    }

    $.fn.fileDroppable = function (options) {
        var el = this, defaults = {
            accept: null,
            start: null,
            over: null,
            out: null,
            stop: null,
            read: null,
            drop: null,
            helper: null,
            hoverClass: null,
            activeClass: null
        };

        if (options)
            $.extend(defaults, options);

        this[0].addEventListener('dragenter', function (e) {
            e.stopPropagation();
            e.preventDefault();

            if ($(".d-droppable-helper", el).length == 0) {
                var helper = $.isFunction(defaults.helper) ? defaults.helper() : null;

                if (helper && helper.length) {
                    helper.addClass("d-droppable-helper")
                               .appendTo(el);
                }
            }

            if ($.isFunction(defaults.start)) {
                var startProxy = $.proxy(defaults.start, el);
                startProxy(e, { helper: helper });
            }

            if (defaults.hoverClass)
                el.addClass(defaults.hoverClass);

        }, false);

        var removeHelper = function () {
            var _helper = $(".d-droppable-helper", el);
            if (_helper.length)
                _helper.remove();
        };

        this[0].addEventListener('dragover', function (e) {
            e.stopPropagation();
            e.preventDefault();

            if ($.isFunction(defaults.over)) {
                var overProxy = $.proxy(defaults.over, el);
                overProxy(e);
            }

            if (defaults.activeClass)
                el.addClass(defaults.activeClass);

        }, false);

        this[0].addEventListener('dragleave', function (e) {
            e.stopPropagation();
            e.preventDefault();

            if ($.isFunction(defaults.out)) {
                var outProxy = $.proxy(defaults.out, el);
                outProxy(e);
            }

            if (defaults.hoverClass)
                el.removeClass(defaults.hoverClass);

            if (defaults.activeClass)
                el.removeClass(defaults.activeClass);

            removeHelper();
        }, false);

        this[0].addEventListener('drop', function (e) {
            e.stopPropagation();
            e.preventDefault();

            var readFileSize = 0;
            var files = e.dataTransfer.files;
            removeHelper();

            if ($.isFunction(defaults.drop))
                defaults.drop(files);

            if ($.isFunction(defaults.stop)) {
                var proxyStop = $.proxy(defaults.stop, el);
                proxyStop(e, files);
            }

            if ($.isFunction(defaults.read)) {
                for (var i = 0; i < files.length; i++) {
                    var reader = new FileReader(),
                        file = files[i];
                    reader.onerror = function (e) {
                        $.err("Error code: " + e.target.error.code);
                    };

                    reader.onload = (function (aFile) {
                        return function (evt) {
                            var proxyRead = $.proxy(defaults.read, el);
                            proxyRead(evt, { result: evt.target.result, size: evt.loaded, reader: evt.target });
                            //fnFileRead(evt, { result: evt.target.result, size: evt.loaded, reader: evt.target });
                        };
                    })(file);

                    if (file.type.startsWith("text"))
                        reader.readAsText(file);
                    else
                        reader.readAsDataURL(file);
                }
            }

            return false;

        }, false);

        return this;
    }

    $.editCode = function (title, value, onsave, lang) {
        $.loading("Loading source code editor...");
        if (window.ace == undefined) {
            $("<script></script>").attr("src", $.resolveUrl("~/scripts/ace/ace.js")).attr("type", "text/javascript").appendTo($("head"));
            $("<script></script>").attr("src", $.resolveUrl("~/scripts/ace/theme-chrome.js")).attr("type", "text/javascript").appendTo("body");
            // $.getScript($.resolveUrl("~/scripts/ace/theme-chrome.js"));
        }

        var langUrl = $.resolveUrl("~/scripts/ace/mode-" + (lang ? lang : "html") + ".js"),
            langID = (lang ? lang : "html") + "_mode_script";
        if ($("#" + langID).length == 0)
            $("<script id=\"" + langID + "\"></script>").attr("src", langUrl).attr("type", "text/javascript").appendTo($("head"));

        var dfd = $.Deferred(),
            dlg = $("<div/>").appendTo("body").css({ "padding": "0px" }),
           toolbar = $("<ul>").appendTo(dlg),
           liSave = $("<li>").appendTo(toolbar).html("<a href='javascript:void(0);'><span class='d-icon-disk'></span></a>"),
           editor = $("<div id='" + (lang ? lang : "html") + "_sourcecode_editor' />").css({ position: "relative" }).appendTo(dlg);

        toolbar.taoMenu({
            type: "toolbar",
            itemClick: function (event, ui) {
                if ($.isFunction(onsave)) {
                    if (onsave(dlg.editor.getValue())) {
                        dlg.taoDialog("close");
                        //dlg.taoDialog("destroy");
                        //dlg.remove();
                    }
                }
            }
        });

        dlg.taoDialog({
            title: title,
            fullscreen: true,
            cache: false,
            open: function () {
                $.loading("hide");
                
                if (dlg.editor == undefined) {

                    var editor_height = dlg.parent().height() - dlg.parent().children('.d-dialog-header').height() - toolbar.outerHeight(true) - 20,
                        editor_width = dlg.width();


                    $(editor).height(editor_height)
                                  .width(editor_width);
                    editor = ace.edit((lang ? lang : "html") + "_sourcecode_editor");
                    editor.setTheme("ace/theme/chrome");
                    //  var editMode = require("ace/mode/" + (lang ? lang : "html")).Mode;
                    //editor.getSession().setMode(new editMode());
                    editor.getSession().setUseWorker(false);
                    editor.getSession().setMode("ace/mode/" + (lang ? lang : "html"));

                    if (value) {
                        editor.setValue(value, 1);
                        editor.clearSelection();
                        editor.focus();
                    }

                    if (!$.isFunction(onsave)) {
                        liSave.addClass("d-state-disable");
                        editor.setReadOnly(true);
                    }

                    dlg.editor = editor;
                }
                window.setTimeout(function () { dlg.closest(".d-dialog").css("top","0px"); },300);
            }
        });

        return dfd;
    }

})(jQuery, window, document);