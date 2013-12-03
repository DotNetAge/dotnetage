/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.taoGrid", $.dna.taoDataBindingList, {
        options: {
            selection: null, //null | row | cell | both
            pagable: false,
            sortable: false,
            scrollable: false,
            filterable: false,
            groupable: false,
            height: null,
            width: null,
            showHeader: true,
            columns: null,
            detailTemplate: null,
            rowTemplate: null
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, eventPrefix = this.widgetEventPrefix, fromtable = false;
            this._unobtrusive();
            this._widget = el;

            if (el[0].tagName.toLowerCase() == "table")
                fromtable = true;

            if (fromtable) {
                this._widget = $("<div/>").addClass("d-reset d-ui-widget d-grid");
                el.before(this._widget);
                el.removeAttr("data-role");
            } else
                el.addClass("d-grid");

            if (opts.groupable)
                $("<div/>").addClass("d-grid-group")
                                     .prependTo(this._widget);

            var header = $("<div/>").addClass("d-grid-header")
                                                         .appendTo(this._widget);

            if (!opts.showHeader) header.hide();

            $("<div/>").addClass("d-grid-filter")
                                 .appendTo(this.widget())
                                 .hide();

            $("<div/>").addClass("d-ui-widget-content d-grid-content")
                                 .appendTo(this._widget);

            if (opts.pagable) {
                $("<div/>").addClass("d-grid-pager")
                                     .appendTo(this._widget)
                                     .taoPager({ datasource: opts.datasource });
            }

            if (fromtable) {
                var columns = [];
                var _cols = $(">thead th,>thead td", el);
                _cols.each(function (i, n) {
                    var col = $(n), column = {
                        name: col.attr("data-field") != undefined ? col.attr("data-field") : $.trim(col.text()),
                        title: $.trim(col.text()),
                        width: col.data("width") != undefined ? col.dataInt("width") : 0,
                        type: col.attr("data-type") != undefined ? col.attr("data-type") : "string"
                    };

                    if (col.data("align"))
                        column.align = col.data("align");
                    else {
                        if (column.type == "number") {
                            column.align = "right";
                        }
                    }

                    columns.push(column);
                });
                this.options.columns = columns;
                $(">thead", el).hide();
                //el.appendTo($(">.d-grid-content", this._widget));

                if (el.attr("style") != undefined && el.attr("style").indexOf("height") > -1) {
                    this.options.height = el.height();
                    this.options.scrollable = true;
                    //el.removeAttr("style");
                }

                if (opts.datasource == null) {
                    opts.datasource = {
                        mapper: new tao.htmlTableMapper(),
                        data: el
                    }
                }
                el.hide();
            }

            this._setDataSource(opts.datasource);

            if (this._source)
                this._getSchemaCols(this._source.taoDataSource("option", "schema"));

            if (opts.columns) {
                this._grid().initColumns(opts.columns);
                if (opts.height) {
                    this._grid().content.tableElement.height(opts.height);
                    self._createScroller(true);
                } else {
                    $(">.d-grid-content", this.widget()).css("overflow-y", "auto");
                }
            }

            this._setGroupable(opts.groupable)
                   ._setFilterable(opts.filterable)
                   ._bindRowEvents()
                   ._bindCellEvents()
                   ._bindColEvents();

            if (this._source)
                this._source.taoDataSource("read");
        },
        _unobtrusive: function () {
            var el = this.element, opts = this.options;
            if (el.data("onselect")) opts.select = new Function("event", "ui", el.data("onselect"));
            if (el.data("selection") != undefined) opts.selection = el.data("selection");
            if (el.data("pagable") != undefined) opts.pagable = el.dataBool("pagable");
            if (el.data("scrollable") != undefined) opts.scrollable = el.dataBool("scrollable");
            if (el.data("filterable") != undefined) opts.filterable = el.dataBool("filterable");
            if (el.data("sortable") != undefined) opts.sortable = el.dataBool("sortable");
            if (el.data("groupable") != undefined) opts.groupable = el.dataBool("groupable");
            if (el.data("height") != undefined) opts.height = el.dataInt("height");
            if (el.data("detailTmpl")) opts.detailTemplate = el.datajQuery("detailTmpl");
            if (el.data("rowTmpl")) {
                opts.rowTemplate = el.datajQuery("rowTmpl");
                opts.itemTmpl = opts.rowTemplate;
            }
            return $.dna.taoDataBindingList.prototype._unobtrusive.call(this);
        },
        _grid: function () {
            var opts = this.options, self = this,
             table = function (element) {
                 this.element = element;
                 var tb = $(">table", element);
                 if (!tb.length)
                     tb = $("<table/>").appendTo(element);
                 this.tableElement = tb;
             };
            table.prototype = {
                columns: function (val) {
                    if (val && $.isArray(val)) {
                        //use to check the columns is init
                        var colGroup = $(">colgroup", this.tableElement);
                        if (!colGroup.length)
                            colGroup = $("<colgroup/>").appendTo(this.tableElement);
                        else
                            colGroup.empty();

                        $.each(val, function (i, n) {
                            if (!n.hidden) {
                                var col = $("<col/>").appendTo(colGroup);
                                if (n.width)
                                    col.width(n.width);
                            }
                        });
                    }
                    return $(">colgroup>col", this.tableElement);
                },
                heads: function (val, factory) {
                    if (val && $.isArray(val)) {
                        var thead = $(">thead", this.tableElement);
                        if (!thead.length)
                            thead = $("<thead/>").appendTo(this.tableElement);
                        else
                            thead.empty();

                        var headRow = $("<tr/>").appendTo(thead);

                        $.each(val, function (i, n) {
                            if (!n.hidden) {
                                var th = $("<th/>").attr("data-field", n.name)
                                                            .attr("data-type", n.type ? n.type : "string")
                                                            .addClass("d-ui-widget-header")
                                                            .appendTo(headRow)
                                                            .disableSelection();

                                if (opts.sortable) {
                                    th.hover(function () { $(this).isHover(true); }, function () { $(this).isHover(false); });
                                }

                                if (n.width)
                                    th.width(n.width);

                                if ($.isFunction(factory))
                                    factory(th, n);
                                else
                                    th.append($("<a/>").attr("href", "javascript:void(0);").text(n.title ? n.title : n.name));
                            }
                        });
                    }

                    return $(">thead>tr>th", this.tableElement);
                },
                body: function (newWhenEmpty) {
                    var tbody = $(">tbody", this.tableElement);

                    if (tbody.length == 0 && newWhenEmpty)
                        return $("tbody").appendTo(this.tableElement);

                    return tbody;
                }
            };
            if (!this.elements) {
                this.elements = {
                    header: new table($(">.d-grid-header", this.widget())),
                    filter: new table($(">.d-grid-filter", this.widget())),
                    group: new table($(">.d-grid-group", this.widget())),
                    content: new table($(">.d-grid-content", this.widget())),
                    footer: new table($(">.d-grid-footer", this.widget())),
                    initColumns: function (val) {
                        this.header.columns(val);
                        this.header.heads(val);

                        if (opts.filterable)
                            this.filter.columns(val);

                        this.content.columns(val);

                        if (this.footer.length)
                            this.footer.columns(val);
                    }
                };
            }
            return this.elements;
        },
        _bindRowEvents: function () {
            var opts = this.options, self = this, rows = $(">.d-grid-content>table>tbody>tr", this.widget());
            rows.each(function (i, n) {
                var row = $(n);
                row.attr("tabIndex", i);
                if (i % 2) row.addClass("d-row-alt");
                row.hover(function () {
                    if (!$(this).isDisable() && !$(this).isActive()) {
                        if (opts.selection && (opts.selection == "row" || opts.selection == "both"))
                            $(this).isHover(true);
                    }
                },
                function () {
                    $(this).isHover(false);
                })
                    .bind("click", function () {
                        if (!$(this).isDisable() && !$(this).isActive()) {
                            if (opts.selection && (opts.selection == "row" || opts.selection == "both")) {
                                $(this).siblings(".d-state-active").removeClass("d-state-active");
                                $(this).isActive(true);
                                var dataItem = null;
                                if (self._source) {
                                    if ($(this).data("dataItem")) {
                                        dataItem = $(this).data("dataItem");
                                        self._source.taoDataSource("pos", dataItem);
                                    }
                                }
                                self._triggerEvent("select", { element: $(this), dataItem: dataItem });
                            }
                        }
                    })
                    .bind("keypress", function (event) {
                        // event.stopPropagation();
                        // event.preventDefault();
                        if (event.keyCode == 40 || event.keyCode == 38) {
                            var i = parseInt($(this).attr("tabIndex"));
                            var sibling = $(this).siblings("[tabIndex='" + (i + (event.keyCode == 40 ? 1 : -1)) + "']");
                            if (sibling.length)
                                sibling.click();
                        }
                        else {
                            if (event.keyCode == 13) {
                                // self._onitemselected($(item).siblings().index(item), item);
                                //self._setItemSelected(item);
                            }
                            else {
                                //if (event.keyCode == 46)
                                // self._onitemdelete(item);
                            }
                        }
                    });

            });
            return this;
        },
        _bindCellEvents: function () {
            var self = this, opts = this.options, cells = $(">.d-grid-content>table>tbody>tr>td", this.widget());
            cells.each(function (i, n) {
                $(n).bind("click",function () {
                    var activeCells = $(">.d-grid-content>table>tbody>tr>td.d-state-active", self.widget());
                    if (activeCells.length) activeCells.removeClass("d-state-active");
                    if (opts.selection && (opts.selection == "cell" || opts.selection == "both"))
                        $(this).isActive(true);
                });
            });
            return this;
        },
        _bindColEvents: function () {
            var self = this, opts = this.options, colElements = this._grid().header.heads();
            colElements.click(function () {
                if (opts.sortable) {
                    var dir = $(this).data("dir");
                    var sorter = $(">a>.d-sorter", colElements);
                    if (sorter.length) sorter.remove();

                    if (!dir) dir = "asc";

                    sorter = $("<span/>").addClass("d-sorter").appendTo($(">a", this));

                    if (dir == "asc") {
                        $(this).data("dir", "desc");
                        sorter.addClass("d-icon-caret-up");
                    } else {
                        $(this).data("dir", "asc");
                        sorter.addClass("d-icon-caret-down");
                    }
                    self.sort({ field: $(this).data("field"), dir: dir });
                }
            });
            return this;
        },
        _onDataChanged: function (results) {
            var _schema = this._source.taoDataSource("option", "schema");
            if (_schema && _schema.fields && _schema.fields.length)
                this._getSchemaCols(_schema);
            else {
                if (this.options.columns == null || this.options.columns.length == 0)
                    this._generateCols(results.data);
            }

            this._createDataElements(results.data)
                   ._bindRowEvents()
                   ._bindCellEvents();
        },
        _onProcess: function () {
            this.widget().blockUI();
        },
        _onComplete: function () {
            this.widget().unblockUI();
        },
        _getItemsContainer: function () {
            var opts = this.options, contentTb = $(">.d-grid-content>table", this.widget()),
            tbody = $(">tbody", contentTb);
            if (!tbody.length)
                tbody = $("<tbody/>").appendTo(contentTb);
            return tbody;
        },
        _createItemElement: function () {
            return $("<tr/>");
        },
        _onItemCreated: function (element, data) {
            $.each(this.options.columns, function (j, col) {
                if (!col.hidden) {
                    var cellVal = data ? data[col.name] : "";
                    var cell = $("<td/>").appendTo(element).html(cellVal);
                    if (col.align)
                        cell.css("text-align", col.align);
                }
                //                if (td.isOverflow())
                //                    td.attr("title", cellVal);
            });
        },
        _createScroller: function (scrollable) {
            var head = $(">.d-grid-header", this.widget()),
            wrapper = $(">.d-grid-header-wrap", head);

            if (scrollable) {
                if (!wrapper.length) {
                    head.css("padding-right", "17px")
                            .wrapInner("<div class='d-grid-header-wrap' />");
                    var content = $(">.d-grid-content", this.widget());

                    if (this.options.height)
                        content.height(this.options.height);
                }
                $(">.d-grid-content", this.widget()).css("overflow-y", "scroll");
            } else {
                if (wrapper.length) {
                    $(">table", wrapper).appendTo(head);
                    head.css("padding-right", "0px");
                    wrapper.remove();
                    $(">.d-grid-content", this.widget()).css("height", "auto");
                }
                $(">.d-grid-content", this.widget()).css("overflow-y", "auto");
            }
            return this;
        },
        _getSchemaCols: function (val) {
            if (val) {
                if (val.fields) {
                    //if (this.options.columns==null || this.options.columns==undefined || this.options.columns==[]) {
                    if (val.fields.length > 0) {
                        this.options.columns = val.fields;
                        this._grid().initColumns(val.fields);
                    }
                    //}
                }
            }
            return this;
        },
        _generateCols: function (data) {
            //Auto generate columns
            var cols = [];
            if (data && data.length) {
                var model = data[0];
                for (var p in model) {
                    var _c = {
                        name: p,
                        title: p,
                        type: $.type(model[p])
                    };
                    //if (p.width!=undefined)
                    //_c.width=model.
                    cols.push(_c);
                }
                this.options.columns = cols;
                this._grid().initColumns(cols);
            }
            return this;
        },
        _setFilterable: function (val) {
            var filters = this._grid().filter, self = this;
            if (val) {
                if (!filters.heads().length) {
                    if (this.options.columns) {
                        filters.heads(this.options.columns, function (element, column) {

                            var filterTrigger = $("<a data-role='link'>Filter:None</a>").appendTo(element);

                            var menu = $("<ul/>").attr("data-role", "menu")
                                                              .attr("data-trigger", "prev")
                                                              .attr("data-checkable", true)
                                                              .appendTo(element);
                            var input = $("<input data-role='textbox' />");

                            $("<li/>").append(input)
                                             .appendTo(menu);

                            $("<li/>").addClass("d-spliter").appendTo(menu);
                            if (column.type == undefined || column.type != "number") {
                                $("<li/>").append($("<a/>").text("Starts with").attr("href", "javascript:void(0);"))
                                          .data("operator", "startswith")
                                          .appendTo(menu);
                                $("<li/>").append($("<a/>").text("Contains").attr("href", "javascript:void(0);"))
                                          .data("operator", "contains")
                                          .appendTo(menu);
                                $("<li/>").append($("<a/>").text("Doesn't contain").attr("href", "javascript:void(0);"))
                                          .data("operator", "not~contains")
                                          .appendTo(menu);
                            }

                            $("<li/>").append($("<a/>").text("Equals").attr("href", "javascript:void(0);"))
                                          .data("operator", "eq")
                                          .appendTo(menu);
                            $("<li/>").append($("<a/>").text("Doesn't equal").attr("href", "javascript:void(0);"))
                                          .data("operator", "neq")
                                          .appendTo(menu);

                            if (column.type && column.type == "number") {
                                $("<li/>").append($("<a/>").text("Is less then").attr("href", "javascript:void(0);"))
                                          .data("operator", "lt")
                                          .appendTo(menu);
                                $("<li/>").append($("<a/>").text("Is less then or equal to").attr("href", "javascript:void(0);"))
                                          .data("operator", "le")
                                          .appendTo(menu);
                                $("<li/>").append($("<a/>").text("Is greater then").attr("href", "javascript:void(0);"))
                                          .data("operator", "gt")
                                          .appendTo(menu);
                                $("<li/>").append($("<a/>").text("Is greater then or equal to").attr("href", "javascript:void(0);"))
                                          .data("operator", "ge")
                                          .appendTo(menu);
                            }

                            input.keyup(function () {
                                if ($(element).attr("data-operator") != undefined && input.val())
                                    self._onFilterChanged();
                            })
                                    .click(function (evt) {
                                        evt.stopPropagation();
                                    });

                            menu.children().click(function () {
                                if (input.val()) {
                                    $(element).attr("data-operator", $(this).data("operator"));
                                    $(element).attr("data-val", input.val());
                                    filterTrigger.hide();
                                    var thisItem = $(this);
                                    $("<div/>").addClass("d-filter d-state-active")
                                                         .append($("<span/>").text($(this).text() + " " + input.val()))
                                                         .appendTo(element)
                                                         .append($("<span/>").addClass("d-icon-cross-3"))
                                                         .click(function () {
                                                             $(this).remove();
                                                             filterTrigger.show();
                                                             $(element).removeAttr("data-operator")
                                                                                  .removeAttr("data-val");
                                                             input.val("");
                                                             thisItem.isActive(false);
                                                             self._onFilterChanged();
                                                         });

                                    self._onFilterChanged();
                                }
                            });
                        });
                    }
                }

                if ($(">.d-grid-header>.d-grid-header-wrap", this.widget()).length) {
                    if (!$(filters.element).parent().hasClass("d-grid-header-wrap")) {
                        $(filters.element).wrapInner("<div class=\"d-grid-header-wrap\"/>");
                        $(filters.element).css({ "padding-right": "17px" });
                    }
                }

                $(filters.element).show()
                                         .taoUI();

                //tao.init(filters.element);
            }
            else
                $(filters.element).hide();
            return this;
        },
        _setGroupable: function (val) {
            var self = this;
            if (val) {
                if (!$(">.d-grid-group", this.widget()).length) {
                    $("<div/>").addClass("d-grid-group")
                                     .prependTo(this.widget());
                }

                $(">.d-grid-group", this.widget()).show();

                var g = this._grid().group;
                if ($(">thead", g.tableElement).length == 0) {
                    $("<thead/>").appendTo(g.tableElement)
                                         .append($("<tr/>").append($("<th/>").addClass("d-empty-helper")
                                                                                                   .text("Drag a column header and drop it here to group by that column")));
                }

                $(">.d-grid-group tr", this.widget()).sortable({
                    revert: true,
                    axis: "x",
                    items: "th:not(.d-empty-helper)",
                    placeholder: "d-state-normal",
                    change: function () { self._onGroupsChanged(); },
                    receive: function (event, ui) {
                        var emptyHelper = $(">.d-grid-group .d-empty-helper", self.widget());
                        emptyHelper.hide();
                        ui.sender.addClass("d-state-added");
                        var thisItem = $("th[data-field='" + ui.sender.data("field") + "']", this);
                        $(">a", thisItem).prepend($("<span/>").addClass("d-icon-cross-3")
                                                                                  .css({ "display": "inline-block" })
                                                                                  .click(function () {
                                                                                      if (thisItem.siblings(":not(.d-empty-helper)").length == 0)
                                                                                          emptyHelper.show();
                                                                                      thisItem.remove();
                                                                                      self._onGroupsChanged()
                                                                                  }));
                        var syncSorting = function () {
                            var thisSorter = $(".d-sorter", thisItem), sorter = $(".d-sorter", ui.sender);
                            if (thisSorter.length)
                                $(".d-sorter", thisItem).attr("class", sorter.attr("class"));
                            else
                                $(">a", thisItem).append(sorter.clone());
                        };

                        thisItem.click(function () {
                            ui.sender.click();
                        });

                        ui.sender.bind("click", function () {
                            syncSorting();
                        });
                        self._onGroupsChanged();
                    }
                });

                $(">.d-grid-header th", this.widget()).draggable({
                    revert: "invalid",
                    revertDuration: 200,
                    cancel: ".d-state-added",
                    opacity: .8,
                    connectToSortable: $(">.d-grid-group thead>tr", this.widget()),
                    helper: function (event) {
                        return $("<div/>").addClass("d-state-active ui-corner-all")
                                                    .data("field", $(event.currentTarget).data("field"))
                                                    .appendTo("body")
                                                    .append($("<span/>").text($(event.currentTarget).text())
                                                                                     .css({
                                                                                         "display": "inline-block"
                                                                                     }))
                                                    .prepend($("<span/>").addClass("d-groupable-helper d-icon-plus-4")
                                                                                       .css({
                                                                                           "display": "inline-block"
                                                                                       }))
                                                    .css({
                                                        "padding": "5px 10px",
                                                        width: $(event.currentTarget).width() - 50 + "px"
                                                    });
                    }
                });

            } else {
                $(">.d-grid-group", this.widget()).hide();
                $(">.d-grid-header th", this.widget()).draggable("destory");
            }
            return this;
        },
        _onGroupsChanged: function () {
            var groupEls = $(">.d-grid-group th:not(.d-empty-helper)", this.widget()),
            _groups = [];
            if (groupEls.length) {
                groupEls.each(function (i, n) {
                    _groups.push($(n).data("field"));
                });
                return this.groups(_groups);
            }
            return this.groups();

        },
        _onFilterChanged: function () {
            var filterEls = $(">.d-grid-filter th", this.widget()),
            _filters = [];
            filterEls.each(function (i, n) {
                filterEl = $(n);
                if (filterEl.attr("data-operator") != undefined) {
                    _filters.push({
                        field: filterEl.attr("data-field"),
                        operator: filterEl.attr("data-operator"),
                        val: filterEl.attr("data-val")
                    });
                }
            });
            if (_filters.length > 0)
                return this.filter(_filters);
            else
                return this.filter();
        },
        _exec: function (method, params) {
            this._grid().content.body(true).empty();
            return this._source.taoDataSource(method, params);
        },
        sort: function (val) {
            return this._exec("sort", val ? ($.isArray(val) ? val : [val]) : undefined);
        },
        filter: function (val) {
            return this._exec("filter", val ? ($.isArray(val) ? val : [val]) : undefined);
        },
        groups: function (val) {
            return this._exec("group", val ? ($.isArray(val) ? val : [val]) : undefined);
        },
        widget: function () {
            return this._widget;
        }
    });

})(jQuery);